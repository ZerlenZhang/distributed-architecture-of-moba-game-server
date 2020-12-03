--初始化日志模块
Debug.LogInit("../logs/logic","logic",true);

--读取配置文件
local sType = require("ServiceType");
local config = require("GameConfig");

--初始化协议
ProtoManager.Init(config.protoType,"C:\\Users\\Administrator\\Documents\\GitHub\\distributed-architecture-of-moba-game-server\\ServerApp\\scripts\\logic\\const");

--如果是Protobuf协议，还需要注册映射表
local cmdNameMap=require("logic/const/CmdNameMap");
if cmdNameMap then
    ProtoManager.RegisterCmdMap(cmdNameMap);
else
    Debug.LogError("Protobuf register cmdNameMap failed");
end



local logicServerConfig=config.servers[sType.Logic];
local logicConfig=require("logic/LogicConfig");

--开启服务器
Netbus.TcpListen(logicServerConfig.port
    ,function(s)
        local ip,port = Session.GetAddress(s);
        print("new client come ["..ip..":"..port.."]");
    end);

print("Logic server [tcp] listen at: ", logicServerConfig.port);

Netbus.UdpListen(logicConfig.udp_port);
print("Logic server [udp] listen at: ", logicConfig.udp_port);


--注册服务
local logicService = require("logic/LogicService");
local ret = Service.Register(sType.Logic,logicService);
if ret then
    print("register Logic Service:[" .. sType.Logic .. "] service success");
else
    Debug.LogError("register Logic Service:[" .. sType.Logic .. "] service failed");
end