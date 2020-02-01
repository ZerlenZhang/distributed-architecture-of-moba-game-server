#ifndef __ABSTRACTSERVICE_H__
#define __ABSTRACTSERVICE_H__

class AbstractSession;
struct CmdPackage;

//抽象服务类
class AbstractService
{
public:
	//Session接收到命令时调用
	virtual bool OnSessionRecvCmd(const AbstractSession* session, const CmdPackage* package)const;
	//Session关闭时调用
	virtual bool OnSessionDisconnected(const AbstractSession* session)const;
};




#endif // !__ABSTRACTSERVICE_H__



