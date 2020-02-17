local config=require("GameConfig");
local mysqlConn=nil;

--链接数据库
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

function get_guest_user( key,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql="select uid,unick,usex,uface,uvip,status,is_guest from uinfo where guest_key=\"%s\" limit 1";
	sql=string.format(sql,key);
	Mysql.Query(mysqlConn,sql,function( err,ret )
		--出现错误
		if err then
			if handler then
				handler(err,nil);
			end
			return;
		end
		--没有查到数据
		if ret==nil or #ret <=0 then
			if handler then
				handler(nil,nil);
			end
			return;
		end
		--查到数据
		local result = ret[1];
		local uinfo =
		{
			uid= tonumber(result[1]),
			unick=result[2],
			usex=tonumber(result[3]),
			uface=tonumber(result[4]),
			uvip=tonumber(result[5]),
			status=tonumber(result[6]),
			is_guest=tonumber(result[7]),
		};
		if handler then
			handler(nil,uinfo);
		end
	end);
end

function insert_guest_user( key,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end
	local sql="insert into uinfo(guest_key,unick,uface,usex,is_guest)values(\"%s\",\"%s\",%d,%d,1)";
	local unick = "gst"..math.random(100000,999999);
	local uface = math.random(1,9);
	local usex = math.random(0,1);
	sql=string.format(sql,key,unick,uface,usex);
	Mysql.Query(mysqlConn,sql,function( err,ret )
		--出现错误
		if err then
			if handler then
				handler(err);
			end
			return;
		end
		if handler then
			handler(nil);
		end
	end);
end

mysql_connect_to_auth_center();

return {
	GetGuestUinfo=get_guest_user,
	InsertGuestUser=insert_guest_user,
};