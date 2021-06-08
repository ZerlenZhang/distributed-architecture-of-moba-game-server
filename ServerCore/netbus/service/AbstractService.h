#ifndef __ABSTRACTSERVICE_H__
#define __ABSTRACTSERVICE_H__

class AbstractSession;
struct CmdPackage;
struct RawPackage;

//抽象服务类
class AbstractService
{
public:
	bool useRawPackage;

	AbstractService();

	virtual bool OnSessionRecvRawPackage(const AbstractSession* session, const RawPackage* package)const;

	//Session接收到命令时调用
	virtual bool OnSessionRecvCmdPackage(const AbstractSession* session, const CmdPackage* package)const;
	
	//Session断开某个服务的链接时调用关闭时调用
	virtual bool OnSessionDisconnected(const AbstractSession* session,const int & serviceType)const;

	//Session链接到否个服务的时候调用
	virtual void OnSessionConnected(const AbstractSession* session, const int& serviceType)const;

	virtual ~AbstractService() {}
};




#endif // !__ABSTRACTSERVICE_H__



