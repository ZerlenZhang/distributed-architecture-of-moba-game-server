#ifndef __NETBUS_H__
#define __NETBUS_H__






class Netbus
{
public:
	//µ¥Àý
	static const Netbus* Instance();
public:
	void StartTcpServer(int port)const;
	void StartWebSocketServer(int port)const;
	void Run()const;
	void Init()const;
};








#endif // !__NETBUS_H__

