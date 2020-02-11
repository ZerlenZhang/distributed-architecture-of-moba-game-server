#ifndef __MYSQL_EXPORT_TO_LUA_H__

#define __MYSQL_EXPORT_TO_LUA_H__

struct lua_State;

int register_mysql_export(lua_State* l);
#endif // !__MYSQL_EXPORT_TO_LUA_H__
