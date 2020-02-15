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
				Debug.LogError("failed to connect to server ["..config.servers[sType].descrip.."]"..ip..port);
				return;
			end
			--链接成功
			print("success to connect to server ["..config.servers[sType].descrip.."]"..ip..port);
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
    OnSessionDisconnected=function(s)
    	if Session.IsClient(s) then
    		--连接到其他服务器的链接断开了
    		for k,v in pairs(sessionDic) do
    			if v==s then
    				print("GatewayServer disconnected from: ["..config.servers[k].descrip.."]");
    				sessionDic[k]=nil;
    			end
    		end
    		return;
    	else
    		--连接到客户端的链接断开了

    		print("client removed");
    		--1.把客户端删除
    		local utag = Session.GetUTag(s);
    		if tag_sessionDic[uTag]~=nil then
    			tag_sessionDic[uTag]=nil;
    			Session.SetUTag(s,0);
    			table.remove(tag_sessionDic,uTag);
    		end
    		local uid = Session.GetUId(s);
    		if uid_sessionDic[uid]~=nil then
    			uid_sessionDic[uid]=nil;
    			Session.SetUTag(s,0);
    			table.remove(uid_sessionDic,uid);
    		end
    	end
    end,
};