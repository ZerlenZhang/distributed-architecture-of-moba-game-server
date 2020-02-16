local sType=require("ServiceType");
local cType=require("CmdType");

return   {
    OnSessionRecvCmd=function(s,msg)
    	print(msg[1],msg[2],msg[3],msg[4].guest_key);
    	
    	--local pk = {sType.Auth,cType.eLoginRes,msg[3],{status=1}};
    	--Session.SendPackage(s,pk);
    end,
    OnSessionDisconnected=function(s,sType)
    	local ip,port = Session.GetAddress(s);
    	print("client ["..ip..":"..port.."] leave");
    end,
};