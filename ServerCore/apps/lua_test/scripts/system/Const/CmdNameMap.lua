local ServiceType = require("ServiceType");

local cmdNameMap={};
cmdNameMap[ServiceType.System]={
	UserLostConn = 0,
	GetMobaInfoReq=1,
	GetMobaInfoRes=2,
	RecvLoginBonuesReq = 3,
	RecvLoginBonuesRes = 4,
};

return cmdNameMap;