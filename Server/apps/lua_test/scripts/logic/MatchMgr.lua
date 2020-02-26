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

--	print("add player: ",player.uinfo.unick);

	if self.state~=RoomState.InView
		or player.roomid~=-1 
		or player.state~=RoomState.InView then
		return false;
	end

	--玩家进入集结列表
	table.insert(self.inviewPlayers,player);
	player.roomid=self.roomid;
	player.zoneid=self.zoneid;

	if not player.isRobot then

		--告诉客户端进入房间
		player:SendPackage(ServiceType.Logic,CmdType.eEnterRoom,
			{
				zoneid=self.zoneid,
				roomid=self.roomid,
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
		--房间进入Ready状态
		self.state=RoomState.Ready;
		--更新玩家状态
		for i,p in ipairs(self.inviewPlayers) do
			p.state=RoomState.Ready;
		end
	end

	return true;
end
return roomMgr;
