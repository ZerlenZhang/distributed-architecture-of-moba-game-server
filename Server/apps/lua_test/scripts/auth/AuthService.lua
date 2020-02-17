local sType=require("ServiceType");
local cType=require("auth/Const/CmdType");

local guestLogic = require("auth/Guest");

local cTypeToCallback = {};
cTypeToCallback[cType.eGuestLoginReq]=guestLogic.login;

return   {
    OnSessionRecvCmd=function(s,msg)
        --{sType,cType,utag,body}
        if cTypeToCallback[msg[2]] then
            cTypeToCallback[msg[2]](s,msg);
        end
    	--local pk = {sType.Auth,cType.eLoginRes,msg[3],{status=1}};
    	--Session.SendPackage(s,pk);
    end,
    OnSessionDisconnected=function(s,sType)
    	local ip,port = Session.GetAddress(s);
    	print("client ["..ip..":"..port.."] leave");
    end,
};