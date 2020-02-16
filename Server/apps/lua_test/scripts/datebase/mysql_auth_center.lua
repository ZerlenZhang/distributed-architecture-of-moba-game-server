local config=require("GameConfig");

local mysqlConn=nil;
function mysql_connect_to_auth_center()
	local authConfig = config.auth_mysql;
	Mysql.Connect(
		authConfig.host,
		authConfig.port,
		authConfig.dbName,
		authConfig.uname,
		authConfig.upwd,
		function ( err,conn )
			if err then
				Debug.LogError(err);
				Timer.Once(mysql_connect_to_auth_center,5000);
				return;
			end
			Debug.Log("connect to auth_center_db success");
			mysqlConn=conn;
		end);
end

mysql_connect_to_auth_center();