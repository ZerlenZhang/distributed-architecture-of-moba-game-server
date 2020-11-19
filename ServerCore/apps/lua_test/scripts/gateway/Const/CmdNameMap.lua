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
	GuestLoginReq = 1,
	GuestLoginRes = 2,
	ReLogin = 3,
	UserLoginReq = 9,
	UserLoginRes = 10,
	UserUnregisterRes = 12,
}

return cmdNameMap;

