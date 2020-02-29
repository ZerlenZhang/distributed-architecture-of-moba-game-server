#ifndef __UVSESSION_H__
#define __UVSESSION_H__

#include "AbstractSession.h"
#define RECV_LEN 4096

//Socket类型
enum class SocketType
{
	TcpSocket,
	WebSocket
};


class TcpSession :
	public AbstractSession
{
public:
	static TcpSession* Create();
	static void Destory(TcpSession*& session);

	#pragma region AbstractSession

	virtual void Close() override;
	//发送数据
	virtual void SendData(unsigned char* body, int len) override;
	//获取IP地址和端口
	virtual const char* GetAddress(int & clientPort) const override;
	//发送自定义包
	virtual void SendCmdPackage(CmdPackage* msg)override;

	// 通过 AbstractSession 继承
	virtual void SendRawPackage(RawPackage* pkg) override;

	//用于对象池
	virtual void Enable()override;
	virtual void Disable()override;
	#pragma endregion


	void* operator new(size_t size);
	void operator delete(void* mem);

public:
	uv_shutdown_t shutdown;
	uv_tcp_t tcpHandle;
	//记录当前会话对象的IP和端口
	char clientAddress[32];
	int clientPort;

	//控制结束数据
	char recvBuf[RECV_LEN];
	int recved; 


	SocketType socketType;

	//WebSocket情况下是否已经握手
	int isWebSocketShakeHand;

	// 如果包体大小超过 RECV_LEN 包数据就保存在这里，而不是recvData
	char* long_pkg;
	int long_pkg_size;
private:
	//是否已经关闭，用于在异步时避免重复关闭
	bool isShutDown;

};


void InitAllocers();

#endif // !__UVSESSION_H__



