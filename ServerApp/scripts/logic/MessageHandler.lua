local Respones = require("Respones")
local Stype = require("ServiceType")
local CmdType = require("logic/const/CmdType")
local config= require("GameConfig");


local function login_logic_server(s, req)
	local uid = req[3]
	local stype = req[1]
    local logicServerConfig=config.servers[Stype.Logic];

    if config.enable_proto_log then
        Debug.Log("a player login LogicServer")
    end

    Session.SendPackage(s,{stype,CmdType.LoginLogicRes,uid,{
        udp_ip=logicServerConfig.udp_ip,
        udp_port=logicServerConfig.udp_port,
    }});

	-- local p = logic_server_players[uid] -- player对象
	-- if p then -- 玩家对象已经存在了，更新一下session就可以了; 
	-- 	p:set_session(s)
	-- 	p:set_udp_addr(body.udp_ip, body.udp_port)
	-- 	send_sta tus(s, stype, Cmd.eLoginLogicRes, uid, Respones.OK)
	-- 	return
	-- end

	-- p = player:new()
	-- p:init(uid, s, function(status)
	-- 	if status == Respones.OK then
	-- 		logic_server_players[uid] = p
	-- 		online_player_num = online_player_num + 1
	-- 	end
	-- 	send_status(s, stype, Cmd.eLoginLogicRes, uid, status)
	-- end)

	-- p:set_udp_addr(body.udp_ip, body.udp_port)
end

local function udp_test(s, req) 
	local stype = req[1]
	local ctype = req[2]
	local body = req[4]


	local msg = {stype, CmdType.UdpTestRes, 0, {
		content = "success! "..body.content,
	}}

    local ip,port=Session.GetAddress(s);
    
	Debug.Log("UdpTest: "..body.content.." IP-"..ip.." Port-"..port);
	Session.UdpSendPackage(ip,port,msg);
end

local function on_start_match(s,req)
    --do some thing
    if config.enable_proto_log then
        Debug.Log("A player start match")
    end

    Session.SendPackage(s,{
        req[1],
        CmdType.StartMatchRes,
        req[3],
        nil
    });
end

local function on_user_lost_conn(s,req)
end

return {
	OnUdpTest = udp_test,
    OnPlayerLoginLogic = login_logic_server,
    OnStartMatch = on_start_match,
    OnUserLostConn = on_user_lost_conn,
}

