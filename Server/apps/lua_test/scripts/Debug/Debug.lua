--初始化日志模块
Debug.LogInit("Test","Debug",true);

Debug.Log("hello","???");

local Player={
	name="Player"
}

function Player:New(instant)
	if not instant then
		instant={};
	end
	setmetatable(instant, self)
	self.__index = self

--	self.name="P-1";

	return instant;
end

function Player:Test()
	print("Player_Test");
end

local RedPlayer=Player:New();

--function RedPlayer:New(instant)
--	if not instant then
--		instant=Player:New(instant);
--	end
--	setmetatable(instant, self)
--	self.__index = self
--	return instant;
--end

function RedPlayer:Test()
	Player.Test(self);
	print("dgasdg");
end

local p = RedPlayer:New();
p:Test();
print(p.name);
