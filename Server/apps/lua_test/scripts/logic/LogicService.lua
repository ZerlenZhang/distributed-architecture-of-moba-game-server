local cType=require("logic/Const/CmdType");
local gameMgr = require("logic/GameMgr");

local cTypeToCallback = {};
cTypeToCallback[cType.eLoginLogicReq]=gameMgr.OnLoginLogicReq;
cTypeToCallback[cType.eEnterZoneReq]=gameMgr.EnterZone;
cTypeToCallback[cType.eUserLostConn]=gameMgr.OnPlayerLostConn;
cTypeToCallback[cType.eExitRoomReq]=gameMgr.OnPlayerExitRoom;
cTypeToCallback[cType.eUdpTest]=gameMgr.OnUdpTest;
cTypeToCallback[cType.eNextFrameOpts]=gameMgr.OnNextFrameOpts;

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

    OnSessionConnected=function ( s,sType )
        gameMgr.OnGatewayConn(s,sType);
    end
};