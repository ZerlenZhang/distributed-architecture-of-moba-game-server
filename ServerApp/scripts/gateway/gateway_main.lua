--初始化日志模块
Debug.LogInit("../logs/gateway","gateway",true);

--读取配置文件
local config = require("GameConfig");

--初始化协议
ProtoManager.Init(config.protoType,"C:\\Users\\Administrator\\Documents\\GitHub\\distributed-architecture-of-moba-game-server\\ServerApp\\scripts\\gateway\\const");

local cmdNameMap=require("gateway/const/CmdNameMap");
if cmdNameMap then
    ProtoManager.RegisterCmdMap(cmdNameMap);
else
    Debug.LogError("register cmdNameMap failed");
end



--开启网关
Netbus.TcpListen(config.gateway_tcp_port
    ,function(s)      
            local ip,port = Session.GetAddress(s);
            print("new client come ["..ip..":"..port.."]");
    end);

Debug.Log("Gateway Server [tcp] listen at: ",config.gateway_tcp_port);

--注册服务
local gatewayService = require("gateway/GatewayService");
for k,v in pairs(config.servers) do

    --注意这里每一个服务器都注册了一个网关服务，所以，网关服务的代码会被多个服务器使用和访问
    --需注意可能引发的矛盾冲突

    local ret = Service.RegisterRaw(v.serviceType,gatewayService);
    if ret then
        print("register Gateway Service:[" .. v.descrip .. "] service success");
    else
        print("register Gateway Service:[" .. v.descrip .. "] service failed");
    end
end