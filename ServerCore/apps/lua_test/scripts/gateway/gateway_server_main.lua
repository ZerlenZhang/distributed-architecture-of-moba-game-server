--初始化日志模块
Debug.LogInit("logger/gateway","gateway",true);
--初始化协议模块
local ProtoType={
    Json=0,
    Protobuf=1,
};
Debug.Log("hello");
ProtoManager.Init(ProtoType.Protobuf,"F:\\Projects\\unity\\Moba\\Server\\apps\\lua_test\\scripts\\gateway\\Const");
--如果是Protobuf协议，还需要注册映射表
if ProtoManager.ProtoType()==ProtoType.Protobuf then
    local cmdNameMap=require("gateway/Const/CmdNameMap");
    if cmdNameMap then
        ProtoManager.RegisterCmdMap(cmdNameMap);
    else
        Debug.LogError("register cmdNameMap failed");
    end
end
--读取配置文件
local config = require("GameConfig");

--开启网关
Netbus.TcpListen(config.gateway_tcp_port
    ,function(s)      
            local ip,port = Session.GetAddress(s);
            print("new client come ["..ip..":"..port.."]");
    end);
print("Gateway Server [tcp] listen at: ",config.gateway_tcp_port);

--注册服务
local servers=config.servers;
local gatewayService = require("gateway/GatewayService");
for k,v in pairs(servers) do

    --注意这里每一个服务器都注册了一个网关服务，所以，网关服务的代码会被多个服务器使用和访问
    --需注意可能引发的矛盾冲突

    local ret = Service.RegisterRaw(v.serviceType,gatewayService);
    if ret then
        print("register Gateway Service:[" .. v.serviceType .. "] service success");
    else
        print("register Gateway Service:[" .. v.serviceType .. "] service failed");
    end
end