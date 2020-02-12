#include "lua_wrapper.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus


#include "tolua_fix.h"
#include <string>
#include "mysql_export_to_lua.h"
#include "redis_export_to_lua.h"
#include "service_export_to_lua.h"
#include "session_export_to_lua.h"
#include "logger_export_to_lua.h"
#include "../utils/logger/logger.h"
#include "timer_export_to_lua.h"
#include "netbus_export_to_lua.h"
#include "cmd_package_proto_export_to_lua.h"

lua_State* g_lua = NULL;


static int lua_lua_addsearchpath(lua_State* lua)
{
	auto path = luaL_checkstring(lua, 1);
	if (path)
	{
		lua_wrapper::AddSearchPath(path);
	}
	return 0;
}


static void register_lua_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Lua", 0);
		tolua_beginmodule(tolua_S, "Lua");

		tolua_function(tolua_S, "AddSearchPath", lua_lua_addsearchpath);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}




#pragma region Native
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

#pragma endregion




#pragma region lua_wrapper
void lua_wrapper::Init()
{
	g_lua = luaL_newstate();
	//打开所有lua库
	luaL_openlibs(g_lua);
	toluafix_open(g_lua);

	register_logger_export(g_lua);
	register_mysql_export(g_lua);
	register_redis_export(g_lua);
	register_service_export(g_lua);
	register_session_export(g_lua); 
	register_timer_export(g_lua);
	register_lua_export(g_lua);
	register_netbus_export(g_lua);
	register_package_proto_export(g_lua);
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

bool lua_wrapper::DoFile(const std::string& luaFilePath)
{
	if (luaL_dofile(g_lua, luaFilePath.c_str()))
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

void lua_wrapper::AddSearchPath(const std::string& path)
{
	char strPath[1024] = { 0 };
	sprintf(strPath, "local path = string.match([[%s]],[[(.*)/[^/]*$]])\n package.path = package.path .. [[;]] .. path .. [[/?.lua;]] .. path .. [[/?/init.lua]]\n", path.c_str());
	luaL_dostring(g_lua, strPath);
}


void lua_wrapper::ExportFunc2Lua(const char* name, int(*func)(lua_State*))
{
	lua_pushcfunction(g_lua, func);
	lua_setglobal(g_lua, name);
}

#pragma endregion





