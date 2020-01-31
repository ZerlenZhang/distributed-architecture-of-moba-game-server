#include<iostream>
#include<string>
#include "proto/game.pb.h"
#include "../../netbus/protocol/ProtoManager.h"
#include "../../netbus/Netbus.h"
#include "pf_cmd_map.h"
using namespace std;

int main(int argc, char** argv)
{
	ProtoManager::Init();
	InitPfCmdMap();
	Netbus::Instance()->Init();
	Netbus::Instance()->StartTcpServer(6080);
	Netbus::Instance()->StartWebSocketServer(8001);
	Netbus::Instance()->Run();

	system("pause");
	return 0;
}