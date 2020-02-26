#include "service_export_to_lua.h"


#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"
#include <google/protobuf/message.h>
using namespace google::protobuf;

#include "lua_wrapper.h"
#include "../utils/logger/logger.h"
#include "../netbus/service/ServiceManager.h"
#include "../netbus/protocol/CmdPackageProtocol.h"



#pragma region Service模块全局表
#define SERVICE_FUNCTION_MAPPING "service_function_mapping"

static void init_service_function_map(lua_State* lua)
{
	lua_pushstring(lua, SERVICE_FUNCTION_MAPPING);
	lua_newtable(lua);
	lua_rawset(lua, LUA_REGISTRYINDEX);
}

static unsigned int s_function_ref_id = 0;
static unsigned int
save_service_function(lua_State* L, int lo, int def)
{
	// function at lo
	if (!lua_isfunction(L, lo)) return 0;

	s_function_ref_id++;

	lua_pushstring(L, SERVICE_FUNCTION_MAPPING);
	lua_rawget(L, LUA_REGISTRYINDEX);                           /* stack: fun ... refid_fun */
	lua_pushinteger(L, s_function_ref_id);                      /* stack: fun ... refid_fun refid */
	lua_pushvalue(L, lo);                                       /* stack: fun ... refid_fun refid fun */

	lua_rawset(L, -3);                  /* refid_fun[refid] = fun, stack: fun ... refid_ptr */
	lua_pop(L, 1);                                              /* stack: fun ... */

	return s_function_ref_id;

	// lua_pushvalue(L, lo);                                           /* stack: ... func */
	// return luaL_ref(L, LUA_REGISTRYINDEX);
}

static void
get_service_function(lua_State* L, int refid)
{
	lua_pushstring(L, SERVICE_FUNCTION_MAPPING);
	lua_rawget(L, LUA_REGISTRYINDEX);                           /* stack: ... refid_fun */
	lua_pushinteger(L, refid);                                  /* stack: ... refid_fun refid */
	lua_rawget(L, -2);                                          /* stack: ... refid_fun fun */
	lua_remove(L, -2);                                          /* stack: ... fun */
}

static bool
push_service_function(int nHandler)
{
	get_service_function(lua_wrapper::lua_state(), nHandler);                  /* L: ... func */
	if (!lua_isfunction(lua_wrapper::lua_state(), -1))
	{
		log_error("[LUA ERROR] function refid '%d' does not reference a Lua function", nHandler);
		lua_pop(lua_wrapper::lua_state(), 1);
		return false;
	}
	return true;
}

static int
exe_function(int numArgs)
{
	int functionIndex = -(numArgs + 1);
	if (!lua_isfunction(lua_wrapper::lua_state(), functionIndex))
	{
		log_error("value at stack [%d] is not function", functionIndex);
		lua_pop(lua_wrapper::lua_state(), numArgs + 1); // remove function and arguments
		return 0;
	}

	int traceback = 0;
	lua_getglobal(lua_wrapper::lua_state(), "__G__TRACKBACK__");                         /* L: ... func arg1 arg2 ... G */
	if (!lua_isfunction(lua_wrapper::lua_state(), -1))
	{
		lua_pop(lua_wrapper::lua_state(), 1);                                            /* L: ... func arg1 arg2 ... */
	}
	else
	{
		lua_insert(lua_wrapper::lua_state(), functionIndex - 1);                         /* L: ... G func arg1 arg2 ... */
		traceback = functionIndex - 1;
	}

	int error = 0;
	error = lua_pcall(lua_wrapper::lua_state(), numArgs, 1, traceback);                  /* L: ... [G] ret */
	if (error)
	{
		if (traceback == 0)
		{
			log_error("[LUA ERROR] %s", lua_tostring(lua_wrapper::lua_state(), -1));        /* L: ... error */
			lua_pop(lua_wrapper::lua_state(), 1); // remove error message from stack
		}
		else                                                            /* L: ... G error */
		{
			lua_pop(lua_wrapper::lua_state(), 2); // remove __G__TRACKBACK__ and error message from stack
		}
		return 0;
	}

	// get return value
	int ret = 0;
	if (lua_isnumber(lua_wrapper::lua_state(), -1))
	{
		ret = (int)lua_tointeger(lua_wrapper::lua_state(), -1);
	}
	else if (lua_isboolean(lua_wrapper::lua_state(), -1))
	{
		ret = (int)lua_toboolean(lua_wrapper::lua_state(), -1);
	}
	// remove return value from stack
	lua_pop(lua_wrapper::lua_state(), 1);                                                /* L: ... [G] */

	if (traceback)
	{
		lua_pop(lua_wrapper::lua_state(), 1); // remove __G__TRACKBACK__ from stack      /* L: ... */
	}

	return ret;
}

static int
execute_service_function(int nHandler, int numArgs) {
	int ret = 0;
	if (push_service_function(nHandler))                                /* L: ... arg1 arg2 ... func */
	{
		if (numArgs > 0)
		{
			lua_insert(lua_wrapper::lua_state(), -(numArgs + 1));                        /* L: ... func arg1 arg2 ... */
		}
		ret = exe_function(numArgs);
	}
	lua_settop(lua_wrapper::lua_state(), 0);
	return ret;
}
#pragma endregion


