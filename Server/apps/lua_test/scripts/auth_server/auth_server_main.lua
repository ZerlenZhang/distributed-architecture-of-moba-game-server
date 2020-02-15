--初始化日志模块
Debug.LogInit("logger/auth","auth",true);
--初始化协议模块
local ProtoType={
    Json=0,
    Protobuf=1,
};
ProtoManager.Init(ProtoType.Protobuf,"F:\\Projects\\unity\\Moba\\Server\\build\\proj_win32\\x64\\Debug\\protos");
--如果是Protobuf协议，还需要注册映射表
if ProtoManager.ProtoType()==ProtoType.Protobuf then
    local cmdNameMap=require("CmdNameMap");
    if cmdNameMap then
        ProtoManager.RegisterCmdMap(cmdNameMap);
    end
end

--读取配置文件
local config = require("GameConfig");
local sType=require("ServiceType");

local servers=config.servers;

--开启服务器
Netbus.TcpListen(servers[sType.Auth].port);
print("Auth server [tcp] listen at: ",servers[sType.Auth].port);

--注册服务
local authService = require("auth_server/AuthService");
local ret = Service.Register(sType.Auth,authService);
if ret then
    print("register Auth Service:[" .. sType.Auth .. "] service success");
else
    print("register Auth Service:[" .. sType.Auth .. "] service failed");
end