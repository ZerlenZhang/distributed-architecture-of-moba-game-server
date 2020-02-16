#ifndef __NETBUS_H__
#define __NETBUS_H__

#include <string>
using std::string;

#include "protocol/CmdPackageProtocol.h"




class AbstractSession;
typedef void (*TcpConnectedCallback)(int, AbstractSession*, void*);
typedef void (*TcpListenCallback)(AbstractSession*, void*);

class Netbus
{
public:
	//µ¥Àý
	static const Netbus* Instance();
public:
	void TcpListen(int port, TcpListenCallback callback=0,void* udata=0)const;
	void WebSocketListen(int port, TcpListenCallback callback = 0, void* udata = 0)const;
	void UdpListen(int port)const;
	void Run()const;
	void Init()const;

	void TcpConnect(const char* serverIp, 
		int port, 
		TcpConnectedCallback callback=0,
		void* udata=0)const;
};








#endif // !__NETBUS_H__

