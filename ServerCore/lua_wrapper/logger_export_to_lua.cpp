#include "logger_export_to_lua.h"


#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"
#include <string>
#include "../utils/logger/logger.h"
#include "lua_wrapper.h"

#pragma region 重定义_lua_panic函数

static int lua_panic(lua_State* L)
{
	return lua_log_error(L);
}

#pragma endregion

static void
print_error(const char* file_name, int line_num, const char* msg) {
	logger::log(file_name, line_num, tag_ERROR, msg);
}

static void
print_warning(const char* file_name, int line_num, const char* msg) {
	logger::log(file_name, line_num, tag_WARNING, msg);
}

static void
print_debug(const char* file_name, int line_num, const char* msg) {
	logger::log(file_name, line_num, tag_DEBUG, msg);
}

static void
do_log_message(void(*log)(const char* file_name, int line_num, const char* msg), const char* msg) {
	lua_Debug info;
	int depth = 0;
	auto g_lua_State = lua_wrapper::lua_state();
	while (lua_getstack(g_lua_State, depth, &info)) {

		lua_getinfo(g_lua_State, "S", &info);
		lua_getinfo(g_lua_State, "n", &info);
		lua_getinfo(g_lua_State, "l", &info);

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

static int
lua_log_debug(lua_State* luastate) {
	int nargs = lua_gettop(luastate);
	std::string t;
	for (int i = 1; i <= nargs; i++)
	{
		if (lua_istable(luastate, i))
			t += "table";
		else if (lua_isnone(luastate, i))
			t += "none";
		else if (lua_isnil(luastate, i))
			t += "nil";
		else if (lua_isboolean(luastate, i))
		{
			if (lua_toboolean(luastate, i) != 0)
				t += "true";
			else
				t += "false";
		}
		else if (lua_isfunction(luastate, i))
			t += "function";
		else if (lua_islightuserdata(luastate, i))
			t += "lightuserdata";
		else if (lua_isthread(luastate, i))
			t += "thread";
		else
		{
			const char* str = lua_tostring(luastate, i);
			if (str)
				t += lua_tostring(luastate, i);
			else
				t += lua_typename(luastate, lua_type(luastate, i));
		}
		if (i != nargs)
			t += "\t";
	}
	do_log_message(print_debug, t.c_str());
	return 0;
}

static int
lua_log_warning(lua_State* luastate) {
	int nargs = lua_gettop(luastate);
	std::string t;
	for (int i = 1; i <= nargs; i++)
	{
		if (lua_istable(luastate, i))
			t += "table";
		else if (lua_isnone(luastate, i))
			t += "none";
		else if (lua_isnil(luastate, i))
			t += "nil";
		else if (lua_isboolean(luastate, i))
		{
			if (lua_toboolean(luastate, i) != 0)
				t += "true";
			else
				t += "false";
		}
		else if (lua_isfunction(luastate, i))
			t += "function";
		else if (lua_islightuserdata(luastate, i))
			t += "lightuserdata";
		else if (lua_isthread(luastate, i))
			t += "thread";
		else
		{
			const char* str = lua_tostring(luastate, i);
			if (str)
				t += lua_tostring(luastate, i);
			else
				t += lua_typename(luastate, lua_type(luastate, i));
		}
		if (i != nargs)
			t += "\t";
	}
	do_log_message(print_warning, t.c_str());
	return 0;
}

int lua_log_error(lua_State* luastate) {
	int nargs = lua_gettop(luastate);
	std::string t;
	for (int i = 1; i <= nargs; i++)
	{
		if (lua_istable(luastate, i))
			t += "table";
		else if (lua_isnone(luastate, i))
			t += "none";
		else if (lua_isnil(luastate, i))
			t += "nil";
		else if (lua_isboolean(luastate, i))
		{
			if (lua_toboolean(luastate, i) != 0)
				t += "true";
			else
				t += "false";
		}
		else if (lua_isfunction(luastate, i))
			t += "function";
		else if (lua_islightuserdata(luastate, i))
			t += "lightuserdata";
		else if (lua_isthread(luastate, i))
			t += "thread";
		else
		{
			const char* str = lua_tostring(luastate, i);
			if (str)
				t += lua_tostring(luastate, i);
			else
				t += lua_typename(luastate, lua_type(luastate, i));
		}
		if (i != nargs)
			t += "\t";
	}
	do_log_message(print_error, t.c_str());
	return 0;
}

static int
lua_logger_init(lua_State* tolua_S) {
	if (3 != lua_gettop(tolua_S))
	{
		log_error("函数调用错误");
		return 0;
	}

	const char* path = lua_tostring(tolua_S, 1);
	if (path == NULL) {
		return 0;
	}

	const char* prefix = lua_tostring(tolua_S, 2);
	if (prefix == NULL) {
		return 0;
	}

	bool std_out = lua_toboolean(tolua_S, 3);
	logger::init(path, prefix, std_out);

	return 0;
}


int register_logger_export(lua_State* tolua_S)
{

	//重定义终止函数，默认是直接终止
	lua_atpanic(tolua_S, lua_panic);

	//重载print
	lua_wrapper::ExportFunc2Lua("print", lua_log_debug);

	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Debug", 0);
		tolua_beginmodule(tolua_S, "Debug");

		tolua_function(tolua_S, "LogInit", lua_logger_init);
		tolua_function(tolua_S, "Log", lua_log_debug);
		tolua_function(tolua_S, "LogWarning", lua_log_warning);
		tolua_function(tolua_S, "LogError", lua_log_error);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
