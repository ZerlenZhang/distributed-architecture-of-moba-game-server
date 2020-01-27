#ifndef __UVSESSION_H__
#define __UVSESSION_H__

#include <uv.h>
#include "AbstractSession.h"

#define RECV_LEN 4096

enum class SocketType
{
	TcpSocket,
	WebSocket
};

class UvSession :
	public virtual AbstractSession
{
public:
	static UvSession* Create();
	static void Destory(UvSession*& session);

	// Í¨¹ý AbstractSession ¼Ì³Ð
	virtual void Close() override;
	virtual void SendData(unsigned char* body, int len) override;
	virtual const char* GetAddress(int & clientPort) const override;

	virtual void Enable()override;
	virtual void Disable()override;

public:
	uv_shutdown_t shutdown;
	uv_tcp_t tcpHandle;
	char clientAddress[32];
	int clientPort;

	char recvBuf[RECV_LEN];
	int recved; 

	SocketType socketType;

private:
	bool isShutDown;
};


void InitSessionAllocer();

#endif // !__UVSESSION_H__



