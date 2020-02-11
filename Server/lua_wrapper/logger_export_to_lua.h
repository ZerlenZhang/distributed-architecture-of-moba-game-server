#ifndef __LOGGER_EXPORT_TO_LUA_H__
#define __LOGGER_EXPORT_TO_LUA_H__



struct lua_State;


int register_logger_export(lua_State* lua);
int lua_log_error(lua_State* lua);





#endif // !__LOGGER_EXPORT_TO_LUA_H__
