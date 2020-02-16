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

bool ServiceManager::OnRecvCmd(const AbstractSession* session, const RawPackage* raw)
{
	if (NULL == raw)
	{
		log_warning("CmdRaw 包为空");
		return false;
	}
	if (g_serviceSet[raw->serviceType] == NULL)
	{// 没有注册过
		log_warning("未注册的serviceType: %d", raw->serviceType);
		return false;
	}

	//是否使用原始数据
	if (g_serviceSet[raw->serviceType]->useRawPackage)
	{
		return g_serviceSet[raw->serviceType]->OnSessionRecvRawPackage(session, raw);
	}

	//使用包数据
	CmdPackage* pk;
	if (CmdPackageProtocol::DecodeBytesToCmdPackage(raw->rawCmd, raw->rawLen, pk))
	{
		auto ret = g_serviceSet[raw->serviceType]->OnSessionRecvCmdPackage(session, pk);
		CmdPackageProtocol::FreeCmdPackage(pk);
		return ret;
	}


	return false;
}

void ServiceManager::OnSessionDisconnected(const AbstractSession* session)
{
	auto index = -1;
	for (auto service : g_serviceSet)
	{
		index++;
		if (service == NULL)
		{
			continue;
		}
		service->OnSessionDisconnected(session,index);
	}
}
