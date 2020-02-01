#include<iostream>
#include<string>
#include "proto/game.pb.h"
#include "../../netbus/Netbus.h"
#include "../../utils/timer/time_list.h"
#include "../../utils/logger/logger.h"
#include "../../utils/timestamp/timestamp.h"
using namespace std; 

static void TestTimer(void* data)
{
	log_debug("hello", "???", "!!!");
}

int main(int argc, char** argv)
{
	Netbus::Instance()->Init();
	//schedule(TestTimer, NULL, 3000, 4);
	log_debug("当前时间戳 %d", timestamp());
	log_debug("今天零时时间戳 %d", timestamp_today());
	log_debug("时间字符串转时间戳: %d", date2timestamp("%Y-%m-%d %H:%M:%S", "2020-02-01 00:00:00"));

	char outBuf[64];
	timestamp2date(timestamp_today(), "%Y-%m-%d %H:%M:%S", outBuf, sizeof(outBuf));
	log_debug(outBuf);

	Netbus::Instance()->StartTcpServer(6080);
	Netbus::Instance()->StartWebSocketServer(8001);
	Netbus::Instance()->Run();

	 
	system("pause");
	return 0;
}