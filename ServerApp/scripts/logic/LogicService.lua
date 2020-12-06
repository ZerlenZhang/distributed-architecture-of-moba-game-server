local sType=require("ServiceType");
local cType=require("logic/const/CmdType");

local logicMessageHandler = require("logic/MessageHandler");

local cTypeToCallback = {};
cTypeToCallback[cType.UserLostConn] = logicMessageHandler.OnUserLostConn;
cTypeToCallback[cType.LoginLogicReq] = logicMessageHandler.OnPlayerLoginLogic;
cTypeToCallback[cType.UdpTestReq] = logicMessageHandler.OnUdpTest;
cTypeToCallback[cType.StartMatchReq] = logicMessageHandler.OnStartMatch;
cTypeToCallback[cType.StopMatchReq] = logicMessageHandler.OnStopMatch;
cTypeToCallback[cType.SelectHeroReq] = logicMessageHandler.OnSelectHero;
cTypeToCallback[cType.SubmitHeroReq] = logicMessageHandler.OnSubmitHero;
cTypeToCallback[cType.StartGameReq] = logicMessageHandler.OnStartGameReq;
cTypeToCallback[cType.NextFrameInput] = logicMessageHandler.OnGetNextFrameInput;
cTypeToCallback[cType.InitUdpReq] = logicMessageHandler.OnInitUdp;



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