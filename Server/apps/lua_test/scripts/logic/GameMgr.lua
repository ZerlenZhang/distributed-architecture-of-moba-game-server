local ServiceType = require("ServiceType");
local CmdType = require("logic/Const/CmdType");
local Respones = require("Respones");
local Player = require("logic/Player");
local RobotPlayer =require("logic/RobotPlayer");
local Zone = require("logic/Const/Zone");
local MatchMgr = require("logic/MatchMgr");
local RoomState = require("logic/Const/RoomState");
local CenterRedis = require("datebase/redis_center");
local GameRedis = require("datebase/redis_game");
local CenterMysql = require("datebase/mysql_center");
local GameMysql = require("datebase/mysql_game");

--在线玩家【uid->Player】
local onlinePlayers = {};
local onlinePlayerCount = 0;
--地区等待列表【zoneWaitingPlayerList[Zone.Sgyd]={},uid->Player]
local zoneWaitingPlayerList = {};
zoneWaitingPlayerList[Zone.Sgyd]={};
zoneWaitingPlayerList[Zone.Assy]={};
--机器人列表
local zoneRobotList={};
zoneRobotList[Zone.Sgyd]={};
zoneRobotList[Zone.Assy]={};
--当前房间列表【roomList[Zone.Sgyd]={},uid->room]
local zoneRoomList = {};
zoneRoomList[Zone.Sgyd]={};
zoneRoomList[Zone.Assy]={};

local _robot_game_infos_={};
local _robot_load_index_=1;

