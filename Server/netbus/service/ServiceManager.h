#ifndef __SERVICEMANAGER_H__
#define __SERVICEMANAGER_H__
#endif // !__SERVICEMANAGER_H__
class AbstractService;
struct RawPackage;
class ServiceManager
{
public:
	static void Init();
	//注册服务
	static bool RegisterService(int serviceType, AbstractService* service);
	//Netbus收到包的回调，返回true表示正常处理了，否则要关闭socket
	static bool OnRecvRawPackage(AbstractSession* session, const RawPackage* package);
	//客户端断开的回调
	static void OnSessionDisconnected(const AbstractSession* session);

	//客户端链接成功回调
	static void OnSessionConnect(const AbstractSession* session);
};

