#include "timer_export_to_lua.h"
#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"
#include "lua_wrapper.h"
#include "../utils/timer/time_list.h"
#include <cstdlib>

#include "../utils/cache_alloc/small_alloc.h"
#include "../utils/logger/logger.h"

#define my_alloc small_alloc
#define my_free small_free

struct timer_repeat
{
	int handler;
	int repeat_count;
};

static void on_lua_repeat_func(void* udata)
{
	auto tr = (timer_repeat*)udata;
	lua_wrapper::ExeScriptHandle(tr->handler,0);
	if (tr->repeat_count-- <= -1)
	{
		return;
	}
	if (tr->repeat_count == 0)
	{
		lua_wrapper::RemoveScriptHandle(tr->handler);
		my_free(tr);
	}
}


static int lua_timer_repeat(lua_State* lua)
{
	if (4 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto handle = toluafix_ref_function(lua, 1, 0);

	if (!handle)
	{
		lua_pushnil(lua);
		return 1;
	}
	auto after_msec = (int)lua_tointeger(lua, 2, 0);
	if (after_msec < 0)
	{
		if (handle)
		{
			lua_wrapper::RemoveScriptHandle(handle);
		}
		lua_pushnil(lua);
		return 1;
	}
	auto repeatCount = (int)lua_tointeger(lua, 3, );
	if (repeatCount == 0)
	{
		if (handle)
		{
			lua_wrapper::RemoveScriptHandle(handle);
		}
		lua_pushnil(lua);
		return 1;
	}
	else if (repeatCount < 0)
	{
		repeatCount = -1;
	}

	auto repeate_msec = (int)lua_tointeger(lua, 4, 0);
	if (repeate_msec <= 0)
	{
		repeate_msec = after_msec;
	}

	auto tr = (timer_repeat*)my_alloc(sizeof(timer_repeat));
	tr->handler = handle;
	tr->repeat_count = repeatCount;


	auto t = schedule_repeat(on_lua_repeat_func, tr, after_msec, repeatCount, repeate_msec);
	tolua_pushuserdata(lua, t);
	return 1;
}
static int lua_timer_once(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto handle = toluafix_ref_function(lua, 1, 0);
	if (!handle)
	{
		lua_pushnil(lua);
		return 1;
	}
	auto after_msec = (int)lua_tointeger(lua, 2, 0);
	if (after_msec < 0)
	{
		if (handle)
		{
			lua_wrapper::RemoveScriptHandle(handle);
		}
		lua_pushnil(lua);
		return 1;
	}
	auto tr = (timer_repeat*)my_alloc(sizeof(timer_repeat));
	tr->handler = handle;
	tr->repeat_count = 1;

	auto t = schedule_once(on_lua_repeat_func, tr, after_msec);
	tolua_pushuserdata(lua, t);
	return 1;
}
static int lua_timer_cancel(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	if (!lua_isuserdata(lua, 1))
	{
		return 0;
	}
	auto t = (timer*)lua_touserdata(lua, 1);
	auto tr = (timer_repeat*)get_timer_udata(t);
	lua_wrapper::RemoveScriptHandle(tr->handler);
	my_free(tr);
	cancel_timer(t);
	return 0;
}

 
void register_timer_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Timer", 0);
		tolua_beginmodule(tolua_S, "Timer");

		tolua_function(tolua_S, "Repeat", lua_timer_repeat);
		tolua_function(tolua_S, "Once", lua_timer_once);
		tolua_function(tolua_S, "Cancel", lua_timer_cancel);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