--实例化机器人对象
local function _do_new_robotplayers(robots)
	local halfLen = math.floor(#robots * 0.5);

	local index = 0;
	for k, robot in pairs(robots) do
		local rp = RobotPlayer:New();
		rp:Init(robot.uid,nil,nil);

		if index < halfLen then
			zoneRobotList[Zone.Sgyd][robot.uid]=rp;
		else
			zoneRobotList[Zone.Assy][robot.uid]=rp;
		end

		index = index + 1;
	end
end

--加载机器人角色信息到Redis【uinfo】
local function _do_load_robot_uinfo(uid)
	CenterMysql.GetUinfoByUid(uid,
		function(err,uinfo)
			if err or not uinfo then
				Debug.LogError(err);
				return;
			end

			--设置到Redis中去
			CenterRedis.SetUinfo(uid,uinfo,
				function()
					--加载完最后一个的时候开始实例化机器人对象
					if _robot_load_index_ == #_robot_game_infos_ then
						_do_new_robotplayers(_robot_game_infos_);
						_robot_game_infos_=nil;
					end
					_robot_load_index_ = _robot_load_index_ + 1;
				end);
		end);
end

--加载机器人游戏信息Redis【ginfo】
local function _do_load_robot_ugame_info()
--	print("_do_load_robot_ugame_info");
	GameMysql.GetRobotUgameInfo(
	function(err,robots)
		if err then
			Debug.LogError(err);
			return;
		end
		if not robots or #robots<=0 then
			Debug.LogError("not robots or #robots<=0");
			return;
		end

		--保存到全局变量
		_robot_game_infos_=robots;

		--写入Redis
		for k, robot in pairs(robots) do
			GameRedis.SetUgameInfo(robot.uid,robot,
				function()
					_do_load_robot_uinfo(robot.uid);
				end);
		end
	end
	);
end

--加载机器人信息
local function _load_robots()
--	print("_load_robots");
	if not CenterRedis.IsConnect() or
	   not GameRedis.IsConnect() or
	   not CenterMysql.IsConnect() or
	   not GameMysql.IsConnect() then
	   Timer.Once(_load_robots,5000);
		return;
	end
	_do_load_robot_ugame_info();

end

--获取空闲机器人
local function _search_idle_robot(zid)
	for k, robot in pairs(zoneRobotList[zid]) do
		if robot.roomid==-1 then
			return robot;
		end
	end

	return nil;
end

--把空闲机器人匹配到房间中去
local function _do_push_robots_to_room()
	for zid, roomList in pairs(zoneRoomList) do
--		print("zoneType",zid,"rooms",#roomList);
		for roomid, room in pairs(roomList) do
			if room.state==RoomState.InView then

--				print("find room",room.roomid,roomid);
				--找一个空闲机器人
				local robot=_search_idle_robot(zid);
				if robot then
					room:AddPlayer(robot);
--					test
--					Timer.Once(function()
--						room:RemovePlayer(robot)
--					end,5000);
				end
			end
		end
	end
end

--查询还在集结状态的房间，没有就创建
local function _search_inview_room( zoneType )
	local targetRooms = zoneRoomList[zoneType];

	for roomid,room in ipairs(targetRooms) do
		if room.state==RoomState.InView then
			return room;
		end
	end

	--没有就创建房间
	local room = MatchMgr:New();
	room:Init(zoneType);

	if not room then
		Debug.LogError("create room failed");
		return;
	elseif not room.roomid then
		Debug.LogError("room.roomid is nil");
		return;
	end

	targetRooms[room.roomid]=room;

	print("create room",room.roomid);


	return room;
end

--匹配玩家
local function _do_match_players()

	for zoneType,waitlist in pairs(zoneWaitingPlayerList) do
		if not waitlist then
			Debug.LogError("waiting list is null，zoneType: ",zoneType);
			return;
		end

		for uid,player in pairs(waitlist) do
			local room = _search_inview_room(zoneType);
			if room then
				if not room:AddPlayer(player) then
					Debug.LogError("player enter room error,player.state",player.state,"room.state",room.state);
				end

				--进入房间成功，将玩家移出等待队列
				waitlist[player.uid]=nil;
			end
		end
	end
	
end

--返回区域ID的合法性
local function _check_zoneid( zid )
	for k,v in pairs(Zone) do
		if v==zid then
			return true;
		end
	end
	return false;
end

--发送消息
local function _send_status( s,stype,ctype,uid,_status)

	local msg = {stype,ctype,uid,
	{
		status=_status,
	}};
	Session.SendPackage(s,msg);
end


--玩家登陆请求
local function on_login_logic_req( s,req )
	local uid = req[3];
	local stype = req[1];
	local p = onlinePlayers[uid];
	local msg=req[4];

	--玩家已经存在
	if p then
		--更新session
		p:SetSession(s);
		p:SetAddr(msg.ip,msg.udp_port);
		_send_status(s,stype,CmdType.eLoginLogicRes,uid,Respones.OK);
		return;
	end

	--保存Player对象
	p=Player:New();
	p:Init(uid,s,function ( status )
		--出错
		if status~=Respones.OK then
			_send_status(s,stype,CmdType.eLoginLogicRes,uid,status);
			return;
		end
		--成功
		onlinePlayers[uid]=p;
		onlinePlayerCount=onlinePlayerCount+1;
		print("Player [ "..uid.." ] come in");
		_send_status(s,stype,CmdType.eLoginLogicRes,uid,Respones.OK);
		p:SetAddr(msg.ip,msg.udp_port);
	end);
end

--玩家短线
local function on_player_lost_conn( s,req )
	local uid = req[3];

	local p = onlinePlayers[uid];
	if not p then
		return;
	end

	p:SetSession(nil);
	p:SetAddr(-1,-1);
	--游戏中的玩家
	if p.zoneid~=-1 then
		--是否在地区中
		if zoneWaitingPlayerList[p.zoneid][uid]
			and zoneWaitingPlayerList[p.zoneid][uid]==p then
			--移除玩家
			zoneWaitingPlayerList[p.zoneid][uid]=nil;
			p.zoneid=-1;
		end
	end

	--玩家下线
	print("Player [ "..uid.." ] leave");
	onlinePlayers[uid]=nil;
	onlinePlayerCount=onlinePlayerCount-1;
end

--网关掉线
local function on_gateway_disconnect(s)
	local k,v;
	for k,v in pairs(onlinePlayers) do
		v:SetSession(nil);
	end
end

--网关连接
local function on_gateway_connect( s,stype )
	print("gateway connect to logic");
	local k,v;
	for k,v in pairs(onlinePlayers) do
		v:SetSession(s);
	end

end

--进入区域
local function enter_zone( s,req )
	local stype = req[1];
	local uid = req[3];

	local p = onlinePlayers[uid];
	--检查Player以及所在区域的合法性
	if not p then
		Debug.LogError("player is null ???");
		_send_status(s,stype,CmdType.eEnterZoneRes,uid,Respones.InvalidOprate);
		return;
	elseif p.zoneid~=-1 then
		Debug.LogError("player has entered zone: ",p.zoneid);
		_send_status(s,stype,CmdType.eEnterZoneRes,uid,Respones.InvalidOprate);
		return;
	end


	local zid = req[4].zoneid;
	if not _check_zoneid(zid) then
		_send_status(s,stype,CmdType.eEnterZoneRes,uid,Respones.InvalidParams);
		return;
	end

	--加入等待列表
	if not zoneWaitingPlayerList[zid] then
		zoneWaitingPlayerList[zid]={};
	end

	zoneWaitingPlayerList[zid][uid]=p;
	p.zoneid=zid;
	_send_status(s,stype,CmdType.eEnterZoneRes,uid,Respones.OK);
	return;
end

--离开房间
local function on_player_exit_room(s,req)
	local uid = req[3];
	local player = onlinePlayers[uid];
	if not player or player.roomid==-1 or player.zoneid==-1 or player.seatid==-1 or player.state~=RoomState.InView
		then
		_send_status(s,req[1],CmdType.eExitRoomRes,Respones.InvalidOprate);
		return;
	end

	local room = zoneRoomList[player.zoneid][player.roomid];
	if not room or room.state~=RoomState.InView then
		_send_status(s,req[1],CmdType.eExitRoomRes,Respones.InvalidOprate);
		return;
	end

	room:RemovePlayer(player);

end

--Udp测试消息处理
local function on_udp_test(s,req)
	print("get conent: ", req[4].content);
	Session.SendPackage(s,req);
end

GameRedis.Connect();
GameMysql.Connect();
CenterRedis.Connect();
CenterMysql.Connect();

--加载机器人信息
_load_robots();

--启动一个定时器，定期匹配玩家
Timer.Repeat(_do_match_players,1000,-1,5000);
--定期匹配机器人
Timer.Repeat(_do_push_robots_to_room,1000,-1,2000);

return{
	OnLoginLogicReq     =   on_login_logic_req,
	OnPlayerLostConn    =   on_player_lost_conn,
	OnGatewayLostConn   =   on_gateway_disconnect,
	OnGatewayConn       =   on_gateway_connect,
	EnterZone           =   enter_zone,
	OnPlayerExitRoom    =   on_player_exit_room,
	OnUdpTest           =   on_udp_test,
};