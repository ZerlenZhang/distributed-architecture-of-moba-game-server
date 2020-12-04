local Respones = require("Respones")
local Stype = require("ServiceType")
local CmdType = require("logic/const/CmdType")
local config= require("GameConfig");
local MatchMgr=require("logic/MatchMgr");

local logicServerConfig=config.servers[Stype.Logic];
local LogicConfig=require("logic/LogicConfig");

local function login_logic_server(s, req)
	local uid = req[3]
	local stype = req[1]

    if config.enable_proto_log then
        Debug.Log("a player login LogicServer")
    end

    Session.SendPackage(s,{stype,CmdType.LoginLogicRes,uid,{
        udp_ip=LogicConfig.udp_ip,
        udp_port=LogicConfig.udp_port,
    }});
end

local function udp_test(s, req) 
	local stype = req[1]
	local ctype = req[2]
	local body = req[4]


	local msg = {stype, CmdType.UdpTestRes, 0, {
		content = "success! "..body.content,
	}}

	local ip,port=Session.GetAddress(s);

    if config.enable_proto_log then
		Debug.Log("UdpTest: "..body.content.." IP-"..ip.." Port-"..port);
	end

	Session.UdpSendPackage(ip,port,msg);
end

local function on_start_match(s,req)
    if config.enable_proto_log then
        Debug.Log(req[4].uname.." try match")
	end

	MatchMgr.OnPlayerTryMatch(s,req[3],req[2],req[4]);
end

local function on_stop_match(s,req)
    if config.enable_proto_log then
        Debug.Log(req[4].uname.." try stop match");
	end
	MatchMgr.OnPlayerTryStopMatch(req[4].uname);
end


local function on_user_lost_conn(s,req)
end

local function on_select_hero(s,req)
	if not req[4].hero_id then
		Debug.LogError("there is no heroId");
		return;
	end
    if config.enable_proto_log then
        Debug.Log(req[4].uname.." select hero "..req[4].hero_id);
	end
	MatchMgr.OnPlayerSelectHero(req[4].uname,req[4].hero_id);
end

local function on_submit_hero(s,req)
    if config.enable_proto_log then
        Debug.Log(req[4].uname.." submit hero ");
	end
	MatchMgr.OnPlayerSubmitHero(req[4].uname);
end

local function on_start_game(s,req)
    if config.enable_proto_log then
        Debug.Log(req[4].uname.." request to start game");
	end
    MatchMgr.OnPlayerTryStartGame(req[4].uname);
end

local function on_get_next_frame_input(s,req)
    MatchMgr.OnTakeFrameInput(req[4].roomType,req[4].roomId,req[4].frameId,req[4].seatId,req[4].inputs);
end

return {
	OnUdpTest = udp_test,
    OnPlayerLoginLogic = login_logic_server,
    OnStartMatch = on_start_match,
	OnUserLostConn = on_user_lost_conn,
	OnStopMatch = on_stop_match,
	OnSelectHero = on_select_hero,
	OnSubmitHero = on_submit_hero,
	OnStartGameReq = on_start_game,
	OnGetNextFrameInput = on_get_next_frame_input,
}

