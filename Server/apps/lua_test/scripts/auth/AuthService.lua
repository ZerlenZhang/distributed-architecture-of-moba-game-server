local sType=require("ServiceType");
local cType=require("auth/Const/CmdType");

local guestLogic = require("auth/Guest");

local cTypeToCallback = {};
cTypeToCallback[cType.eGuestLoginReq]=guestLogic.login;
cTypeToCallback[cType.eEditProfileReq]=guestLogic.edit_profile;

return   {
    OnSessionRecvCmd=function(s,msg)
        print("???");
        --{sType,cType,utag,body}
        if cTypeToCallback[msg[2]] then
            cTypeToCallback[msg[2]](s,msg);
        else
            Debug.LogError("unregistered cmdType: ",msg[2]);
        end
    	--local pk = {sType.Auth,cType.eLoginRes,msg[3],{status=1}};
    	--Session.SendPackage(s,pk);
    end,
    OnSessionDisconnected=function(s,sType)
    	local ip,port = Session.GetAddress(s);
    	print("client ["..ip..":"..port.."] leave");
    end,
};