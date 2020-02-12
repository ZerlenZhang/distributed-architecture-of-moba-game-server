#include "cmd_package_proto_export_to_lua.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus


#include "tolua_fix.h"
#include "../netbus/protocol/CmdPackageProtocol.h"


#include<map>
#include<string>
using std::map;
using std::string;
static int lua_init(lua_State* lua)
{
	if (lua_gettop(lua) != 1)
		return 0;

	auto protoType = (int)lua_tointeger(lua, 1);
	if (protoType < 0 || protoType>1)
		return 0;

	CmdPackageProtocol::Init((ProtoType)protoType);
}
static int lua_prototype(lua_State* lua)
{
	lua_pushinteger(lua, (int)CmdPackageProtocol::ProtoType());
	return 1;
}
static int lua_register_protobuf(lua_State* lua)
{
	map<int, string> cmdMap;
	auto n = luaL_len(lua, 1);
	if (n < 0)
		return 0;
	for (auto i = 1; i <= n; i++)
	{
		lua_pushnumber(lua, i);
		lua_gettable(lua, 1);
		auto name = luaL_checkstring(lua, -1);
		if (name)
		{
			cmdMap[i] = name;
		}
		lua_pop(lua, 1);
		
	}
	CmdPackageProtocol::RegisterProtobufCmdMap(cmdMap);
	return 0;

}



void register_package_proto_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "ProtoManager", 0);
		tolua_beginmodule(tolua_S, "ProtoManager");

		tolua_function(tolua_S, "Init", lua_init);
		tolua_function(tolua_S, "ProtoType", lua_prototype);
		tolua_function(tolua_S, "RegisterCmdMap", lua_register_protobuf);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
