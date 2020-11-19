
Debug.LogInit("../logs/debug","debug",true);
Debug.Log("hello");
-- local ip="121.196.178.141";
-- local port=6080;
-- Netbus.TcpConnect(ip,port,
-- 		--链接成功的回调
--         function(session)
--             if session then
--                 print("succeed to connect to ["..ip..":"..port.."]");
--                 return;
--             end
--             print("failed to connect to ["..ip..":"..port.."]")
--         end);
redis=require("database/redis_center");
redis.Connect();
mysql=require("database/mysql_center");
mysql.Connect();
