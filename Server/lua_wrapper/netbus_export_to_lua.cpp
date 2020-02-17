#include "netbus_export_to_lua.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus

#include "tolua_fix.h"

#include "../netbus/Netbus.h"
#include "lua_wrapper.h"
#include "../utils/logger/logger.h"

static void on_tcp_connect(int err, AbstractSession* s, void* udata)
{
	auto lua = lua_wrapper::lua_state();
	lua_pushinteger(lua, err);
	if (err)
	{
		lua_pushnil(lua);
	}
	else {
		tolua_pushuserdata(lua, s);
	}
	lua_wrapper::ExeScriptHandle((int)udata, 2);
	lua_wrapper::RemoveScriptHandle((int)udata);
}

static void on_tcp_listen(AbstractSession* s, void* udata)
{	
	//如果没有自定义参数就是没有传递第二第三参数
	if (udata == NULL)
		return;
	auto lua = lua_wrapper::lua_state();
	tolua_pushuserdata(lua, s);
	lua_wrapper::ExeScriptHandle((int)udata, 1);
	//lua_wrapper::RemoveScriptHandle((int)udata);
}

static void on_ws_listen(AbstractSession* s, void* udata)
{
	//如果没有自定义参数就是没有传递第二第三参数
	if (udata == NULL)
		return;
	auto lua = lua_wrapper::lua_state();
	tolua_pushuserdata(lua, s);
	lua_wrapper::ExeScriptHandle((int)udata, 1);
	//lua_wrapper::RemoveScriptHandle((int)udata);
}

// ip port lua_func
static int lua_tcp_connect(lua_State* lua)
{
	if (3 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto ip = luaL_checkstring(lua, 1);
	if (NULL == ip)
		return 0;
	auto port = luaL_checkinteger(lua, 2);
	auto handle = toluafix_ref_function(lua, 3, 0);
	if (0 == handle)
		return 0;

	Netbus::Instance()->TcpConnect(ip, port, on_tcp_connect, (void*)handle);
	return 0;
}

static int lua_tcp(lua_State* lua)
{
	auto cout = lua_gettop(lua);
	auto port = luaL_checkinteger(lua, 1);
	if (cout == 1)
	{
		Netbus::Instance()->TcpListen(port);
	}
	else if(cout == 2)
	{
		auto handle = toluafix_ref_function(lua, 2, 0);
		if (0 == handle)
		{
			log_error("TcpListen_函数句柄无效");
			return 0;
		}

		Netbus::Instance()->TcpListen(port, on_tcp_listen, (void*)handle);
	}else
	{
		log_error("TcpListen_函数调用错误");
	}

	return 0;
}
static int lua_udp(lua_State* lua)
{
	if (1 != lua_gettop(lua))
	{
		log_error("函数调用错误");
		return 0;
	}
	auto port = (int)lua_tointeger(lua, 1);
	Netbus::Instance()->UdpListen(port);

	return 0;
}
static int lua_websocket(lua_State* lua)
{
	auto cout = lua_gettop(lua);
	auto port = luaL_checkinteger(lua, 1);
	if (cout == 1)
	{
		Netbus::Instance()->WebSocketListen(port);
	}
	else if (cout == 2)
	{
		auto handle = toluafix_ref_function(lua, 2, 0);
		if (0 == handle)
			return 0;

		Netbus::Instance()->WebSocketListen(port, on_ws_listen, (void*)handle);
	}else
	{
		log_error("函数调用错误");
		return 0;
	}

	return 0;
}


#include<map>
#include<string>
using std::map;
using std::string;
void register_netbus_export(lua_State* tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1)) {
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Netbus", 0);
		tolua_beginmodule(tolua_S, "Netbus");

		tolua_function(tolua_S, "TcpListen", lua_tcp);
		tolua_function(tolua_S, "UdpListen", lua_udp);
		tolua_function(tolua_S, "WebsocketListen", lua_websocket);
		tolua_function(tolua_S, "TcpConnect", lua_tcp_connect);
		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
}
