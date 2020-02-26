local mysql = require("datebase/mysql_game");
local Respones = require("Respones");
local Zone = require("logic/Const/Zone");
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

function Player:GetInfo()

	if self.isRobot then
		return {
			unick = "DefultUnick_"..self.uid,
			uface = -1,
			usex = -1,
		}
	else
		return {
			unick = self.uinfo.unick,
			uface = self.uinfo.uface,
			usex = self.uinfo.usex,
		};
	end
end

return Player;