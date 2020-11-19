#include "session_export_to_lua.h"
#include <google/protobuf/message.h>
using namespace google::protobuf;

#include "../netbus/session/AbstractSession.h"
#include "../netbus/protocol/CmdPackageProtocol.h"
#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"
#include "../utils/logger/logger.h"
#include "../netbus/Netbus.h"

Message* lua_table_to_protobuf(lua_State* L, int stack_index, const char* msg_name) {
	if (!lua_istable(L, stack_index)) {
		return NULL;
	}

	Message* message = CmdPackageProtocol::CreateMessage(msg_name);
	if (!message) {
		log_error("cant find message  %s from compiled poll \n", msg_name);
		return NULL;
	}

	const Reflection* reflection = message->GetReflection();
	const Descriptor* descriptor = message->GetDescriptor();

	// 遍历table的所有key， 并且与 protobuf结构相比较。如果require的字段没有赋值， 报错！ 如果找不到字段，报错！
	for (int32_t index = 0; index < descriptor->field_count(); ++index) {
		const FieldDescriptor* fd = descriptor->field(index);
		const string& name = fd->name();

		bool isRequired = fd->is_required();
		bool bReapeted = fd->is_repeated();
		lua_pushstring(L, name.c_str());
		lua_rawget(L, stack_index);

		bool isNil = lua_isnil(L, -1);

		if (bReapeted) {
			if (isNil) {
				lua_pop(L, 1);
				continue;
			}
			else {
				bool isTable = lua_istable(L, -1);
				if (!isTable) {
					log_error("cant find required repeated field %s\n", name.c_str());
					CmdPackageProtocol::ReleaseMessage(message);
					return NULL;
				}
			}

			lua_pushnil(L);
			for (; lua_next(L, -2) != 0;) {
				switch (fd->cpp_type()) {

				case FieldDescriptor::CPPTYPE_DOUBLE:
				{
					double value = luaL_checknumber(L, -1);
					reflection->AddDouble(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_FLOAT:
				{
					float value = luaL_checknumber(L, -1);
					reflection->AddFloat(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_INT64:
				{
					int64_t value = luaL_checknumber(L, -1);
					reflection->AddInt64(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_UINT64:
				{
					uint64_t value = luaL_checknumber(L, -1);
					reflection->AddUInt64(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
				{
					int32_t value = luaL_checknumber(L, -1);
					const EnumDescriptor* enumDescriptor = fd->enum_type();
					const EnumValueDescriptor* valueDescriptor = enumDescriptor->FindValueByNumber(value);
					reflection->AddEnum(message, fd, valueDescriptor);
				}
				break;
				case FieldDescriptor::CPPTYPE_INT32:
				{
					int32_t value = luaL_checknumber(L, -1);
					reflection->AddInt32(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_UINT32:
				{
					uint32_t value = luaL_checknumber(L, -1);
					reflection->AddUInt32(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_STRING:
				{
					size_t size = 0;
					const char* value = luaL_checklstring(L, -1, &size);
					reflection->AddString(message, fd, std::string(value, size));
				}
				break;
				case FieldDescriptor::CPPTYPE_BOOL:
				{
					bool value = lua_toboolean(L, -1);
					reflection->AddBool(message, fd, value);
				}
				break;
				case FieldDescriptor::CPPTYPE_MESSAGE:
				{
					Message* value = lua_table_to_protobuf(L, lua_gettop(L), fd->message_type()->name().c_str());
					if (!value) {
						log_error("convert to message %s failed whith value %s\n", fd->message_type()->name().c_str(), name.c_str());
						CmdPackageProtocol::ReleaseMessage(value);
						return NULL;
					}
					Message* msg = reflection->AddMessage(message, fd);
					msg->CopyFrom(*value);
					CmdPackageProtocol::ReleaseMessage(value);
				}
				break;
				default:
					break;
				}

				// remove value， keep the key
				lua_pop(L, 1);
			}
		}
		else {

			if (isRequired) {
				if (isNil) {
					log_error("cant find required field %s\n", name.c_str());
					CmdPackageProtocol::ReleaseMessage(message);
					return NULL;
				}
			}
			else {
				if (isNil) {
					lua_pop(L, 1);
					continue;
				}
			}

			switch (fd->cpp_type()) {
			case FieldDescriptor::CPPTYPE_DOUBLE:
			{
				double value = (double)luaL_checknumber(L, -1);
				reflection->SetDouble(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_FLOAT:
			{
				float value = (float)luaL_checknumber(L, -1);
				reflection->SetFloat(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_INT64:
			{
				int64_t value = (int64_t)luaL_checknumber(L, -1);
				reflection->SetInt64(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_UINT64:
			{
				uint64_t value = (uint64_t)luaL_checknumber(L, -1);
				reflection->SetUInt64(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
			{
				int32_t value = (int32_t)luaL_checknumber(L, -1);
				const EnumDescriptor* enumDescriptor = fd->enum_type();
				const EnumValueDescriptor* valueDescriptor = enumDescriptor->FindValueByNumber(value);
				reflection->SetEnum(message, fd, valueDescriptor);
			}
			break;
			case FieldDescriptor::CPPTYPE_INT32:
			{
				int32_t value = (int32_t)luaL_checknumber(L, -1);
				reflection->SetInt32(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_UINT32:
			{
				uint32_t value = (uint32_t)luaL_checknumber(L, -1);
				reflection->SetUInt32(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_STRING:
			{
				size_t size = 0;
				const char* value = luaL_checklstring(L, -1, &size);
				reflection->SetString(message, fd, std::string(value, size));
			}
			break;
			case FieldDescriptor::CPPTYPE_BOOL:
			{
				bool value = lua_toboolean(L, -1);
				reflection->SetBool(message, fd, value);
			}
			break;
			case FieldDescriptor::CPPTYPE_MESSAGE:
			{
				Message* value = lua_table_to_protobuf(L, lua_gettop(L), fd->message_type()->name().c_str());
				if (!value) {
					log_error("convert to message %s failed whith value %s \n", fd->message_type()->name().c_str(), name.c_str());
					CmdPackageProtocol::ReleaseMessage(message);
					return NULL;
				}
				Message* msg = reflection->MutableMessage(message, fd);
				msg->CopyFrom(*value);
				CmdPackageProtocol::ReleaseMessage(value);
			}
			break;
			default:
				break;
			}
		}

		// pop value
		lua_pop(L, 1);
	}

	return message;
}


static int lua_session_close(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}

	session->Close();
	return 0;
}
static int lua_session_getaddress(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}

	int clientPort;
	auto ip = session->GetAddress(clientPort);
	lua_pushstring(lua, ip);
	lua_pushinteger(lua, clientPort);
	return 2;
}
static int lua_session_setutag(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}

	auto utag = (unsigned int)lua_tointeger(lua, 2);
	session->utag = utag;
	return 0;
}
static int lua_session_getutag(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}
	lua_pushinteger(lua, session->utag);
	return 1;
}

static int lua_session_setuid(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}

	auto uid = (unsigned int)lua_tointeger(lua, 2);
	session->uid = uid;
	return 0;
}
static int lua_session_getuid(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}
	lua_pushinteger(lua, session->uid);
	return 1;
}

static int lua_session_isclient(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}
	lua_pushboolean(lua,session->isClient ? 1:0);
	return 1;
}


static int lua_session_sendrawpackage(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	// 1
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}

	//2
	auto* pk = (struct RawPackage*)tolua_touserdata(lua, 2, NULL);
	if (!pk)
		return 0;

	session->SendRawPackage(pk);
	return 0;
}

//第一个参数：session
//第二个参数：表：{1： sType, 2: cType, 3: uTag, 4: body}
static int lua_session_sendpackage(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	// 1
	auto session = (AbstractSession*)tolua_touserdata(lua, 1, 0);
	if (NULL == session)
	{
		return 0;
	}
	// 2
	if (!lua_istable(lua, 2))
	{
		return 0;
	}

	CmdPackage msg;

	auto num = luaL_len(lua, 2);
	if (num != 4 && num != 3)
	{
		log_error("Session.SendPackage参数错误")
		return 0;
	}

	lua_pushnumber(lua, 1);
	lua_gettable(lua, 2);
	msg.serviceType = luaL_checkinteger(lua, -1);
	lua_pop(lua, 1);

	lua_pushnumber(lua, 2);
	lua_gettable(lua, 2);
	msg.cmdType = luaL_checkinteger(lua, -1);
	lua_pop(lua, 1);

	lua_pushnumber(lua, 3);
	lua_gettable(lua, 2);
	msg.userTag = luaL_checkinteger(lua, -1);
	lua_pop(lua, 1);

	if (num == 3)
	{
		msg.body = NULL;
		session->SendCmdPackage(&msg);
		return 0;
	}

	lua_pushnumber(lua, 4);
	lua_gettable(lua, 2);
	switch (CmdPackageProtocol::ProtoType())
	{
	case ProtoType::Json:
		msg.body = (char*)lua_tostring(lua, -1);
		session->SendCmdPackage(&msg);
		break;
	case ProtoType::Protobuf:
		if (!lua_istable(lua, -1))
		{
			msg.body = NULL;
		}
		else
		{// protobuf message table
			msg.body =
			lua_table_to_protobuf(lua, lua_gettop(lua),
				CmdPackageProtocol::GetMessageName(msg.serviceType, msg.cmdType));
		}
		session->SendCmdPackage(&msg);
		CmdPackageProtocol::ReleaseMessage((Message*)msg.body);
		break;
	}
	lua_pop(lua, 1);
	return 0;
}


static void udp_send_package(char* ip, int port, CmdPackage* msg)
{
	int pkgLen;
	unsigned char* pkg = CmdPackageProtocol::EncodeCmdPackageToBytes(msg, &pkgLen);
	if (pkg)
	{
		Netbus::Instance()->UdpSendTo(ip, port, pkg, pkgLen);
		CmdPackageProtocol::FreeCmdPackageBytes(pkg);
	}
}
 
// ip, port, {stype,ctype,utag,body}
static int lua_session_udpsendpackage(lua_State* lua)
{
	auto x = 1;
	x++;


	if (3 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	// 1
	const char* ip = tolua_tostring(lua, 1,NULL);
	if (NULL == ip)
	{
		log_error("Session.UdpSendPackage(ip,port,pkg)");
		return 0;
	}
	// 2
	int port = (int)tolua_tonumber(lua, 2, NULL);
	if (!port || port==-1)
	{
		log_error("Session.UdpSendPackage: 获取port失败，port: %d", port);
		return 0;
	}

	if (!lua_istable(lua, 3))
	{
		return 0;
	}

	CmdPackage msg;

	auto num = luaL_len(lua, 3);
	if (num != 4 && num != 3)
	{
		log_error("Session.SendPackage参数错误")
			return 0;
	}

	lua_pushnumber(lua, 1);
	lua_gettable(lua, 3);
	msg.serviceType = luaL_checkinteger(lua, -1);
	lua_pop(lua, 1);

	lua_pushnumber(lua, 2);
	lua_gettable(lua, 3);
	msg.cmdType = luaL_checkinteger(lua, -1);
	lua_pop(lua, 1);

	lua_pushnumber(lua, 3);
	lua_gettable(lua, 3);
	msg.userTag = luaL_checkinteger(lua, -1);
	lua_pop(lua, 1);

	if (num == 3)
	{
		msg.body = NULL;
		udp_send_package((char*)ip, port, &msg);
		return 0;
	}

	lua_pushnumber(lua, 4);
	lua_gettable(lua, 3);
	switch (CmdPackageProtocol::ProtoType())
	{
	case ProtoType::Json:
		msg.body = (char*)lua_tostring(lua, -1);
		udp_send_package((char*)ip, port, &msg);
		break;
	case ProtoType::Protobuf:
		if (!lua_istable(lua, -1))
		{
			msg.body = NULL;
		}
		else
		{// protobuf message table 
			msg.body =
				lua_table_to_protobuf(lua, lua_gettop(lua),
					CmdPackageProtocol::GetMessageName(msg.serviceType, msg.cmdType));
		}
		udp_send_package((char*)ip, port, &msg);
		CmdPackageProtocol::ReleaseMessage((Message*)msg.body);
		break;
	}
	lua_pop(lua, 1);
	return 0;
}


void register_session_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Session", 0);
		tolua_beginmodule(tolua_S, "Session");

		tolua_function(tolua_S, "Close", lua_session_close);
		tolua_function(tolua_S, "GetAddress", lua_session_getaddress);
		tolua_function(tolua_S, "SendPackage", lua_session_sendpackage);
		tolua_function(tolua_S, "SendRawPackage", lua_session_sendrawpackage);
		tolua_function(tolua_S, "UdpSendPackage", lua_session_udpsendpackage);
		
		tolua_function(tolua_S, "SetUTag", lua_session_setutag);
		tolua_function(tolua_S, "GetUTag", lua_session_getutag);
		tolua_function(tolua_S, "SetUId", lua_session_setuid);
		tolua_function(tolua_S, "GetUId", lua_session_getuid);
		tolua_function(tolua_S, "IsClient", lua_session_isclient);
		//tolua_function(tolua_S, "query", lua_redis_query);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
