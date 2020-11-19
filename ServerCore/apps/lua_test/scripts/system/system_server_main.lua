--初始化日志模块
Debug.LogInit("logger/system","system",true);
--初始化协议模块
local ProtoType={
    Json=0,
    Protobuf=1,
};
ProtoManager.Init(ProtoType.Protobuf,"F:\\Projects\\unity\\Moba\\Server\\apps\\lua_test\\scripts\\system\\Const");
--如果是Protobuf协议，还需要注册映射表
if ProtoManager.ProtoType()==ProtoType.Protobuf then
    local cmdNameMap=require("system/Const/CmdNameMap");
    if cmdNameMap then
        ProtoManager.RegisterCmdMap(cmdNameMap);
    else
    	Debug.LogError("register cmdNameMap failed");
    end
end

--读取配置文件
local config = require("GameConfig");
local sType = require("ServiceType");

local servers=config.servers;

--开启服务器
Netbus.TcpListen(servers[sType.System].port
    ,function(s)
        local ip,port = Session.GetAddress(s);
        print("new client come ["..ip..":"..port.."]");
    end);

print("System server [tcp] listen at: ",servers[sType.System].port);

--注册服务
local systemService = require("system/SystemService");
local ret = Service.Register(sType.System,systemService);
if ret then
    print("register System Service:[" .. sType.System .. "] service success");
else
    print("register System Service:[" .. sType.System .. "] service failed");
end
