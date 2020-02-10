#ifndef __LUA_WRAPPER_H__
#define __LUA_WRAPPER_H__

#include <lua.hpp>

class lua_wrapper
{
public:
	static void Init();
	static void Exit();

	static lua_State* lua_state();

	//执行lua文件
	static bool ExeLuaFile(char* luaFilePath);
	//调用脚本函数
	static int ExeScriptHandle(int handle, int numArgs);
	//移除脚本函数
	static void RemoveScriptHandle(int handle);


private:
	//导出C/C++函数
	static void ExportFunc2Lua(
		const char* name,
		int (*func)(lua_State*));

};



#endif // !__LUA_WRAPPER_H__
