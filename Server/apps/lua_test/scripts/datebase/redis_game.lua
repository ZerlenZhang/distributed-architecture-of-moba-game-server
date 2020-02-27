local config=require("GameConfig");
local conn=nil;

local function redis_connect_to_game()


	if conn then
		return;
	end

	local host = config.game_redis.host;
	local port = config.game_redis.port;
	local index = config.game_redis.db_index;

	Redis.Connect(host,port,
		function( err,redisConn )
			--链接错误，重连
			if err~=nil then
				Debug.LogError(err);
				Timer.Once(redis_connect_to_game,5000);
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

--设置用户游戏信息
local function set_ugame_info(uid,ugameinfo,handler)

--	print("flag 1");
	if conn==nil then
		--数据库还没有联好
		Debug.LogError("redis is not connected");
		return;
	end
	local cmd = "hmset moba_ugame_user_uid_"..uid..
			" ucoin_1 "..ugameinfo.ucoin_1..
			" uexp "..ugameinfo.uexp..
			" uvip ".. ugameinfo.uvip;
	Redis.Query(conn,cmd,
		function ( err,ret )
			if err then
				Debug.LogError(err);
				return;
			end
--			print("[Redis]",ret);
			if handler then
				handler();
			end
		end);
end

--获取用户游戏信息
--handler: err,ginfo
local function get_ugame_info(uid,handler)
	if nil==conn then
		Debug.LogError("redis is not connected yet");
		return;
	end
	local cmd = "hgetall moba_ugame_user_uid_"..uid;
	Redis.Query(conn,cmd,
		function ( err,ret )
			if err then
				Debug.LogError(err);
				if handler then
					handler(err,nil);
				end
				return;
			end
			local ginfo = {};
			ginfo.uid=uid;
			ginfo.ucoin_1=tonumber(ret[2]);
			ginfo.uexp=tonumber(ret[4]);
			ginfo.uvip=tonumber(ret[6]);
			handler(nil,ginfo);
		end);
end

--添加货币1
local function add_coin_1(uid,add_coin)
	get_ugame_info(uid,
		function(err,ginfo)
			if err then
				Debug.LogError(err);
				return;
			end
			ginfo.ucoin_1 = ginfo.ucoin_1 + add_coin;

			set_ugame_info(uid,ginfo);
		end);
end

return {
	Connect=redis_connect_to_game,
	IsConnect=function() return conn~=nil end,
	GetUgameInfo=get_ugame_info,
	SetUgameInfo=set_ugame_info,
	AddCoin1=add_coin_1,
};