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

	--当前帧id
	self.frameid=1;
	--所有帧操作
	self.allframes={};
	--当前帧操作
	self.nextframe={
		frameid = self.frameid,
		opts = {},
	};

	print("room init ",self.roomid,self.zoneid);
end

function roomMgr:send_unsync_frames( player )
	local frames = {};

	for i = (player.sync_frame+1), #self.allframes do
		table.insert(frames,self.allframes[i]);
	end
--	print("player.frame:"..player.sync_frame.."self.frameid"..self.frameid.."#self.allframes:"..#self.allframes);
	local body = {frameid=self.frameid,unsync_frames=frames}
	player:UdpSendPackage(ServiceType.Logic,CmdType.eLogicFrame,body);
end

--帧同步函数
function roomMgr:on_frame_synce()

	table.insert(self.allframes,self.nextframe);
	self.frameid=#self.allframes;
--	print("self.frameid "..self.frameid.." #self.allframe: "..#self.allframes);

	for k, player in pairs(self.inviewPlayers) do
		self:send_unsync_frames(player);
	end

	self.frameid=self.frameid+1;
	--当前帧操作
	self.nextframe={
		frameid = self.frameid,
		opts = {},
	};
end
--广播TCP消息
function roomMgr:BroadcastInviewPlayers(stype,ctype,body,except)
	for i,player in ipairs(self.inviewPlayers) do
		if not player.isRobot and player~=except then
			player:SendPackage(stype,ctype,body);
		end
	end
end
--添加玩家
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
--移除玩家
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
--开始游戏
function roomMgr:StartGame()

	--房间进入Ready状态
	self.state=RoomState.Playing ;
	--更新玩家状态
	for i,p in ipairs(self.inviewPlayers) do
		p.state=RoomState.Playing;
	end


	--可能进入选择英雄界面，直到所有玩家选择完毕
	--我们随机选择【大乱斗】
	local heros={};
	for k, player in pairs(self.inviewPlayers) do
		local randomHeroId=math.floor(math.random()*5+1);
		local info = 
		{
			heroid = randomHeroId,
			seatid = player.seatid,
			side   = player.side,
		}
		table.insert(heros,info);
	end

	--广播消息游戏开始
	self:BroadcastInviewPlayers(
		ServiceType.Logic,
		CmdType.eGameStart,
		{players=heros});

	--5 秒以后开始第一个逻辑帧
	self.frameid=0;
	self.frameTimer=Timer.Repeat(
	function()
		self:on_frame_synce();
	end,5000,-1,50);
end
--收到客户端下一帧消息
function roomMgr:OnNextFrame(nextFrameOpts)
	--是否过时
	if nextFrameOpts.frameid~=self.frameid then
		return;
	end
	--是否内容无效
	if #nextFrameOpts.opts <= 0 then
		Debug.LogError("get empty NextFrameOpts");
		return;
	end
	--是否不对应玩家
	local player = self.inviewPlayers[nextFrameOpts.seatid];
	if not player then
		Debug.LogError("OnNextFrame_get player nil, seatid:",nextFrameOpts.seatid);
		return;
	end

	--更新客户端同步到帧数
	if player.sync_frame< nextFrameOpts.frameid-1 then
		player.sync_frame=nextFrameOpts.frameid-1;
	end


	--加入到当前帧操作
	for k, opt in pairs(nextFrameOpts.opts) do
		table.insert(self.nextframe.opts,opt);
	end

--	print("player.frame: "..player.sync_frame.."nextframe.opts:"..#self.nextframe.opts.."self.frameid"..self.frameid.."#self.allframes:"..#self.allframes);
end


return roomMgr;
