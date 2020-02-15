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
	//Session关闭时调用
	virtual bool OnSessionDisconnected(const AbstractSession* session)const;
};




#endif // !__ABSTRACTSERVICE_H__



