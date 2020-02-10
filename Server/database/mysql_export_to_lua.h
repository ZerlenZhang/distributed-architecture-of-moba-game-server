#ifndef __MYSQL_EXPORT_TO_LUA_H__

#define __MYSQL_EXPORT_TO_LUA_H__

struct lua_State;
struct MysqlContext;

static void on_open_cb(const char* err, void* context, MysqlContext* udata);

static void on_open_cb(const char* err, MysqlContext* context);

int register_mysql_export(lua_State* l);
#endif // !__MYSQL_EXPORT_TO_LUA_H__
