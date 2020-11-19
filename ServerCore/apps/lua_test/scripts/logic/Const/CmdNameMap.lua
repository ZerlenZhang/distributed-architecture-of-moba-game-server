
local ServiceType = require("ServiceType");

local cmdNameMap={};

cmdNameMap[ServiceType.Logic]={
	UserLostConn = 0,
	LoginLogicReq = 1,
	LoginLogicRes = 2,
	EnterZoneReq = 3,
	EnterZoneRes = 4,
	EnterRoom = 5,
	PlayerEnterRoom = 6,
	PlayerExitRoom = 7,
	ExitRoomReq = 8,
	ExitRoomRes = 9,
	GameStart = 10,
	UdpTest=11,
	LogicFrame = 12,
	NextFrameOpts = 13,
};

return cmdNameMap;