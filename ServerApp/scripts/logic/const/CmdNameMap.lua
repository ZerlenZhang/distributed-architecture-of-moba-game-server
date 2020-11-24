local ServiceType = require("ServiceType");

local cmdNameMap={};
cmdNameMap[ServiceType.Logic]={
    LoginLogicReq = 1,
    LoginLogicRes = 2,
    UdpTestReq = 3,
    UdpTestRes = 4,
};
return cmdNameMap;