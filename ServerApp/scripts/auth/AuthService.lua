local sType=require("ServiceType");
local cType=require("auth/const/CmdType");

local authMessageHandler = require("auth/MessageHandler");

local cTypeToCallback = {};
cTypeToCallback[cType.UserLostConn]=authMessageHandler.UserLostConn;
cTypeToCallback[cType.EditProfileReq]=authMessageHandler.EditProfile;
cTypeToCallback[cType.UserLoginReq]=authMessageHandler.UserLogin;
cTypeToCallback[cType.UserUnregisterReq]=authMessageHandler.UserUnregister;



return   {
    OnSessionRecvCmd=function(s,msg)
        --{sType,cType,utag,body}
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