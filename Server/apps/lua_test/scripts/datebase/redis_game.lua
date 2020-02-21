local config=require("GameConfig");
local conn=nil;

function redis_connect_to_game()
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

return {
	Connect=redis_connect_to_game,
};