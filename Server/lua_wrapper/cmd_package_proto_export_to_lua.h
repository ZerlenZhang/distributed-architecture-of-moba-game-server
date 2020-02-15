#ifndef __CMD_PACKAGE_PROTO_EXPORT_TO_LUA_H__
#define __CMD_PACKAGE_PROTO_EXPORT_TO_LUA_H__

struct lua_State;
void register_package_proto_export(lua_State* lua);
void register_rawcmd_export(lua_State* lua);


#endif // !__CMD_PACKAGE_PROTO_EXPORT_TO_LUA_H__
