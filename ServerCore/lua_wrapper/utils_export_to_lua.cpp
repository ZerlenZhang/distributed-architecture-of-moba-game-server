#include "utils_export_to_lua.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"
#include "lua_wrapper.h"
#include "../utils/timestamp/timestamp.h"


static int lua_timestamp(lua_State* lua)
{
	auto ts = timestamp();
	lua_pushinteger(lua, ts);
	return 1;
}
static int lua_today(lua_State* lua)
{
	auto ts = timestamp_today();
	lua_pushinteger(lua, ts);
	return 1;

}
static int lua_yesterday(lua_State* lua)
{
	auto ts = timestamp_yesterday();
	lua_pushinteger(lua, ts);
	return 1;

}

void register_utils_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Utils", 0);
		tolua_beginmodule(tolua_S, "Utils");

		tolua_function(tolua_S, "TimeStamp", lua_timestamp);
		tolua_function(tolua_S, "Today", lua_today);
		tolua_function(tolua_S, "Yesterday", lua_yesterday);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
