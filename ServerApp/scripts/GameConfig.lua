local ServiceType = require("ServiceType");
local ProtoType=require("ProtoType");


local remote_servers={};

--注册服务所部署的IP地址和端口
remote_servers[ServiceType.Auth]={
    serviceType = ServiceType.Auth,
    ip = "127.0.0.1",
    port = 6081,
    descrip = "Auth Server",
};

-- remote_servers[ServiceType.System]={
--     serviceType = ServiceType.System,
--     ip = "127.0.0.1",
--     port = 8001,
--     descrip = "System Server",
-- };

remote_servers[ServiceType.Logic]={
    serviceType = ServiceType.Logic,
    ip = "127.0.0.1",
    port = 6082,
    descrip = "Logic Server",
    udp_port=10000,
};



return {

    protoType=ProtoType.Protobuf;

    gateway_tcp_ip = "121.196.178.141",
    gateway_tcp_port = 6080,

    enable_gateway_log=true;


    servers = remote_servers,

    auth_mysql={
        host="127.0.0.1",
        port=3306,
        dbName="PurificationPioneer",
        uname="root",
        upwd="Zzl5201314...",
	},

    game_mysql={
        host="127.0.0.1",
        port=3306,
        dbName="moba_game",
        uname="root",
        upwd="Zzl5201314...",
    },

    center_redis={
        host="127.0.0.1",
        port=6379,
        db_index=1,
        auth="Zzl5201314...",
    },

    game_redis={
        host="127.0.0.1",
        port=6379,
        db_index=2,
        auth="Zzl5201314...",
    },
    logic_udp={
        host="127.0.0.1",
        port=8800,
    },
};