#ifndef __SERVICEMANAGER_H__
#define __SERVICEMANAGER_H__
#endif // !__SERVICEMANAGER_H__

class ServiceManager
{
public:
	static void Init();
	//注册服务
	static bool RegisterService(int serviceType, AbstractService* service);
	//Netbus收到包的回调，返回true表示正常处理了，否则要关闭socket
	static bool OnRecvCmdPackage(const AbstractSession* session, const CmdPackage* package);
	//客户端断开的回调
	static void OnSessionDisconnected(const AbstractSession* session);
};

