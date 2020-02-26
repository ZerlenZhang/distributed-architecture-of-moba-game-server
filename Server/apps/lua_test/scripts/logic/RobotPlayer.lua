--
-- Created by IntelliJ IDEA.
-- User: ReadyGamerOne
-- Date: 2020/2/24
-- Time: 19:56
-- To change this template use File | Settings | File Templates.
--

local Player=require("logic/Player");

local RobotPlayer=Player:New();

function RobotPlayer:Init(uid,s,handler)
	--调用基类Init
	Player.Init(self,uid,s,handler);

	self.isRobot=true;
end

return RobotPlayer;




