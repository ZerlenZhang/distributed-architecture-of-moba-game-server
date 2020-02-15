--初始化日志模块
Debug.LogInit("logger/gateway","gateway",true);
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

--开启网关
Netbus.TcpListen(config.gateway_tcp_port);
print("Gateway Server [tcp] listen at: ",config.gateway_tcp_port);

--注册服务
local servers=config.servers;
local gatewayService = require("gateway/GatewayService");
for k,v in pairs(servers) do
    local ret = Service.RegisterRaw(v.serviceType,gatewayService);
    if ret then
        print("register Gateway Service:[" .. v.serviceType .. "] service success");
    else
        print("register Gateway Service:[" .. v.serviceType .. "] service failed");
    end
end