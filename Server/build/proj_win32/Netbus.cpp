#include "Netbus.h"
#include <uv.h>
#include "UvSession.h"
#include "WebSocketProtocol.h"

void OnWebSocketRecvCommond(UvSession* session, unsigned char* body, int len);
void OnWebSocketRecvData(UvSession* session);


#pragma region 回调函数

extern "C"
{
	//关闭链接回调
	static void close_cb(uv_handle_t* handle) {
		printf("用户断开链接\n");

		auto session = (UvSession*)handle->data;
		UvSession::Destory(session);
	}

	//断开链接的回调
	static void shutdown_cb(uv_shutdown_t* req, int status) {
		uv_close((uv_handle_t*)(req->handle), close_cb);
	}

	//申请字符串空间
	static void string_alloc(uv_handle_t* handle,
		size_t suggested_size,
		uv_buf_t* buf) {

		auto session = (UvSession*)handle->data;

		if (session->recved < RECV_LEN)
		{
			//session的长度为 RECV_LEN 的recvBuf还没有存满
			*buf = uv_buf_init(session->recvBuf + session->recved,RECV_LEN - session->recved); 
		}
		else
		{// recvBuf读满了，但是还没有读完 

		
			if (session->long_pkg == NULL)
			{// 如果还没有new空间

				switch (session->socketType)
				{
				case SocketType::TcpSocket:
					break;
				#pragma region WebSocket协议
				case SocketType::WebSocket:
					
					if (session->isWebSocketShakeHand)
					{// 握过手
						int pkgSize;
						int headSize;
						WebSocketProtocol::ReadHeader((unsigned char*)session->recvBuf, session->recved, &pkgSize, &headSize);
					
						session->long_pkg_size = pkgSize;
						session->long_pkg = (char*)malloc(pkgSize);
					
						memcpy(session->long_pkg, session->recvBuf, session->recved);
					}
					break;
				#pragma endregion

				}
			}

			*buf = uv_buf_init(session->long_pkg + session->recved, session->long_pkg_size - session->recved);
		}
		
	}

	//读取结束后的回调
	static void after_read(uv_stream_t* stream,
		ssize_t nread,
		const uv_buf_t* buf) {

		auto session = (UvSession*) stream->data;

		//连接断开
		if (nread < 0) {
			printf("链接断开\n");
			session->Close();
			return;
		}

		session->recved += nread;

		switch (session->socketType)
		{
		case SocketType::TcpSocket:
			break;

		#pragma region WebSocket协议	
		case SocketType::WebSocket: 

			if (session->isWebSocketShakeHand == 0)
			{	//	shakeHand
				if (WebSocketProtocol::ShakeHand(session, session->recvBuf, session->recved))
				{	//握手成功
					printf("握手成功\n");
					session->isWebSocketShakeHand = 1;
					session->recved = 0;
				}
			}
			else//	recv/send Data
			{
				OnWebSocketRecvData(session);
			}
			break;
		#pragma endregion


		default:
			break;
		}
	}

	//TCP有用户链接进来
	static void OnConnect(uv_stream_t* server, int status)
	{
#pragma region 接入客户端

		auto session = UvSession::Create();

		auto pNewClient = &session->tcpHandle;
		pNewClient->data = session;

		//将新客户端TCP句柄也加入到事件循环中
		uv_tcp_init(uv_default_loop(), pNewClient);
		//客户端接入服务器 
		uv_accept(server,(uv_stream_t*) pNewClient);

#pragma region 获取接入者IP和端口

		sockaddr_in addr;
		int len = sizeof(addr);
		uv_tcp_getpeername(pNewClient, (sockaddr*)&addr, &len);
		uv_ip4_name(&addr, session->clientAddress, 64);
		session->clientPort = ntohs(addr.sin_port);
		//保存socket类型
		session->socketType = *((SocketType*)&server->data);
		printf("new client comming:\t%s:%d\n", session->clientAddress, session->clientPort);

#pragma endregion


		//开始监听消息
		uv_read_start((uv_stream_t*)pNewClient, string_alloc, after_read);
#pragma endregion


	} 
}

#pragma endregion


#pragma region WebSocket函数

static void OnWebSocketRecvCommond(UvSession* session,unsigned char* body,int len)
{
	printf("client command!!\n");

	//test
	session->SendData(body, len);
}

static void OnWebSocketRecvData(UvSession* session)
{
	auto pkgData = (unsigned char*)(session->long_pkg != NULL ? session->long_pkg : session->recvBuf);

	while (session->recved > 0)
	{
		//是否是关闭包
		if (pkgData[0] == 0x88) {
			session->Close();
			return;
		}

		int pkgSize;
		int headSize;
		if (!WebSocketProtocol::ReadHeader(pkgData, session->recved, &pkgSize, &headSize))
		{// 读取包头失败
			printf("读取包头失败\n");
			break;
		}

		if (session->recved < pkgSize)
		{// 没有收完数据
			printf("没有收完数据\n");
			break;
		}

		//掩码位置紧随头部数据之后
		//body位置在掩码之后
		unsigned char* body = pkgData + headSize;
		unsigned char* mask = body - 4;

		//解析收到的纯数据
		WebSocketProtocol::ParserRecvData(body, mask, pkgSize);

		//处理收到的完整数据包
		OnWebSocketRecvCommond(session, body, pkgSize);
	
		if (session->recved > pkgSize)
		{
			memmove(pkgData, pkgData + pkgSize, session->recved - pkgSize);
		}

		//每次减去读取到的长度
		session->recved -= pkgSize;

		//如果长包处理完了
		if (session->recved == 0 && session->long_pkg != NULL)
		{
			free(session->long_pkg);
			session->long_pkg = NULL;
			session->long_pkg_size = 0;
		}
	}
}

#pragma endregion



static Netbus g_instance;
const Netbus* Netbus::Instance()
{
	return &g_instance;
}

void Netbus::StartTcpServer(int port)const
{
	auto listen = (uv_tcp_t*)malloc(sizeof(uv_tcp_t));

	memset(listen, 0, sizeof(uv_tcp_t));

	sockaddr_in addr;
	uv_ip4_addr("0.0.0.0", port, &addr);
	uv_tcp_init(uv_default_loop(), listen);
	auto ret = uv_tcp_bind(listen, (const sockaddr*)&addr, 0);
	if (0 != ret)
	{
		printf("bind error\n");
		free(listen);
		listen = NULL;
		return;
	}

	//强转记录socket类型
	listen->data = (void*)SocketType::TcpSocket;

	uv_listen((uv_stream_t*)listen, SOMAXCONN, OnConnect);
	printf("Tcp 服务器已开机\n");
}

void Netbus::StartWebSocketServer(int port)const
{
	auto listen = (uv_tcp_t*)malloc(sizeof(uv_tcp_t));

	memset(listen, 0, sizeof(uv_tcp_t));

	sockaddr_in addr;
	uv_ip4_addr("0.0.0.0", port, &addr);
	uv_tcp_init(uv_default_loop(), listen);
	auto ret = uv_tcp_bind(listen, (const sockaddr*)&addr, 0);
	if (0 != ret)
	{
		printf("bind error\n");
		free(listen);
		listen = NULL;
		return;
	}

	//强转记录socket类型
	listen->data = (void*)SocketType::WebSocket;

	uv_listen((uv_stream_t*)listen, SOMAXCONN, OnConnect);
	printf("Tcp 服务器已开机\n");
}

void Netbus::Run()const
{
	printf("开始监听\n");
	uv_run(uv_default_loop(), UV_RUN_DEFAULT);
}

void Netbus::Init() const
{
	InitSessionAllocer();
}
