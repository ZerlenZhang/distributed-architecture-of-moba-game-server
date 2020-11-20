local sType=require("ServiceType");
local cType=require("auth/Const/CmdType");

local guestLogic = require("auth/Guest");

local cTypeToCallback = {};
cTypeToCallback[cType.UserLostConn]=guestLogic.UserLostConn;
cTypeToCallback[cType.EditProfileReq]=guestLogic.EditProfile;
cTypeToCallback[cType.UserLoginReq]=guestLogic.UserLogin;
cTypeToCallback[cType.UserUnregisterReq]=guestLogic.UserUnregister;



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