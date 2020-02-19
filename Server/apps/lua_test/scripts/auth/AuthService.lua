local sType=require("ServiceType");
local cType=require("auth/Const/CmdType");

local guestLogic = require("auth/Guest");

local cTypeToCallback = {};
cTypeToCallback[cType.eGuestLoginReq]=guestLogic.GuestLogin;
cTypeToCallback[cType.eEditProfileReq]=guestLogic.EditProfile;
cTypeToCallback[cType.eAccountUpgradeReq]=guestLogic.AccountUpgrade;
cTypeToCallback[cType.eUserLoginReq]=guestLogic.UserLogin;
cTypeToCallback[cType.eUserUnregisterReq]=guestLogic.UserUnregister;



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