#include<iostream>
#include<string>
#include "../../netbus/Netbus.h"
#include "../../utils/timer/time_list.h"
#include "../../utils/logger/logger.h"
#include "../../utils/timestamp/timestamp.h"
#include "../../database/mysqlwarpper.h"
#include "../../database/rediswarpper.h"


#include "proto/game.pb.h"
#include "pf_cmd_map.h"
using namespace std; 

#pragma region 回调


static void on_redis_query(const char* err, RedisReply* result)
{
	if (err)
	{
		log_debug("RedisError: %s", err);
		return;
	}

	log_debug("RedisQuery: %s", result->reply->str);
}

static void on_mysql_connect(const char* error,MysqlContext* context)
{
	if (error)
	{
		log_debug("Database error:\t%s",error);
		return;
	}
	log_debug("mysql connect successful");

	mysql_wrapper::query(
		(MysqlContext*)context, 
		(char*)"update test_class set name = 'hello' where id = 1");

	mysql_wrapper::close((MysqlContext*)context);
}

static void on_redis_connect(const char* err, RedisContext* context)
{
	if (err)
	{
		log_debug("RedisError: %s", err);
		return;
	}
	log_debug("redis connect success");
	redis_wrapper::query(context, (char*)"select 12", on_redis_query);
	//redis_wrapper::close_redis((RedisContext *) context);
}

#pragma endregion

#pragma region 模块测试

static void TestTimer(void* data)
{
	log_debug("当前时间戳 %d", timestamp());
	log_debug("今天零时时间戳 %d", timestamp_today());
	log_debug("时间字符串转时间戳: %d", date2timestamp("%Y-%m-%d %H:%M:%S", "2020-02-01 00:00:00"));

	static char outBuf[64];
	timestamp2date(timestamp_today(), "%Y-%m-%d %H:%M:%S", outBuf, sizeof(outBuf));
	log_debug(outBuf);
}

static void TestMySql()
{
	mysql_wrapper::connect(
		(char*)"127.0.0.1",
		3306, 
		(char*)"test_mysql", 
		(char*)"root", 
		(char*)"Zzl5201314...", 
		on_mysql_connect);
}

static void TestRedis()
{
	redis_wrapper::connect(
		(char*)"127.0.0.1",
		7999,
		on_redis_connect);

}
#pragma endregion



int main(int argc, char** argv)
{
	InitPfCmdMap();
	Netbus::Instance()->Init();
	logger::init("logger", "netbus_log", true);

	//TestMySql();
	//TestRedis();
	//schedule(TestTimer, NULL, 3000, 4);


	Netbus::Instance()->TcpListen(6080);
	Netbus::Instance()->WebSocketListen(8001);
	Netbus::Instance()->UdpListen(8002);

	Netbus::Instance()->Run();

	system("pause");
	return 0;
}