#ifndef __UDPSESSION_H__
#define __UDPSESSION_H__

#include "AbstractSession.h"

class UdpSession :
	public AbstractSession
{
public:
	uv_udp_t* udp_handler;
	char clientAddress[32];
	int clientPort;
	const struct sockaddr* addr;

	virtual void Close() override;
	//发送数据
	virtual void SendData(unsigned char* body, int len) override;
	//获取IP地址和端口
	virtual const char* GetAddress(int& clientPort) const override;
	//发送自定义包
	virtual void SendCmdPackage(CmdPackage* msg)override;

	// 通过 AbstractSession 继承
	virtual void SendRawPackage(RawPackage* pkg) override;
};


#endif // !__UDPSESSION_H__
