--初始化日志模块
Debug.LogInit("../logs/gateway","gateway",true);

--读取配置文件
local config = require("GameConfig");

--开启网关
Netbus.TcpListen(config.gateway_tcp_port
    ,function(s)      
            local ip,port = Session.GetAddress(s);
            print("new client come ["..ip..":"..port.."]");
    end);

print("Gateway Server [tcp] listen at: ",config.gateway_tcp_port);