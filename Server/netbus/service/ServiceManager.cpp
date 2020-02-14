#include <cstring>
#include "AbstractService.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"
#include "../../netbus/session/AbstractSession.h"
#include "ServiceManager.h"
#include "../../utils/logger/logger.h"
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
		log_warning("服务注册失败——越界：%d", serviceType);
		return false;
	}

	if (g_serviceSet[serviceType] != NULL)
	{// 注册过
		log_warning("服务注册失败——重复：%d", serviceType);
		return false;
	}

	g_serviceSet[serviceType] = service;

	return true;
}

bool ServiceManager::OnRecvCmdPackage(const AbstractSession* session, const CmdPackage* package)
{
	if (NULL == package)
	{
		log_warning("CmdPackage 包为空");
		return false;
	}
	if (g_serviceSet[package->serviceType] == NULL)
	{// 没有注册过
		log_warning("未注册的serviceType: %d", package->serviceType);
		return false;
	}

	return g_serviceSet[package->serviceType]->OnSessionRecvCmd(session,package);
}

void ServiceManager::OnSessionDisconnected(const AbstractSession* session)
{
	for (auto service : g_serviceSet)
	{
		if (service == NULL)
		{
			continue;
		}
		service->OnSessionDisconnected(session);
	}
}
