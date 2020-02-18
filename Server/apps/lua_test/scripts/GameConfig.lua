local sType = require("ServiceType");

local remote_servers={};

--注册服务所部署的IP地址和端口
remote_servers[sType.Auth]={
    serviceType = sType.Auth,
    ip = "127.0.0.1",
    port = 8000,
    descrip = "Auth Server",
};

return {
    gateway_tcp_ip = "127.0.0.1",
    gateway_tcp_port = 6080,

    gateway_ws_ip = "127.0.0.1",
    gateway_ws_port = 8001,

    servers = remote_servers,

    auth_mysql={
    	host="127.0.0.1",
    	port=3306,
    	dbName="auth_center_db",
    	uname="root",
    	upwd="Zzl5201314...",
	},

    center_redis={
        host="127.0.0.1",
        port=7999,
        db_index=1,
        auth="Zzl5201314..."
    },

};