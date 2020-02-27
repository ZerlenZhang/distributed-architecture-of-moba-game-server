local ServiceType = require("ServiceType");
local CmdType = require("logic/Const/CmdType");
local Respones = require("Respones");
local Player = require("logic/Player");
local Zone = require("logic/Const/Zone");
local RoomState = require("logic/Const/RoomState");

local g_roomId = 1;
local PlayerNum = 3;

local roomMgr={};

function roomMgr:New(instant)
	if not instant then
		instant={};
	end

	setmetatable(instant,{__index=self});

	return instant;
end

function roomMgr:Init(zoneid)
	--区域ID
	self.zoneid=zoneid;
	--房间ID
	self.roomid=g_roomId;
	g_roomId=g_roomId+1;

	--集结状态
	self.state=RoomState.InView;

	--正在集结列表
	self.inviewPlayers={};
	self.leftPlayers={};
	self.rightPlayers={};

	print("room init ",self.roomid,self.zoneid);
end

function roomMgr:BroadcastInviewPlayers(stype,ctype,body,except)
	for i,player in ipairs(self.inviewPlayers) do
		if not player.isRobot and player~=except then
			player:SendPackage(stype,ctype,body);
		end
	end
end

function roomMgr:AddPlayer( player )

	if not player then
		print("AddPlayer: Player is nil");
		return;
	end

	if #self.inviewPlayers>=2*PlayerNum then
		Debug.LogError("room has full");
		return;
	end
--	print("add player: ",player.uinfo.unick);

	if self.state~=RoomState.InView
		or player.roomid~=-1 
		or player.state~=RoomState.InView then
		return false;
	end


	--获取座位号
	local index;
	for index = 1, PlayerNum*2 do
		if self.inviewPlayers[index]==nil then
			self.inviewPlayers[index]=player;
			player:OnEnterRoom(self.roomid,index);
			break;
		end
	end


	if not player.isRobot then

		--告诉客户端进入房间
		player:SendPackage(ServiceType.Logic,CmdType.eEnterRoom,
			{
				zoneid=player.zoneid,
				roomid=player.roomid,
				seatid=player.seatid,
				side=player.side,
			});
		--告诉客户端在它之前已经有一些人进来了
		for k,p in pairs(self.inviewPlayers) do
			if p~=player then
				player:SendPackage(
					ServiceType.Logic,
					CmdType.ePlayerEnterRoom,
					p:GetInfo());
			end
		end


	end
	--end



	--告诉房间内其他客户端有人进来
	self:BroadcastInviewPlayers(
		ServiceType.Logic,
		CmdType.ePlayerEnterRoom,
		player:GetInfo(),
		player);

	--是否集结结束
	if #self.inviewPlayers >= PlayerNum*2 then
		self:StartGame();
	end

	return true;
end

function roomMgr:RemovePlayer(player)
	if not player then
		Debug.LogError("player is nil");
		return;
	end

	--从列表中移除player
	self.inviewPlayers[player.seatid]=nil;

	--告诉其他人有人离开
	self:BroadcastInviewPlayers(
		ServiceType.Logic,
		CmdType.ePlayerExitRoom,
		{seatid=player.seatid},
		player);

	--离开成功
	player:SendPackage(
		ServiceType.Logic,
		CmdType.eExitRoomRes,
		{status=Respones.OK,});

	player:OnExitRoom();
end

function roomMgr:StartGame()

	--房间进入Ready状态
	self.state=RoomState.Ready;
	--更新玩家状态
	for i,p in ipairs(self.inviewPlayers) do
		p.state=RoomState.Ready;
	end


	--可能进入选择英雄界面，直到所有玩家选择完毕
	--我们随机选择【大乱斗】
	local heros={};
	for k, player in pairs(self.inviewPlayers) do
		local heroid=math.floor(math.random()*5+1);
		player.heroid=heroid;
		table.insert(heros,heroid);
	end

	--广播消息游戏开始
	self:BroadcastInviewPlayers(
		ServiceType.Logic,
		CmdType.eGameStart,
		{heros=heros});
end

return roomMgr;
