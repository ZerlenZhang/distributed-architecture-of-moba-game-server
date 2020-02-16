local config = require("GameConfig");

-- stype -> session 映射表
local sessionDic={}

-- 当前正在链接的服务器
local connectingSession={};


--连接到指定服务器
function ConnectToServer(sType,ip,port)
	Netbus.TcpConnect(ip,port,
		--链接成功的回调
		function(err,session)
			connectingSession[sType]=false;
			if err~=0 then
				Debug.LogError("connect to server ["..config.servers[sType].descrip.."]"..ip..port.." failed, reconnecting...");
				return;
			end
			--链接成功
			print("connect to server ["..config.servers[sType].descrip.."]"..ip..port.." success");
			sessionDic[sType]=session;
		end)
end


--检查服务器的链接
function CheckServerConnect()
	-- 检查服务的链接，每秒进行一次
	for k,v in pairs(config.servers) do
		if sessionDic[v.serviceType]==nil 
			and connectingSession[v.serviceType]==false 
			then
			--如果没有链接，就链接服务器
			connectingSession[v.serviceType]=true;
			ConnectToServer(v.serviceType,v.ip,v.port);
		end
	end
end


--初始化网关服务器
function GatewayServiceInit()
	for k,v in pairs(config.servers) do
		sessionDic[v.serviceType]=nil;
		connectingSession[v.serviceType]=false;
	end
	--启动一个定时器
	Timer.Repeat(CheckServerConnect,1000,-1,5000);
end


GatewayServiceInit();

local g_ukey=1;
local tag_sessionDic={};
local uid_sessionDic={};

return   {
    OnSessionRecvRaw=function(s,raw)
    	if Session.IsClient(s) then
    		-- 发给客户端
            local sType,cType,utag = RawCmd.ReadHeader(raw);

            --根据utag查找可客户端session
            --必须要区分命令时登陆前还是登陆后
            --只有命令的类型才知道我峨嵋你是要到uid里查，还是到utag里查
            --先预留出来，做登陆的时候在做
            local clientSession = nil;
            if uid_sessionDic[utag]~=nil then
                clientSession=uid_sessionDic[utag];
            elseif tag_sessionDic[utag]~=nil then
                clientSession=tag_sessionDic[utag];
            end
            --永远不要让用户知道utag
            RawCmd.SetUTag(raw,0);

            --发给用户
            if clientSession then
                Session.SendRawPackage(clientSession,raw);
            end
    	else
    		-- 发给服务器
    		local sType,cType,utag = RawCmd.ReadHeader(raw);
    		local targetServerSession = sessionDic[sType];
    		if nil==targetServerSession then
    			print("server is null, sType:"..sType);
    			return;
    		end

    		local uid = Session.GetUId(s);
    		if uid == 0 then
    			--登陆以前
    			utag = Session.GetUTag(s);
    			if utag==0 then
    				utag=g_ukey;
    				g_ukey=g_ukey+1;
    				tag_sessionDic[utag]=s;
    				Session.SetUTag(s,utag);
    			end
    		else--登陆以后
    			utag=uid;
    			uid_sessionDic[utag]=s;
    		end
    		--打上utag然后发给我们的服务器
    		RawCmd.SetUTag(raw,utag);
    		Session.SendRawPackage(targetServerSession,raw);
    	end
    end,
    OnSessionDisconnected=function(s,sType)

    	if Session.IsClient(s) then 
    		for k,v in pairs(sessionDic) do
    			if v==s then
    				Debug.LogError("disconnected from: ["..config.servers[k].descrip.."]");
    				sessionDic[k]=nil;
    			end
    		end
    		return;
    	else
    		--print("client leave");

            local ip,port = Session.GetAddress(s);
            print("client ["..ip..":"..port.."] leave");

    		--1.把客户端从utag临时映射表删除
    		local utag = Session.GetUTag(s);
    		if tag_sessionDic[uTag]~=nil then
    			tag_sessionDic[uTag]=nil;
    			Session.SetUTag(s,0);
    			--table.remove(tag_sessionDic,uTag);
    		end
            --2.把客户端从uid映射表删除
    		local uid = Session.GetUId(s);
    		if uid_sessionDic[uid]~=nil then
    			uid_sessionDic[uid]=nil;
                --这里不能把用户uid值为零，用户注册的每个服务都会走这段代码
                --如果第一次我们就把uid置为零，后面走这段代码的服务器就不知道
                --是哪个用户掉线了
    			--Session.SetUId(s,0);n
    			--table.remove(uid_sessionDic,uid);
    		end

            --客户端uid用户掉线，我要把这个事件告诉中转服务器
            if uid~=0 then

            end
    	end
    end,
};