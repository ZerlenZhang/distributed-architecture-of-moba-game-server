local sType=require("ServiceType");
local cType=require("system/Const/CmdType");

local gameLogic = require("system/SystemLogic");

local cTypeToCallback = {};
cTypeToCallback[cType.eGetMobaInfoReq]=gameLogic.GetUgameInfo;
cTypeToCallback[cType.eRecvLoginBonuesReq]=gameLogic.RecvLoginBonues;

return   {
    OnSessionRecvCmd=function(s,msg)
        --{sType,cType,utag,body}


        if cTypeToCallback[msg[2]] then
	        if nil==msg then
	        	Debug.LogError("????")
	        	return;
	        end        	
            cTypeToCallback[msg[2]](s,msg);
        else
            Debug.LogError("unregistered cmdType: ",msg[2]);
        end
    end,
    OnSessionDisconnected=function(s,sType)
    	local ip,port = Session.GetAddress(s);
    	print("client ["..ip..":"..port.."] leave");
    end,
};