

local ServiceType = require("ServiceType");

local cmdNameMap={};
cmdNameMap[ServiceType.Auth]={
	UserLostConn = 0,
	GuestLoginReq = 1,
	GuestLoginRes = 2,
	ReLogin = 3,
	EditProfileReq = 5,
	EditProfileRes = 6,
	AccountUpgradeReq = 7,
	AccountUpgradeRes = 8,
	UserLoginReq = 9,
	UserLoginRes = 10,
	UserUnregisterReq = 11,
	UserUnregisterRes = 12,
};

return cmdNameMap;