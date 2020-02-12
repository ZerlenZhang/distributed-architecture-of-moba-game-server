#ifndef __NETBUS_H__
#define __NETBUS_H__






class Netbus
{
public:
	//µ¥Àý
	static const Netbus* Instance();
public:
	void TcpListen(int port)const;
	void WebSocketListen(int port)const;
	void UdpListen(int port)const;
	void Run()const;
	void Init()const;
};








#endif // !__NETBUS_H__

