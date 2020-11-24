local sType=require("ServiceType");
local cType=require("logic/const/CmdType");

local logicMessageHandler = require("logic/MessageHandler");

local cTypeToCallback = {};
cTypeToCallback[cType.LoginLogicReq] = logicMessageHandler.OnPlayerLoginLogic;
cTypeToCallback[cType.UdpTestReq] = logicMessageHandler.OnUdpTest;



return   {
    OnSessionRecvCmd=function(s,msg)
        --{sType,cType,utag,body}
        Debug.Log("GetMsg: CmdType: "..msg[2]);
        if cTypeToCallback[msg[2]] then
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