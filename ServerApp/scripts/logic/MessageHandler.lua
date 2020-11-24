local Respones = require("Respones")
local Stype = require("ServiceType")
local CmdType = require("logic/const/CmdType")



-- Scheduler.schedule(do_push_robot_to_match, 1000, -1, 5000)

-- {stype, ctype, utag, body}
function login_logic_server(s, req)
	local uid = req[3]
	local stype = req[1]
    local body = req[4];
    Debug.Log("player ip"..body.ip..", port:"..body.udp_port);
    Session.SendPackage(s,{stype,CmdType.LoginLogicRes,uid,{status=Respones.Ok}});

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


function do_udp_test(s, req) 
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

local game_mgr = {
	OnUdpTest = do_udp_test,
	OnPlayerLoginLogic = login_logic_server,
}

return game_mgr

