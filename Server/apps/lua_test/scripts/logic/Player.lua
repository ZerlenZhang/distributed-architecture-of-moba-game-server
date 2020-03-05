local mysql = require("datebase/mysql_game");
local Respones = require("Respones");
local RoomState = require("logic/Const/RoomState");
local Redis = require("datebase/redis_center");

local Player={};

function Player:New(instant)
	if not instant then
		instant={};
	end

	setmetatable(instant, self)
	self.__index = self

	return instant;
end

function Player:Init(uid,s,handler)

	self.session=s;
	self.uid=uid;
	--不再任何游戏场
	self.zoneid=-1;
	--不在任何房间
	self.roomid=-1;
	--默认旁观状态
	self.state=RoomState.InView;
	--默认不是机器人
	self.isRobot=false;
	--座位号
	self.seatid=-1;
	--那一边,0 左边，1 右边
	self.side=-1;
	--玩家英雄id
	self.heroid=-1;
	--玩家客户端ip
	self.clientIp=-1;
	--玩家udpPort
	self.clientPort=-1;

	--玩家同步到哪一帧
	self.sync_frame=0;


	--从数据库读取玩家信息
	mysql.GetUgameInfo(uid,
		function ( err,info )
			if err then
				Debug.LogError(err);
				if handler then
					handler(Respones.SystemError);
				end
				return;
			end

			self.gameInfo=info;


			Redis.GetUinfo(self.uid,function ( err,uinfo )
				if err then
					Debug.LogError(err);
					if handler then
						handler(Respones.SystemError);
					end
					return;
				end
				self.uinfo=uinfo;
				if handler then
					handler(Respones.OK);
				end
			end);
		end);
end

function Player:SetSession( s )
	self.session=s;
end

function Player:SendPackage(sType,cType,body)
	if not self.session or self.isRobot then
		return;
	end

	local package = {sType,cType,self.uid,body};
	Session.SendPackage(self.session,package);
end

function Player:UdpSendPackage(sType,cType,body)
	--玩家已经断线或是机器人
	if not self.session or self.isRobot then
		return;
	end

	if self.clientPort==-1 or self.clientIp==-1 then
		return;
	end

	local msg= {sType,cType,0,body};
	Session.UdpSendPackage(self.clientIp,self.clientPort,msg);
end
function Player:GetInfo()
	return {
		unick = self.uinfo.unick,
		uface = self.uinfo.uface,
		usex = self.uinfo.usex,
		seatid = self.seatid,
		side=self.side,
	};
end

function Player:OnEnterRoom(roomid,seatid)
	self.roomid=roomid;
	self.seatid=seatid;
	self.state=RoomState.InView;

	--分边，是否是偶数
	local num1,num2=math.modf(seatid/2)--返回整数和小数部分
	if(num2==0)then
		self.side=-1;
	else
		self.side=1;
	end

end

function Player:OnExitRoom()
	self.roomid=-1;
	self.zoneid=-1;
	self.seatid=-1;
	self.side=-1;
	self.heroid=-1;
	self.sync_frame=0;
	self.state=RoomState.InView;
end

function Player:SetAddr(ip,port)
	self.clientIp=ip;
	self.clientPort=port;
end


return Player;