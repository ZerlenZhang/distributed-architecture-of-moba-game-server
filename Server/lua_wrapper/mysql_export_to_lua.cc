
#include "mysql_export_to_lua.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"

#include "../lua_wrapper/lua_wrapper.h"
#include "../database/mysqlwarpper.h"
#include "../utils/logger/logger.h"

static void push_mysql_row(MYSQL_ROW row, int num)
{
	auto lua = lua_wrapper::lua_state();
	lua_newtable(lua);
	auto index = 1;
	for (auto i = 0; i < num; i++)
	{
		if (row[i] == NULL)
		{
			lua_pushnil(lua);
		}
		else
		{
			lua_pushstring(lua, row[i]);
		}
		lua_rawseti(lua, -2, index);
		++index;
	}
}

#pragma region Callback
static void
on_open_cb(const char* err, MysqlContext* context) {
	if (err) {
		lua_pushstring(lua_wrapper::lua_state(), err);
		lua_pushnil(lua_wrapper::lua_state());
	}
	else {
		lua_pushnil(lua_wrapper::lua_state());
		tolua_pushuserdata(lua_wrapper::lua_state(), context);
	}

	lua_wrapper::ExeScriptHandle((int)context->udata, 2);
	lua_wrapper::RemoveScriptHandle((int)context->udata);
}

static void
on_lua_query_cb(const char* err, MysqlResult* result)
{
	auto lua = lua_wrapper::lua_state();
	if (err)
	{
		lua_pushstring(lua, err);
		lua_pushnil(lua);
	}
	else
	{
		lua_pushnil(lua);
		if (result->result)
		{// 把查询到的结果push成一个表
			lua_newtable(lua);
			auto index = 1;
			auto num = mysql_num_fields(result->result);
			MYSQL_ROW row;
			while (row = mysql_fetch_row(result->result))
			{
				push_mysql_row(row, num);
				lua_rawseti(lua, -2, index);
				++index;
			}
		}
		else 
		{
			lua_pushnil(lua_wrapper::lua_state());
		}
	}

	lua_wrapper::ExeScriptHandle((int)result->udata, 2);
	lua_wrapper::RemoveScriptHandle((int)result->udata);
}
#pragma endregion


#pragma region Lua_Callback
static int
lua_mysql_connect(lua_State* tolua_S) {
	char* ip = (char*)tolua_tostring(tolua_S, 1, 0);
	if (ip == NULL) {
		return 0;
	}

	int port = (int)tolua_tonumber(tolua_S, 2, 0);

	char* db_name = (char*)tolua_tostring(tolua_S, 3, 0);
	if (db_name == NULL) {
		return 0;
	}

	char* uname = (char*)tolua_tostring(tolua_S, 4, 0);
	if (uname == NULL) {
		return 0;
	}

	char* upwd = (char*)tolua_tostring(tolua_S, 5, 0);
	if (upwd == NULL) {
		return 0;
	}

	int handler = toluafix_ref_function(tolua_S, 6, 0);
	mysql_wrapper::connect(ip, port, db_name, uname, upwd, on_open_cb, (void*)handler,false);
	return 0;
}

static int
lua_mysql_close(lua_State* tolua_S) {
	void* context = tolua_touserdata(tolua_S, 1, 0);
	if (context) {
		mysql_wrapper::close((MysqlContext*)context);
	}
	else
	{
		log_error("Mysql 关闭异常");
	}

	return 0;
}

static int
lua_mysql_query(lua_State* tolua_S) {
	auto context = tolua_touserdata(tolua_S, 1, 0);
	if (!context)
		return 0;
	auto sql = tolua_tostring(tolua_S, 2, 0);
	if (!sql)
		return 0;
	auto handle = toluafix_ref_function(tolua_S, 3, 0);
	if (!handle)
		return 0;
	mysql_wrapper::query(
		(MysqlContext*)context,
		(char*)sql,
		on_lua_query_cb,
		(void*)handle,
		false);
	return 0;
}

#pragma endregion
int
register_mysql_export(lua_State* tolua_S) {
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Mysql", 0);
		tolua_beginmodule(tolua_S, "Mysql");

		tolua_function(tolua_S, "Connect", lua_mysql_connect);
		tolua_function(tolua_S, "Close", lua_mysql_close);
		tolua_function(tolua_S, "Query", lua_mysql_query);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}