void
push_proto_message_tolua(const Message* message) {
	lua_State* state = lua_wrapper::lua_state();
	if (!message) {
		// printf("PushProtobuf2LuaTable failed, message is NULL");
		return;
	}
	const Reflection* reflection = message->GetReflection();

	// 顶层table
	lua_newtable(state);

	const Descriptor* descriptor = message->GetDescriptor();
	for (int32_t index = 0; index < descriptor->field_count(); ++index) {
		const FieldDescriptor* fd = descriptor->field(index);
		const std::string& name = fd->lowercase_name();

		// key
		lua_pushstring(state, name.c_str());

		bool bReapeted = fd->is_repeated();

		if (bReapeted) {
			// repeated这层的table
			lua_newtable(state);
			int size = reflection->FieldSize(*message, fd);
			for (int i = 0; i < size; ++i) {
				char str[32] = { 0 };
				switch (fd->cpp_type()) {
				case FieldDescriptor::CPPTYPE_DOUBLE:
					lua_pushnumber(state, reflection->GetRepeatedDouble(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_FLOAT:
					lua_pushnumber(state, (double)reflection->GetRepeatedFloat(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_INT64:
					sprintf(str, "%lld", (long long)reflection->GetRepeatedInt64(*message, fd, i));
					lua_pushstring(state, str);
					break;
				case FieldDescriptor::CPPTYPE_UINT64:

					sprintf(str, "%llu", (unsigned long long)reflection->GetRepeatedUInt64(*message, fd, i));
					lua_pushstring(state, str);
					break;
				case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
					lua_pushinteger(state, reflection->GetRepeatedEnum(*message, fd, i)->number());
					break;
				case FieldDescriptor::CPPTYPE_INT32:
					lua_pushinteger(state, reflection->GetRepeatedInt32(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_UINT32:
					lua_pushinteger(state, reflection->GetRepeatedUInt32(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_STRING:
				{
					std::string value = reflection->GetRepeatedString(*message, fd, i);
					lua_pushlstring(state, value.c_str(), value.size());
				}
				break;
				case FieldDescriptor::CPPTYPE_BOOL:
					lua_pushboolean(state, reflection->GetRepeatedBool(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_MESSAGE:
					push_proto_message_tolua(&(reflection->GetRepeatedMessage(*message, fd, i)));
					break;
				default:
					break;
				}

				lua_rawseti(state, -2, i + 1); // lua's index start at 1
			}

		}
		else {
			char str[32] = { 0 };
			switch (fd->cpp_type()) {

			case FieldDescriptor::CPPTYPE_DOUBLE:
				lua_pushnumber(state, reflection->GetDouble(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_FLOAT:
				lua_pushnumber(state, (double)reflection->GetFloat(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_INT64:

				sprintf(str, "%lld", (long long)reflection->GetInt64(*message, fd));
				lua_pushstring(state, str);
				break;
			case FieldDescriptor::CPPTYPE_UINT64:

				sprintf(str, "%llu", (unsigned long long)reflection->GetUInt64(*message, fd));
				lua_pushstring(state, str);
				break;
			case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
				lua_pushinteger(state, (int)reflection->GetEnum(*message, fd)->number());
				break;
			case FieldDescriptor::CPPTYPE_INT32:
				lua_pushinteger(state, reflection->GetInt32(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_UINT32:
				lua_pushinteger(state, reflection->GetUInt32(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_STRING:
			{
				std::string value = reflection->GetString(*message, fd);
				lua_pushlstring(state, value.c_str(), value.size());
			}
			break;
			case FieldDescriptor::CPPTYPE_BOOL:
				lua_pushboolean(state, reflection->GetBool(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_MESSAGE:
				push_proto_message_tolua(&(reflection->GetMessage(*message, fd)));
				break;
			default:
				break;
			}
		}

		lua_rawset(state, -3);
	}
}


//第一个参数：sType
//第二个参数：表：{OnSessionRecvCmd，OnSessionDisconnected}
static int lua_register_service(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto  serviceType = tolua_tonumber(lua, 1, 0);
	if (!lua_istable(lua, 2))
	{// 如果不是表，直接返回
		lua_pushboolean(lua, 0);
		return 1;
	}

	lua_getfield(lua, 2, "OnSessionRecvCmd");
	lua_getfield(lua, 2, "OnSessionDisconnected");
	lua_getfield(lua, 2, "OnSessionConnected");
	//栈： 3:OnSessionRecvCmd  4:OnSessionDisconnected

	auto recvHandle = save_service_function(lua, 3, 0);
	auto disHandle = save_service_function(lua, 4, 0);
	auto connHandle = save_service_function(lua, 5, 0);

	if (recvHandle || disHandle)
	{// 两个函数不都为零

		auto ls = new LuaService();
		ls->luaDisconnectFuncHandle = disHandle;
		ls->luaRecvCmdPackageHandle = recvHandle;
		ls->luaConnFuncHandle = connHandle;
		ls->luaRecvRawPackageHandle = 0;

		ls->useRawPackage = false;

		auto ret = ServiceManager::RegisterService(serviceType, ls);
		
		lua_pushboolean(lua, ret ? 1 : 0);
		return 1;
	}
	lua_pushboolean(lua, 0);
	return 1;
}

static int lua_register_raw_service(lua_State* lua)
{
	if (2 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto  serviceType = tolua_tonumber(lua, 1, 0);
	if (!lua_istable(lua, 2))
	{// 如果不是表，直接返回
		lua_pushboolean(lua, 0);
		return 1;
	}

	lua_getfield(lua, 2, "OnSessionRecvRaw");
	lua_getfield(lua, 2, "OnSessionDisconnected");
	lua_getfield(lua, 2, "OnSessionConnected");
	//栈： 3:OnSessionRecvCmd  4:OnSessionDisconnected

	auto recvHandle = save_service_function(lua, 3, 0);
	auto disHandle = save_service_function(lua, 4, 0);
	auto connHandle = save_service_function(lua, 5, 0);

	if (recvHandle || disHandle)
	{// 两个函数不都为零

		auto ls = new LuaService();
		ls->luaDisconnectFuncHandle = disHandle;
		ls->luaRecvCmdPackageHandle = 0;
		ls->luaConnFuncHandle = connHandle;
		ls->luaRecvRawPackageHandle = recvHandle;

		ls->useRawPackage = true;

		auto ret = ServiceManager::RegisterService(serviceType, ls);

		lua_pushboolean(lua, ret ? 1 : 0);
		return 1;
	}
	lua_pushboolean(lua, 0);
	return 1;
}


int register_service_export(lua_State* tolua_S)
{
	//初始化service全局表
	init_service_function_map(tolua_S);

	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Service", 0);
		tolua_beginmodule(tolua_S, "Service");

		tolua_function(tolua_S, "Register", lua_register_service);
		tolua_function(tolua_S, "RegisterRaw", lua_register_raw_service);

		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}


#pragma region LuaService

#pragma endregion


bool LuaService::OnSessionRecvRawPackage(const AbstractSession* session, const RawPackage* package) const
{
	auto lua = lua_wrapper::lua_state();
	tolua_pushuserdata(lua, (void*)session);
	tolua_pushuserdata(lua, (void*)package);

	if (luaRecvRawPackageHandle == 0)
	{
		log_error("OnSessionRecvCmdPackage：luaRecvRawPackageHandle==0");
		return false;
	}
	execute_service_function(this->luaRecvRawPackageHandle, 2);
	return true;
}

bool LuaService::OnSessionRecvCmdPackage(const AbstractSession* session, const CmdPackage* package) const
{
	// 如果是Protobuf就把内容解析后传给Lua，如果是json就不变
	// { 1： serviceType, 2: cmdType, 3 uTag, 4:body }
	auto index = 1;
	auto lua = lua_wrapper::lua_state();
	tolua_pushuserdata(lua, (void*)session);
	lua_newtable(lua);
	lua_pushinteger(lua, package->serviceType);
	lua_rawseti(lua, -2, index++);
	lua_pushinteger(lua, package->cmdType);
	lua_rawseti(lua, -2, index++);
	lua_pushinteger(lua, package->userTag);
	lua_rawseti(lua, -2, index++);

	if (!package->body)
	{
		lua_pushnil(lua);
	}
	else
	{
		switch (CmdPackageProtocol::ProtoType())
		{
		case ProtoType::Json:
			lua_pushstring(lua, (char*)package->body);
			break;
		case ProtoType::Protobuf:
			push_proto_message_tolua((Message*)package->body);
			break;
		}
	}
	lua_rawseti(lua, -2, index++);
	if (luaRecvCmdPackageHandle == 0)
	{
		log_error("OnSessionRecvCmdPackage：luaRecvCmdPackageHandle==0");
		return false;
	}
	execute_service_function(this->luaRecvCmdPackageHandle, 2);
	return true;
}

bool LuaService::OnSessionDisconnected(const AbstractSession* session, const int& serviceType) const
{
	tolua_pushuserdata(lua_wrapper::lua_state(),(void*) session);
	tolua_pushnumber(lua_wrapper::lua_state(), serviceType);

	if (luaDisconnectFuncHandle == 0)
	{
		log_error("OnSessionRecvCmdPackage：luaDisconnectFuncHandle==0");
		return false;
	}
	execute_service_function(this->luaDisconnectFuncHandle, 2);
	return true;
}

void LuaService::OnSessionConnected(const AbstractSession* session, const int& serviceType) const
{
	tolua_pushuserdata(lua_wrapper::lua_state(), (void*)session);
	tolua_pushnumber(lua_wrapper::lua_state(), serviceType);
	if(this->luaConnFuncHandle)
		execute_service_function(this->luaConnFuncHandle, 2);
}
