#include<iostream>
#include<string>
using namespace std; 

#include "../../lua_wrapper/lua_wrapper.h"
#include "../../netbus/Netbus.h"
#include "../../utils/logger/logger.h"
#include "../../utils/win32/WinUtil.h"
#include "../../utils/Encoding.h"

static string exeFilePath;
static string protoFileDir;
const string& GetProtoDir()
{
	return protoFileDir;
}
const string& GetExeFilePath()
{
	return exeFilePath;
}

int main(int argc, char** argv)
{
	exeFilePath = argv[0];

	//logger::init("logger/Main", "Main", true);

	Netbus::Instance()->Init();
	lua_wrapper::Init();

	//启动
	if (argc == 2)
	{
		protoFileDir = WinUtil::GetDirPath(argv[0]);
		string searchPath = "../../apps/lua_test/scripts/";
		lua_wrapper::AddSearchPath(searchPath);
		lua_wrapper::DoFile(searchPath+argv[1]);
	}
	else if(argc == 3)
	{
		protoFileDir = WinUtil::GetDirPath(argv[0]);
		string searchPath = argv[1];
		if (*(searchPath.end() - 1) != '/')
		{
			searchPath += "/";		
		}
		lua_wrapper::AddSearchPath(argv[1]);
		lua_wrapper::DoFile(searchPath + argv[2]);
	}
	else {
		protoFileDir = WinUtil::GetDirPath(argv[0]);
		string searchPath = "../../apps/lua_test/scripts/";
		lua_wrapper::AddSearchPath(searchPath);
		lua_wrapper::DoFile(searchPath + "logic/logic_server_main.lua");
	}


	//运行
	Netbus::Instance()->Run();



	//退出
	lua_wrapper::Exit();
	log_debug("服务器退出");
	system("pause");
	return 0;
}