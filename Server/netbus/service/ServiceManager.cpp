#include <cstring>
#include "AbstractService.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"
#include "../../netbus/session/AbstractSession.h"
#include "ServiceManager.h"
#include "../../utils/logger/logger.h"
#include <map>
using std::map;

static map<int, AbstractService*> serviceMap;

void ServiceManager::Init()
{
	//memset(g_serviceSet, 0, sizeof(g_serviceSet));
}

bool ServiceManager::RegisterService(int serviceType, AbstractService* service)
{
	if (serviceMap.find(serviceType)!=serviceMap.end())
	{// 注册过
		log_warning("服务注册失败——重复：%d", serviceType);
		return false;
	}
	serviceMap[serviceType] = service;
	return true;
}

bool ServiceManager::OnRecvRawPackage(AbstractSession* session, const RawPackage* raw)
{
	if (NULL == raw)
	{
		log_warning("RawPackage包为空");
		return false;
	}
	if (serviceMap.find(raw->serviceType)==serviceMap.end())
	{// 没有注册过
		log_warning("未注册的serviceType: %d", raw->serviceType);
		return false;
	}

	//是否使用RawPackage
	if (serviceMap[raw->serviceType]->useRawPackage)
	{
		return serviceMap[raw->serviceType]->OnSessionRecvRawPackage(session, raw);
	}

	//使用CmdPackage
	CmdPackage* cmdPackage;

	//从字符串中解析出CmdPackage
	if (CmdPackageProtocol::DecodeBytesToCmdPackage(raw->body, raw->rawLen, cmdPackage))
	{
		//根据包的serviceType调用对应的服务去处理这个包
		auto ret = serviceMap[raw->serviceType]->OnSessionRecvCmdPackage(session, cmdPackage);
		if (!ret)
		{// 如果有服务返回false，就关闭session
			session->Close();
		}

		CmdPackageProtocol::FreeCmdPackage(cmdPackage);
		return ret;
	}

	//解析失败
	return false;
}

void ServiceManager::OnSessionDisconnected(const AbstractSession* session)
{
	for (auto kv : serviceMap)
	{
		if (kv.second == NULL)
			continue;
		kv.second->OnSessionDisconnected(session, kv.first);
	}
}

void ServiceManager::OnSessionConnect(const AbstractSession* session)
{
	for (auto kv : serviceMap)
	{
		if (kv.second == NULL)
			continue;
		kv.second->OnSessionConnected(session,kv.first);
	}
}
