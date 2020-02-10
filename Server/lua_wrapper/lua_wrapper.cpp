#include "lua_wrapper.h"
#include "../utils/logger/logger.h"

#include <tolua_fix.h>
#include "../database/mysql_export_to_lua.h"

lua_State* g_lua = NULL;



#pragma region Export_日志
static void lua_print_error(const char* fileName, int lineNum, const char* msg)
{
	logger::log(fileName, lineNum, tag_ERROR, msg);
}
static void lua_print_debug(const char* fileName, int lineNum, const char* msg)
{
	logger::log(fileName, lineNum, tag_DEBUG, msg);
}
static void lua_print_warning(const char* fileName, int lineNum, const char* msg)
{
	logger::log(fileName, lineNum, tag_WARNING, msg);
}
static void do_log_message(void(*log)(const char* file_name, int line_num, const char* msg), const char* msg) {
	lua_Debug info;
	int depth = 0;
	while (lua_getstack(g_lua, depth, &info)) {

		lua_getinfo(g_lua, "S", &info);
		lua_getinfo(g_lua, "n", &info);
		lua_getinfo(g_lua, "l", &info);

		if (info.source[0] == '@') {
			log(&info.source[1], info.currentline, msg);
			return;
		}

		++depth;
	}
	if (depth == 0) {
		log("trunk", 0, msg);
	}
}
static int lua_log_debug(lua_State* L)
{
	const char* msg = luaL_checkstring(L, -1);
	if (msg)
	{//是哪个LUA文件？-> 访问lua调用信息

		do_log_message(lua_print_debug, msg);
	}
	return 0;
}
static int lua_log_warning(lua_State* L)
{
	const char* msg = luaL_checkstring(L, -1);
	if (msg)
	{//是哪个LUA文件？-> 访问lua调用信息

		do_log_message(lua_print_warning, msg);
	}
	return 0;
}
static int lua_log_error(lua_State* L)
{
	const char* msg = luaL_checkstring(L, -1);
	if (msg)
	{//是哪个LUA文件？-> 访问lua调用信息

		do_log_message(lua_print_error, msg);
	}
	return 0;
}
#pragma endregion


#pragma region 重定义_lua_panic函数

static int lua_panic(lua_State* L)
{
	return lua_log_error(L);
}

#pragma endregion


static bool
pushFunctionByHandler(int nHandler)
{
	toluafix_get_function_by_refid(g_lua, nHandler);                  /* L: ... func */
	if (!lua_isfunction(g_lua, -1))
	{
		log_error("[LUA ERROR] function refid '%d' does not reference a Lua function", nHandler);
		lua_pop(g_lua, 1);
		return false;
	}
	return true;
}

static int
executeFunction(int numArgs)
{
	int functionIndex = -(numArgs + 1);
	if (!lua_isfunction(g_lua, functionIndex))
	{
		log_error("value at stack [%d] is not function", functionIndex);
		lua_pop(g_lua, numArgs + 1); // remove function and arguments
		return 0;
	}

	int traceback = 0;
	lua_getglobal(g_lua, "__G__TRACKBACK__");                         /* L: ... func arg1 arg2 ... G */
	if (!lua_isfunction(g_lua, -1))
	{
		lua_pop(g_lua, 1);                                            /* L: ... func arg1 arg2 ... */
	}
	else
	{
		lua_insert(g_lua, functionIndex - 1);                         /* L: ... G func arg1 arg2 ... */
		traceback = functionIndex - 1;
	}

	int error = 0;
	error = lua_pcall(g_lua, numArgs, 1, traceback);                  /* L: ... [G] ret */
	if (error)
	{
		if (traceback == 0)
		{
			log_error("[LUA ERROR] %s", lua_tostring(g_lua, -1));        /* L: ... error */
			lua_pop(g_lua, 1); // remove error message from stack
		}
		else                                                            /* L: ... G error */
		{
			lua_pop(g_lua, 2); // remove __G__TRACKBACK__ and error message from stack
		}
		return 0;
	}

	// get return value
	int ret = 0;
	if (lua_isnumber(g_lua, -1))
	{
		ret = (int)lua_tointeger(g_lua, -1);
	}
	else if (lua_isboolean(g_lua, -1))
	{
		ret = (int)lua_toboolean(g_lua, -1);
	}
	// remove return value from stack
	lua_pop(g_lua, 1);                                                /* L: ... [G] */

	if (traceback)
	{
		lua_pop(g_lua, 1); // remove __G__TRACKBACK__ from stack      /* L: ... */
	}

	return ret;
}


void lua_wrapper::Init()
{
	g_lua = luaL_newstate();
	//打开所有lua库
	luaL_openlibs(g_lua);
	//重定义终止函数，默认是直接终止
	lua_atpanic(g_lua, lua_panic);


	toluafix_open(g_lua);

	register_mysql_export(g_lua);

#pragma region Export_日志模块
	ExportFunc2Lua("log_error", lua_log_error);
	ExportFunc2Lua("log_debug", lua_log_debug);
	ExportFunc2Lua("log_warning", lua_log_warning);
#pragma endregion




}

void lua_wrapper::Exit()
{
	if (NULL != g_lua)
	{
		lua_close(g_lua);
		g_lua = NULL;
	}
}

lua_State* lua_wrapper::lua_state()
{
	return g_lua;
}

bool lua_wrapper::ExeLuaFile(char* luaFilePath)
{
	if (luaL_dofile(g_lua, luaFilePath))
	{
		lua_log_error(g_lua);
		return false;
	}

	return true;
}

int lua_wrapper::ExeScriptHandle(int nHandler, int numArgs)
{
	int ret = 0;
	if (pushFunctionByHandler(nHandler))                                /* L: ... arg1 arg2 ... func */
	{
		if (numArgs > 0)
		{
			lua_insert(g_lua, -(numArgs + 1));                        /* L: ... func arg1 arg2 ... */
		}
		ret = executeFunction(numArgs);
	}
	lua_settop(g_lua, 0);
	return ret;
}

void lua_wrapper::RemoveScriptHandle(int handle)
{
	toluafix_remove_function_by_refid(g_lua, handle);
}

void lua_wrapper::ExportFunc2Lua(const char* name, int(*func)(lua_State*))
{
	lua_pushcfunction(g_lua, func);
	lua_setglobal(g_lua, name);
}



