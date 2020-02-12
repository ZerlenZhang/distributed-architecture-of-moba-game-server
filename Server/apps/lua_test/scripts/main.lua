--初始化日志模块
Debug.LogInit("logger/TestLua","gateway",true);
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

--开启网络服务
Netbus.TcpListen(6080);
Netbus.WebsocketListen(8001);
Netbus.UdpListen(8002);

print("start all service success");