local config=require("GameConfig");
local mysqlConn=nil;

--链接数据库
local function mysql_connect_to_auth_center()

	if mysqlConn then
		return;
	end

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
local function get_uinfo_by_key( key,handler )
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
local function get_uinfo_by_uid( uid,handler )
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
local function insert_guest_user( key,handler )
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
local function edit_profile(uid,unick,uface,usex,handler)
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
local function check_uname_exist(	uname,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql = 'select uid from usertable where uname="%s"';
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


--根据用户名密码查找用户信息
--handler:err,uinfo
local function get_uinfo_by_uname_pwd( uname,upwd,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end

	local sql="select uid,uname,pwd,unick,ulevel,uexp,urank,ucoin,udiamond,usignature,uintegrity,status,uface,heros,urankExp from usertable where uname=\"%s\" and pwd=\"%s\" limit 1";
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
			uname=result[2],
			pwd=result[3],
			unick=result[4],
			ulevel=tonumber(result[5]),
			uexp=tonumber(result[6]),
			urank=tonumber(result[7]),
			ucoin=tonumber(result[8]),
			udiamond=tonumber(result[9]),
			usignature=result[10],
			uintegrity=tonumber(result[11]),
			status=tonumber(result[12]),
			uface=tonumber(result[13]),
			heros=result[14],
			urankExp=tonumber(result[15]),
		};
		if handler then
			handler(nil,uinfo);
		end
	end);
end

--根据用户名获取匹配信息
--handler:err,matcherinfo
local function get_matcherinfo_by_uname(uname,handler)

	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end

	local sql="select unick,urank,uface,ulevel from usertable where uname=\"%s\" limit 1";
	sql=string.format(sql,uname);

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
		local matchInfo =
		{
			unick=result[1],
			urank=tonumber(result[2]),
			uface=tonumber(result[3]),
			ulevel=tonumber(result[4]),
		};
		if handler then
			handler(nil,matchInfo);
		end
	end);
end

--根据用户名获取默认英雄
local function get_default_heroId_by_uname(uname,handler)
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end

	local sql="select heros from usertable where uname=\"%s\" limit 1";
	sql=string.format(sql,uname);
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
		local  startIndex, endIndex = string.find(result[1],",",1);
		local defaultHeroId=tonumber(string.sub(result[1],1,startIndex));
		if handler then
			handler(nil,defaultHeroId);
		end
	end);
end

--根据用户名，密码，昵称注册
local function registe(uname,pwd,unick,handler)
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end
	local sql="insert into usertable(uname,pwd,unick)values(\"%s\",\"%s\",\"%s\")";
	sql=string.format(sql,uname,pwd,unick);
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

--根据用户名获取仓库信息
--handler(err,{ulevel,uexp,urank,urankExp,ucoin,udiamond})
local function get_package_info_by_uname(uname,handler)
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end

	local sql="select ulevel,uexp,urank,urankExp,ucoin,udiamond from usertable where uname=\"%s\" limit 1";
	sql=string.format(sql,uname);
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
		local packageInfo =
		{
			ulevel=tonumber(result[1]),
			uexp=tonumber(result[2]),
			urank=tonumber(result[3]),
			urankExp=tonumber(result[4]),
			ucoin=tonumber(result[5]),
			udiamond=tonumber(result[6]),
		};
		if handler then
			handler(nil,packageInfo);
		end
	end);
end

--根据uname更新仓库信息
--handler(err,{ulevel,uexp,urank,urankExp,ucoin,udiamond})
local function update_package_info_by_uname(uname,packageInfo,handler)
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end

	local sql = 'update usertable set ulevel=%d,uexp=%d,urank=%d,urankExp=%d,ucoin=%d,udiamond=%d where uname=\"%s\"';
	sql=string.format(sql,packageInfo.ulevel,packageInfo.uexp,packageInfo.urank,packageInfo.urankExp,packageInfo.ucoin,
		packageInfo.udiamond,uname);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				handler(err);
			else
				handler(nil);
			end
		end);
end

return {
	Connect=mysql_connect_to_auth_center,
	GetUinfoByKey=get_uinfo_by_key,
	GetUinfoByUid=get_uinfo_by_uid,
	GetUinfoByUnamePwd=get_uinfo_by_uname_pwd,
	GetMatcherInfoByUname=get_matcherinfo_by_uname,
	GetDefaultHeroIdByUname=get_default_heroId_by_uname,
	EditProfile=edit_profile,
	CheckUnameExist=check_uname_exist,
	IsConnect=function() return mysqlConn~=nil end,
	Registe=registe,
	GetPackageInfoByUname=get_package_info_by_uname,
	UpdatePackageInfoByUname=update_package_info_by_uname,
};