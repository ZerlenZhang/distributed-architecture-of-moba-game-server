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
			Debug.Log("connect to mysql [ auth_center_db ] success");
			mysqlConn=conn;
		end);
end

--根据guest_key获取uinfo
--handler:err,uinfo
function get_uinfo_by_key( key,handler )
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

--根据uid获取用户信息
function get_uinfo_by_uid( uid,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql="select uid,unick,usex,uface,uvip,status,is_guest from uinfo where uid=\"%d\" limit 1";
	sql=string.format(sql,uid);
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

--插入guest_key
--handler: err
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

--修改用户信息
--handler: err
function edit_profile(uid,unick,uface,usex,handler)
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end

	local sql = 'update uinfo set unick="%s",usex=%d,uface=%d where uid=%d';
	sql=string.format(sql,unick,usex,uface,uid);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				handler(err);
			else
				handler(nil);
			end
		end);
end

--判断用户名是否存在
--handler: err,foundUid
function check_uname_exist(	uname,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql = 'select uid from uinfo where uname="%s"';
	sql=string.format(sql,uname);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				if handler then
					handler(err,nil);
				end
				return;
			end
			if ret==nil or #ret<=0 then
				if handler then
					handler(err,nil);
				end
				return;
			end
			if handler then
				local result = ret[1];
				handler(err,result);
			end
		end);
end

--升级游客账户
--handler:err
function guest_account_upgrade( uid,uname,upwd,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end

	local sql = 'update uinfo set uname="%s",upwd="%s",is_guest=0 where uid=%d';
	sql=string.format(sql,uname,upwd,uid);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				if handler then
					handler(err);
				end
				return;
			end

			if ret==nil or #ret<=0 then
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


--根据用户名密码查找用户信息
--handler:err,uinfo
function get_uinfo_by_uname_pwd( uname,upwd,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end

	local sql="select uid,unick,usex,uface,uvip,status,is_guest from uinfo where uname=\"%s\" and upwd=\"%s\" limit 1";
	sql=string.format(sql,uname,upwd);
	--print(sql);
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

return {
	Connect=mysql_connect_to_auth_center,
	GetUinfoByKey=get_uinfo_by_key,
	GetUinfoByUid=get_uinfo_by_uid,
	GetUinfoByUnamePwd=get_uinfo_by_uname_pwd,
	InsertGuestUser=insert_guest_user,
	EditProfile=edit_profile,
	CheckUnameExist=check_uname_exist,
	GuestAccountUpgrade =guest_account_upgrade,
};