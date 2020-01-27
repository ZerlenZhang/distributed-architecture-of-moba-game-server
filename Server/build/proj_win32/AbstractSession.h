#ifndef __ABSTRACTSESSION_H__
#define __ABSTRACTSESSION_H__



class AbstractSession
{
public:
	virtual void Close() = 0;
	virtual void SendData(unsigned char* body,int len) = 0;
	virtual const char* GetAddress(int & clientPort)const = 0;

	virtual void Enable();
	virtual void Disable();
};




#endif // !__ABSTRACTSESSION_H__



