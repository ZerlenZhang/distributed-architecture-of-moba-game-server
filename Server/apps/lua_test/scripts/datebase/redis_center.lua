local config=require("GameConfig");
local conn=nil;

local function redis_connect_to_center()

	if conn then
		return;
	end


	local host = config.center_redis.host;
	local port = config.center_redis.port;
	local index = config.center_redis.db_index;

	Redis.Connect(host,port,
		function( err,redisConn )
			--链接错误，重连
			if err~=nil then
				Debug.LogError(err);
				Timer.Once(redis_connect_to_center,5000);
				return;
			end
			--连接成功
			conn=redisConn;
			Redis.Query(conn,"select "..index,
				function( err,ret )
					if err then
						Debug.LogError(err);
						return;
					end
					print("connect to redis [ "..index.." ] success");
				end)
		end);
end

local function set_uinfo( uid,uinfo )
	if nil==conn then
		Debug.LogError("redis is not connected yet");
		return;
	end

	local cmd = "hmset moba_auth_center_user_uid_"..uid..
				" unick "..uinfo.unick..
				" usex ".. uinfo.usex..
				" uface "..uinfo.uface..
				" uvip ".. uinfo.uvip..
				" is_guest "..uinfo.is_guest;
	Redis.Query(conn,cmd,
		function ( err,ret )
			if err then
				Debug.LogError(err);
				return;
			end
			--print("[Redis]",ret);
		end);
end

--handler: err,uinfo
local function get_uinfo( uid,handler )
	if nil==conn then
		Debug.LogError("redis is not connected yet");
		return;
	end
	local cmd = "hgetall moba_auth_center_user_uid_"..uid;
	Redis.Query(conn,cmd,
		function ( err,ret )
			if err then
				Debug.LogError(err);
				if handler then
					handler(err,nil);
				end
				return;
			end
			local uinfo = {};
			uinfo.uid=uid;
			uinfo.unick=ret[2];
			uinfo.uface=tonumber(ret[4]);
			uinfo.uvip=tonumber(ret[6]);
			uinfo.usex=tonumber(ret[8]);
			uinfo.is_guest=tonumber(ret[10]);
			handler(nil,uinfo);
		end);
end

local function edit_profile( uid,unick,usex,uface )
	get_uinfo(uid,
		function ( err,uinfo )
			if err then
				Debug.LogError("redis get error");
				return;
			end
			uinfo.unick=unick;
			uinfo.usex=usex;
			uinfo.uface=uface;

			set_uinfo(uid,uinfo);
		end)
end


return {
	Connect=redis_connect_to_center,
	GetUinfo=get_uinfo,
	SetUinfo=set_uinfo,
	EditProfile=edit_profile,
	IsConnect=function() return conn~=nil end
};