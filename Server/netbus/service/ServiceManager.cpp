#include <cstring>
#include "AbstractService.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"
#include "../../netbus/session/AbstractSession.h"
#include "ServiceManager.h"
#define MAX_SERVICE_COUNT 64

static AbstractService* g_serviceSet[MAX_SERVICE_COUNT];

void ServiceManager::Init()
{
	memset(g_serviceSet, 0, sizeof(g_serviceSet));
}

bool ServiceManager::RegisterService(int serviceType, AbstractService* service)
{
	if (serviceType<0 || serviceType>MAX_SERVICE_COUNT)
	{// 越界
		return false;
	}

	if (g_serviceSet[serviceType] != NULL)
	{// 注册过
		return false;
	}

	g_serviceSet[serviceType] = service;

	return true;
}

bool ServiceManager::OnRecvCmdPackage(const AbstractSession* session, const CmdPackage* package)
{
	if (g_serviceSet[package->serviceType] == NULL)
	{// 没有注册过
		return false;
	}

	return g_serviceSet[package->serviceType]->OnSessionRecvCmd(session,package);
}

void ServiceManager::OnSessionDisconnected(const AbstractSession* session)
{
	for (auto service : g_serviceSet)
	{
		if (service != NULL)
		{
			continue;
		}
		service->OnSessionDisconnected(session);
	}
}
