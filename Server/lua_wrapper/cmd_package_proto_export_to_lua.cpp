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

#include <google/protobuf/message.h>
using google::protobuf::Message;
#include<map>
#include<string>
#include "../utils/logger/logger.h"
using std::map;
using std::string;
extern const string& GetProtoDir();
extern void push_proto_message_tolua(const Message* message);
static int lua_init(lua_State* lua)
{
	auto count = lua_gettop(lua);
	int protoType;
	if (count == 1)
	{
		protoType = (int)lua_tointeger(lua, 1);
		if (protoType < 0 || protoType>1)
		{
			log_debug("异常Init: %d", protoType);
			return 0;
		}
		CmdPackageProtocol::Init((ProtoType)protoType);
	}
	else if (count == 2)
	{
		protoType = (int)lua_tointeger(lua, 1);
		if (protoType < 0 || protoType>1)
		{
			log_debug("异常Init: %d", protoType);
			return 0;
		}
		auto dir = luaL_checkstring(lua, 2);
		if (dir)
		{
			CmdPackageProtocol::Init((ProtoType)protoType, dir);
		}
	}
	else
	{
		log_error("函数调用错误");
	}

	return 0;
}
static int lua_prototype(lua_State* lua)
{
	if (0 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	lua_pushinteger(lua, (int)CmdPackageProtocol::ProtoType());
	return 1;
}
static int lua_register_protobuf(lua_State* lua)
{
	if(1!=lua_gettop(lua))
	{
		log_error("ProtoManager.RegisterCmdMap_函数调用错误");
		return 0;
	}
	map<int, string> cmdMap;
	auto n = luaL_len(lua, 1);
	if (n < 0)
	{
		log_warning("ProtoManager.RegisterCmdMap_注册 0 个名字")
		return 0;

	}
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

static int lua_raw_readbody(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto raw = (RawPackage*)tolua_touserdata(lua, 1, 0);
	if (!raw)
	{
		log_error("RawCmd.ReadBody_包体为空");
		return 0;
	}
	CmdPackage* pkg;
	if (CmdPackageProtocol::DecodeBytesToCmdPackage(raw->rawCmd, raw->rawLen, pkg))
	{
		if (pkg->body)
		{
			switch (CmdPackageProtocol::ProtoType())
			{
			case ProtoType::Json:
				lua_pushstring(lua, (const char*)pkg->body);
				break;
			case ProtoType::Protobuf:
				push_proto_message_tolua((Message*)pkg->body);
				break;
			default:
				break;
			}
		}
		else {
			lua_pushnil(lua);
		}
		CmdPackageProtocol::FreeCmdPackage(pkg);

	}
	return 1;
}

static int lua_raw_readheader(lua_State* lua)
{
	auto argc = lua_gettop(lua);
	if (argc != 1)
	{
		log_error("函数调用错误");
		return 0;
	}
	auto raw = (RawPackage*)tolua_touserdata(lua, 1, NULL);
	if (raw)
	{
		lua_pushinteger(lua, raw->serviceType);
		lua_pushinteger(lua, raw->cmdType);
		lua_pushinteger(lua, raw->userTag);
		return 3;
	}
	log_error("RawCmd.ReadHeader_包体为空");
	return 0;
}

static int lua_set_utag(lua_State* lua)
{
	auto argc = lua_gettop(lua);
	if (argc != 2)
	{
		log_error("函数调用错误");
		return 0;
	}
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
	log_error("RawCmd.ReadHeader_包体为空");
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
		tolua_function(tolua_S, "ReadBody", lua_raw_readbody);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
