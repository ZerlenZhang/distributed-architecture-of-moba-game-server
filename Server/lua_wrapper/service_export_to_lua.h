#ifndef __SERVICE_EXPORT_TO_LUA_H__
#define __SERVICE_EXPORT_TO_LUA_H__

#include "../netbus/service/AbstractService.h"


struct lua_State;
int register_service_export(lua_State* lua);


class LuaService :
	public AbstractService
{
public:
	unsigned int luaRecvCmdPackageHandle;
	unsigned int luaDisconnectFuncHandle;
	unsigned int luaRecvRawPackageHandle;

	//Session接收到RawPackage时调用
	virtual bool OnSessionRecvRawPackage(const AbstractSession* session, const RawPackage* package)const override;

	//Session接收到CmdPackage时调用
	virtual bool OnSessionRecvCmdPackage(const AbstractSession* session, const CmdPackage* package)const override;
	//Session关闭时调用
	virtual bool OnSessionDisconnected(const AbstractSession* session)const override;
};


#endif // !__SERVICE_EXPORT_TO_LUA_H__
