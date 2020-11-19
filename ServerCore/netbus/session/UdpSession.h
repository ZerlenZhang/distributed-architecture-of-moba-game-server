#ifndef __UDPSESSION_H__
#define __UDPSESSION_H__

#include "AbstractSession.h"

class UdpSession :
	public AbstractSession
{
private:
	uv_udp_t* udp_handler;
	const sockaddr* addr;

public:
	char clientAddress[64];
	int clientPort;


	void Init(uv_udp_t* udpHandle,const sockaddr* addr);

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
