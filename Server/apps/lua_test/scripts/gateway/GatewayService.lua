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
local cmdType = require("auth/Const/CmdType");
local serviceType = require("ServiceType");
local responce = require("Respones");

function IsLoginReturnRes( cType )
    if cmdType.eGuestLoginRes == cType
    or cmdType.eUserLoginRes == cType then
        return true;
    end
    return false;
end

function IsLoginRequestCmd( ctype )
    if cmdType.eGuestLoginReq == ctype
        or cmdType.eUserLoginReq == ctype then
        return true;
    end
    return false;
end

return   {
    OnSessionRecvRaw=function(s,raw)
    	if Session.IsClient(s) then
    		-- 发给客户端
            local sType,cType,utag = RawCmd.ReadHeader(raw);
            local clientSession = nil;
            --如果是登陆命令的回复
            if sType==serviceType.Auth and IsLoginReturnRes(cType) then
                --print("AAA");
                --取消tag_sessionDIc中的引用
                clientSession=tag_sessionDic[utag];
                tag_sessionDic[utag]=nil;

                if clientSession==nil then
                    print("clientSession is null, uTag:", utag);
                    return;
                end

                local body = RawCmd.ReadBody(raw);

                --如果返回结果不正常，直接返回
                --print("status: ",body.status,"responce.Ok",responce.OK);
                if body.status~=responce.OK then
                    print("responce is not ok",body.status);
                    RawCmd.SetUTag(raw,0);
                    Session.SendRawPackage(clientSession,raw);
                    return;
                end
 
                local uid = body.uinfo.uid;

                --判断是否有session已经用这个id登陆
                if uid_sessionDic[uid] and uid_sessionDic[uid]~=clientSession then
                    --说明重复登陆
                    print("somebody relogin");
                    local reloginMsg = {serviceType.Auth,cmdType.eReLogin,0,nil};
                    Session.SendPackage(uid_sessionDic[uid],reloginMsg);
                    Session.Close(uid_sessionDic[uid]);
                    -- uid_sessionDic[uid]=nil;
                end

                --记录uid
                --print("remember uid: ",uid);
                uid_sessionDic[uid]=clientSession;
                Session.SetUId(clientSession,uid);

                --发送回客户端
                --print("send back to client");
                body.uinfo.uid=0;
                local loginRes={sType,cType,0,body};
                Session.SendPackage(clientSession,loginRes);
                return;
            end

            --print("BBB");
            clientSession=uid_sessionDic[utag];
            

            --发给用户
            if clientSession then
                --print("flag");
                --永远不要让用户知道utag
                RawCmd.SetUTag(raw,0);
                Session.SendRawPackage(clientSession,raw);

                --如果是注销消息
                if sType==serviceType.Auth and cType==cmdType.eUserUnregisterRes then
                    Session.SetUId(clientSession,0);
                    uid_sessionDic[utag]=nil;
                end
            else
                print("clientSession is null? utag: ",utag);
            end
    	else
    		-- 发给服务器
    		local sType,cType,utag = RawCmd.ReadHeader(raw);
    		local targetServerSession = sessionDic[sType];
    		if nil==targetServerSession then
    			print("server is null, sType:"..sType);
    			return;
    		end
 
            --如果是登陆请求
            if sType==serviceType.Auth and IsLoginRequestCmd(cType) then
                utag = Session.GetUTag(s);
                if utag==0 then
                    --print("is loginRequest");
                    utag=g_ukey;
                    g_ukey=g_ukey+1;
                    Session.SetUTag(s,utag);
                end  
                tag_sessionDic[utag]=s;
            else
                utag = Session.GetUId(s);
                if utag==0 then
                    --该操作需要先登陆
                    print("you need to login first");
                    return;
                end
                --uid_sessionDic[uid]=s;
            end

    		--打上utag然后发给我们的服务器
            --print("client tag: "..utag.." send to server")
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
    		if tag_sessionDic[utag] and tag_sessionDic[utag]==s then
    			tag_sessionDic[utag]=nil;
    			Session.SetUTag(s,0);
    		end
            --2.把客户端从uid映射表删除
    		local uid = Session.GetUId(s);
    		if uid_sessionDic[uid] and uid_sessionDic[uid]==s then
    			uid_sessionDic[uid]=nil;
                --这里不能把用户uid值为零，用户注册的每个服务都会走这段代码
                --如果第一次我们就把uid置为零，后面走这段代码的服务器就不知道
                --是哪个用户掉线了
    			--Session.SetUId(s,0);
    			--table.remove(uid_sessionDic,uid);
    		end

            local targetServer = sessionDic[sType];
            if nil==targetServer then
                return;
            end

            --客户端uid用户掉线，我要把这个事件告诉中转服务器
            if uid~=0 then
                local userLostMsg = {sType,cmdType.eUserLostConn,uid,nil};
                Session.SendPackage(targetServer,userLostMsg);
            end
    	end
    end,
};