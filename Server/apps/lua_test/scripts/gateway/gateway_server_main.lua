--初始化日志模块
Debug.LogInit("logger/gateway","gateway",true);
--初始化协议模块
local ProtoType={
    Json=0,
    Protobuf=1,
};
ProtoManager.Init(ProtoType.Protobuf);
--如果是Protobuf协议，还需要注册映射表
if ProtoManager.ProtoType()==ProtoType.Protobuf then
    local cmdNameMap=require("CmdNameMap");
    if cmdNameMap then
        ProtoManager.RegisterCmdMap(cmdNameMap);
    end
end

--读取配置文件
local config = require("GameConfig.lua");

--开启网关
Netbus.TcpListen(config.gateway_tcp_port);

--注册服务
local servers=config.servers;
local gatewayService = require("gateway/gw_services.lua");
for k,v in pairs(servers) do
    local ret = Service.Register(v.serviceType,gatewayService);
    if ret then
        print("register gw_service:[" .. v.serviceType .. "] service success");
    else
        print("register gw_service:[" .. v.serviceType .. "] service failed");
    end
end
