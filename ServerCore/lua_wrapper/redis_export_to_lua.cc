#include "redis_export_to_lua.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus


#include "tolua_fix.h"
#include "../database/rediswarpper.h"
#include "../utils/logger/logger.h"
#include "lua_wrapper.h"

static void push_result_to_lua(redisReply* result)
{
	auto lua = lua_wrapper::lua_state();
	int index = 1;
	switch (result->type)
	{
	case REDIS_REPLY_ARRAY:
		lua_newtable(lua);
		for (auto i = 0; i < result->elements; i++)
		{
			push_result_to_lua(result->element[i]);
			lua_rawseti(lua, -2, index);
			++index;
		}
		
		break;
	case REDIS_REPLY_INTEGER:
		lua_pushinteger(lua, result->integer);
		break;
	case REDIS_REPLY_NIL:
		lua_pushnil(lua);
		break;
	case REDIS_REPLY_STATUS:
	case REDIS_REPLY_STRING:
		lua_pushstring(lua, result->str);
		break;
	}
}

#pragma region Callback
static void
on_open_cb(const char* err, RedisContext* context) {
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
on_lua_query_cb(const char* err, RedisReply* result)
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
		if (result->reply)
		{// 把查询到的结果push成一个表
			push_result_to_lua(result->reply);
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



static int
lua_redis_connect(lua_State* tolua_S) {
	if (3 != lua_gettop(tolua_S))
	{
		log_error("函数调用错误");
		return 0;
	}
	char* ip = (char*)tolua_tostring(tolua_S, 1, 0);
	if (ip == NULL) {
		return 0;
	}

	int port = (int)tolua_tonumber(tolua_S, 2, 0);

	int handler = toluafix_ref_function(tolua_S, 3, 0);
	redis_wrapper::connect(ip, port, on_open_cb, (void*)handler, false);
	return 0;
}

static int
lua_redis_close(lua_State* tolua_S) {
	if (1 != lua_gettop(tolua_S))
	{
		log_error("函数调用错误");
		return 0;
	}
	void* context = tolua_touserdata(tolua_S, 1, 0);
	if (context) {
		redis_wrapper::close_redis((RedisContext*)context);
	}
	else
	{
		log_error("Redis 关闭异常");
	}

	return 0;
}

static int
lua_redis_query(lua_State* tolua_S) {
	if (3 != lua_gettop(tolua_S))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto context = tolua_touserdata(tolua_S, 1, 0);
	if (!context)
		return 0;
	auto cmd = tolua_tostring(tolua_S, 2, 0);
	if (!cmd)
		return 0;
	auto handle = toluafix_ref_function(tolua_S, 3, 0);
	if (!handle)
		return 0;
	redis_wrapper::query(
		(RedisContext*)context,
		(char*)cmd,
		on_lua_query_cb,
		(void*)handle,
		false);
	return 0;
}


int register_redis_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Redis", 0);
		tolua_beginmodule(tolua_S, "Redis");

		tolua_function(tolua_S, "Connect", lua_redis_connect);
		tolua_function(tolua_S, "Close", lua_redis_close);
		tolua_function(tolua_S, "Query", lua_redis_query);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
