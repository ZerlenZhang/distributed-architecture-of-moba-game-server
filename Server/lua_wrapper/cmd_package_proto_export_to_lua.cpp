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
#include "../utils/logger/logger.h"
using std::map;
using std::string;
extern const string& GetProtoDir();
static int lua_init(lua_State* lua)
{
	auto protoType = (int)lua_tointeger(lua, 1);
	if (protoType < 0 || protoType>1)
	{
		log_debug("异常Init: %d", protoType);
		return 0;
	}
	auto dir = luaL_checkstring(lua, 2);

	if (dir)
	{
		CmdPackageProtocol::Init((ProtoType)protoType,dir);
	}
	else
	{
		CmdPackageProtocol::Init((ProtoType)protoType, GetProtoDir());
	}

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


static int lua_raw_readheader(lua_State* lua)
{
	auto argc = lua_gettop(lua);
	if (argc != 1)
		return 0;
	auto raw = (RawPackage*)tolua_touserdata(lua, 1, NULL);
	if (raw)
	{
		lua_pushinteger(lua, raw->serviceType);
		lua_pushinteger(lua, raw->cmdType);
		lua_pushinteger(lua, raw->userTag);
		return 3;
	}
	return 0;
}

static int lua_set_utag(lua_State* lua)
{
	auto argc = lua_gettop(lua);
	if (argc != 2)
		return 0;
	auto raw = (RawPackage*)tolua_touserdata(lua, 1, NULL);
	if (raw)
	{
		auto utag = (unsigned int)luaL_checkinteger(lua, 2);
		raw->userTag = utag;

		//设置元数据
		auto utagPointer = raw->rawCmd + 4;
		utagPointer[0] = utag & 0xff;
		utagPointer[1] = (utag & 0xff00) >> 8;
		utagPointer[2] = (utag & 0xff0000) >> 16;
		utagPointer[3] = (utag & 0xff000000) >> 24;

		return 0;
	}
	return 0;
}

void register_rawcmd_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "RawCmd", 0);
		tolua_beginmodule(tolua_S, "RawCmd");

		tolua_function(tolua_S, "ReadHeader", lua_raw_readheader);
		tolua_function(tolua_S, "SetUTag", lua_set_utag);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
