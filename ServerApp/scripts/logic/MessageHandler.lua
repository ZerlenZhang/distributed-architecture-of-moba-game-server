local Respones = require("Respones")
local Stype = require("ServiceType")
local CmdType = require("logic/const/CmdType")
local config= require("GameConfig");
local MatchMgr=require("logic/MatchMgr");

local logicServerConfig=config.servers[Stype.Logic];
local LogicConfig=require("logic/LogicConfig");

local function login_logic_server(s, req)
	local utag = req[3];
	local stype = req[1];

    if config.enable_proto_log then
        Debug.Log("a player login LogicServer")
    end

	MatchMgr.InitPlayer(s, utag, req[4].uname);
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
	local roomType = req[2];
	local uname = req[4].uname;
    if config.enable_proto_log then
        Debug.Log(uname.." try match")
	end

	MatchMgr.OnPlayerTryMatch(uname,roomType);
end

local function on_stop_match(s,req)
    if config.enable_proto_log then
        Debug.Log(req[4].uname.." try stop match");
	end
	MatchMgr.OnPlayerTryStopMatch(req[4].uname);
end


local function on_user_lost_conn(s,req)
    if config.enable_proto_log then
        Debug.Log("User["..req[3].."] lost conn");
	end
	MatchMgr.OnUserLostConn(req[3]);
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

local function on_init_udp(s,req)
	local uname=req[4].uname;
	local ip,port=Session.GetAddress(s);
	MatchMgr.SetUdpAddr(uname,ip,port);
end

local function on_start_multi_match(s,req)
	local roomType = req[2];
	local uname = req[4].uname;
    if config.enable_proto_log then
        Debug.Log(uname.." try match multi");
	end

	MatchMgr.OnPlayerTryMatch(uname,roomType);
end

local function on_start_story_mode(s, req)
	Session.SendPackage(s,{req[1],CmdType.StartStoryRes,req[3]});
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
	OnInitUdp = on_init_udp,
	OnStartMultiMatch = on_start_multi_match,
	OnStartStoryMode = on_start_story_mode,
}

