#ifndef __ABSTRACTSESSION_H__
#define __ABSTRACTSESSION_H__
struct CmdPackage;
class AbstractSession
{
public:
	unsigned int isClient;
	unsigned int utag;

	AbstractSession();

	virtual void Close() = 0;
	virtual void SendData(unsigned char* body,int len) = 0;
	virtual const char* GetAddress(int & clientPort)const = 0;
	virtual void SendCmdPackage(CmdPackage* msg) = 0;

	virtual void Enable();
	virtual void Disable();
};




#endif // !__ABSTRACTSESSION_H__



