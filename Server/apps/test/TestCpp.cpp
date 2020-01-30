#include<iostream>
#include<string>
#include "../../build/proj_win32/Netbus.h"
using namespace std;

int main(int argc, char** argv)
{
	Netbus::Instance()->Init();
	Netbus::Instance()->StartTcpServer(6080);
	Netbus::Instance()->StartWebSocketServer(8001);
	Netbus::Instance()->Run();

	system("pause");
	return 0;
}