local config=require("GameConfig");
local mobaConfig = require("MobaConfig");
local mysqlConn=nil;

--链接数据库
local function mysql_connect_to_game_center()

	if mysqlConn then
		return;
	end

	local gameConfig = config.game_mysql;
	Mysql.Connect(
		gameConfig.host,
		gameConfig.port,
		gameConfig.dbName,
		gameConfig.uname,
		gameConfig.upwd,
		function ( err,conn )
			if err then
				Debug.LogError(err);
				Timer.Once(mysql_connect_to_game_center,5000);
				return;
			end
			Debug.Log("connect to mysql [ moba_game ] success");
			mysqlConn=conn;
		end);
end

--获取游戏数据
--handler: err,uinfo
local function get_ugame_info( uid,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql="select ucoin_1,ucoin_2,uexp,uvip,uitem_1,uitem_2,ustatus from ugame where uid=%d limit 1";
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
			ucoin_1= tonumber(result[1]),
			ucoin_2=tonumber(result[2]),
			uexp=tonumber(result[3]),
			uvip=tonumber(result[4]),
			uitem_1=tonumber(result[5]),
			uitem_2=tonumber(result[6]),
			ustatus=tonumber(result[7]),
		};
		if handler then
			handler(nil,uinfo);
		end
	end);
end

--插入游戏数据
--handler: err
local function insert_ugame_info( uid,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end
	local sql="insert into ugame(uid,ucoin_1,ucoin_2,uvip,uexp)values(%d,%d,%d,%d,%d)";
	sql=string.format(sql,uid,mobaConfig.ugame.ucoin_1,mobaConfig.ugame.ucoin_2,mobaConfig.ugame.uvip,mobaConfig.ugame.uexp);
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

--获取登陆奖励信息
--handler: err,bonuesInfo
local function get_bonues_info( uid,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql="select bonues,status,bonues_time,days from login_bonues where uid=%d limit 1";
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
		local bonuesInfo =
		{
			bonues= tonumber(result[1]),
			status=tonumber(result[2]),
			bonues_time=tonumber(result[3]),
			days=tonumber(result[4]),
		};
		if handler then
			handler(nil,bonuesInfo);
		end
	end);
end

--插入登陆奖励
--handler: err
local function insert_bonues_info( uid,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end
	local sql="insert into login_bonues(uid,bonues_time,status)values(%d,%d,1)";
	sql=string.format(sql,uid,Utils.TimeStamp());
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

--更新登陆信息
--handler: err
local function set_bonues_info( uid,uinfo,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end

	local sql = 'update login_bonues set status=0,bonues=%d,bonues_time=%d,days=%d where uid=%d';
	sql=string.format(sql,uinfo.bonues,uinfo.bonues_time,uinfo.days,uid);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				handler(err);
			else
				handler(nil);
			end
		end);
end

--添加货币一
local function add_coin( uid,coin_1,handler )
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end

	local sql = 'update ugame set ucoin_1=ucoin_1+%d where uid=%d';
	sql=string.format(sql,coin_1,uid);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				handler(err);
			else

				handler(nil);
			end
		end);
end

--修改登陆奖励信息
--handle: err
local function update_login_bonues_status( uid,handler )

	if handler ==nil then
		Debug.LogWarning("[update_login_bonues_status( uid,handler )]Handler is nil ??");
		return;
	end
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected");
		end
		return;
	end

	local sql = 'update login_bonues set status=1 where uid=%d';
	sql=string.format(sql,uid);
	Mysql.Query(mysqlConn,sql,
		function( err,ret )
			if err then
				if handler then
					handler(err);
				end
			else
				if handler then
					handler(nil);
				end
			end
		end);
end

--获取机器人信息
--handle: err,ret
local function get_robot_ugame_info(handler)
	if mysqlConn==nil then
		--数据库还没有联好
		if handler then
			handler("mysql is not connected",nil);
		end
		return;
	end
	local sql="select ucoin_1,ucoin_2,uexp,uvip,uitem_1,uitem_2,ustatus,uid from ugame where is_robot=1";
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
--			print("no data found");
			if handler then
				handler(nil,nil);
			end
			return;
		end
		--查到数据

		local robots={};

		for k, result in pairs(ret) do
			local uinfo =
			{
				ucoin_1= tonumber(result[1]),
				ucoin_2=tonumber(result[2]),
				uexp=tonumber(result[3]),
				uvip=tonumber(result[4]),
				uitem_1=tonumber(result[5]),
				uitem_2=tonumber(result[6]),
				ustatus=tonumber(result[7]),
				uid=tonumber(result[8]),
			};
			table.insert(robots,uinfo);
		end

		if handler then
--			print("Get data :",#robots);
			handler(nil,robots);
		end
	end);
end

return {
	Connect=mysql_connect_to_game_center,
	GetUgameInfo=get_ugame_info,
	GetBonuesInfo=get_bonues_info,
	InsertBonuesInfo=insert_bonues_info,
	SetBonuesInfo=set_bonues_info,
	InsertUgameInfo=insert_ugame_info,
	UpdateLoginBonuesStatus=update_login_bonues_status,
	AddCoin1=add_coin,
	IsConnect=function() return mysqlConn~=nil end,
	GetRobotUgameInfo=get_robot_ugame_info,
}