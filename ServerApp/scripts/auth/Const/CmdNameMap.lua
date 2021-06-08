

local ServiceType = require("ServiceType");

local cmdNameMap={};
cmdNameMap[ServiceType.Auth]={
	UserLostConn = 0,
	ReLogin = 1,
	UserLoginReq = 2,
	UserLoginRes = 3,
	UserUnregisterRes = 4,
	UserUnregisterReq = 5,
	EditProfileReq = 6,
	EditProfileRes = 7,
	UserRegisteReq = 8,
	UserRegisteRes = 9,
};

return cmdNameMap;