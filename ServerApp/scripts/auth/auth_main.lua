--初始化日志模块
Debug.LogInit("../logs/auth","auth",true);

--读取配置文件
local sType = require("ServiceType");
local config = require("GameConfig");

--初始化协议
ProtoManager.Init(config.protoType,"C:\\Users\\Administrator\\Documents\\GitHub\\distributed-architecture-of-moba-game-server\\ServerApp\\scripts\\auth\\const");

--如果是Protobuf协议，还需要注册映射表
local cmdNameMap=require("auth/const/CmdNameMap");
if cmdNameMap then
    ProtoManager.RegisterCmdMap(cmdNameMap);
else
    Debug.LogError("Protobuf register cmdNameMap failed");
end



local authServerConfig=config.servers[sType.Auth];

--开启服务器
Netbus.TcpListen(authServerConfig.port
    ,function(s)
        local ip,port = Session.GetAddress(s);
        print("new client come ["..ip..":"..port.."]");
    end);

Debug.Log("Auth server [tcp] listen at: ", authServerConfig.port);

--注册服务
local authService = require("auth/AuthService");
local ret = Service.Register(sType.Auth,authService);
if ret then
    print("register Auth Service:[" .. sType.Auth .. "] service success");
else
    print("register Auth Service:[" .. sType.Auth .. "] service failed");
end