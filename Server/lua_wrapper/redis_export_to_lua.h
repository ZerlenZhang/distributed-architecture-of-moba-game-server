#ifndef __REDIS_EXPORT_TO_LUA_H__
#define __REDIS_EXPORT_TO_LUA_H__

struct lua_State;
int register_redis_export(lua_State* lua);



#endif // !__REDIS_EXPORT_TO_LUA_H__
