--
-- Created by IntelliJ IDEA.
-- User: ReadyGamerOne
-- Date: 2020/3/1
-- Time: 11:16
-- To change this template use File | Settings | File Templates.
--
local ServiceType = require("ServiceType");

local cmdNameMap={};
cmdNameMap[ServiceType.Logic]={
	LoginLogicReq = 1,
};

cmdNameMap[ServiceType.Auth]={
	UserLostConn = 0,
	ReLogin = 1,
	UserLoginReq = 2,
	UserLoginRes = 3,
	UserUnregisterRes = 4,
	UserRegisteReq = 8,
	UserRegisteRes = 9,
}

return cmdNameMap;

