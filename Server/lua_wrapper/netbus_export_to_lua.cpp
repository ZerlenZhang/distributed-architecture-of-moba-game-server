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

// ip port lua_func
static int lua_tcp_connect(lua_State* lua)
{
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
	if (lua_gettop(lua) != 1)
		return 0;
	auto port = (int)lua_tointeger(lua, 1);
	Netbus::Instance()->TcpListen(port);

	return 0;
}
static int lua_udp(lua_State* lua)
{
	if (lua_gettop(lua) != 1)
		return 0;
	auto port = (int)lua_tointeger(lua, 1);
	Netbus::Instance()->UdpListen(port);

	return 0;
}
static int lua_websocket(lua_State* lua)
{
	if (lua_gettop(lua) != 1)
		return 0;
	auto port = (int)lua_tointeger(lua, 1);
	Netbus::Instance()->WebSocketListen(port);

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
