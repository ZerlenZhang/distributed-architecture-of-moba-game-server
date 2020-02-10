#include "Netbus.h"
#include <uv.h>
#include "../../netbus/protocol/WebSocketProtocol.h"
#include "../../netbus/protocol/TcpPackageProtocol.h"
#include "../../netbus/session/TcpSession.h"

#include "protocol/CmdPackageProtocol.h"
#include "service/ServiceManager.h"

#include "../utils/logger/logger.h"
#include "session/UdpSession.h"

#pragma region 函数声明

void OnRecvCommond(AbstractSession& session, unsigned char* body, const int len);

void OnWebSocketRecvData(TcpSession* session);

void OnTcpRecvData(TcpSession* session);

#pragma endregion

struct UdpRecvBuf
{
	char* data;
	int maxRecvLen;
};

#pragma region 回调函数

extern "C"
{
	#pragma region Tcp_&_Websocket
	//关闭链接回调
	static void tcp_close_cb(uv_handle_t* handle) {
		log_debug("用户断开链接");

		auto session = (TcpSession*)handle->data;
		TcpSession::Destory(session);
	}

	//断开链接的回调
	static void tcp_shutdown_cb(uv_shutdown_t* req, int status) {
		uv_close((uv_handle_t*)(req->handle), tcp_close_cb);
	}

	//Tcp申请字符串空间
	static void tcp_str_alloc(uv_handle_t* handle,
		size_t suggested_size,
		uv_buf_t* buf) {

		auto session = (TcpSession*)handle->data;

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
	static void tcp_after_read(uv_stream_t* stream,
		ssize_t nread,
		const uv_buf_t* buf) {

		auto session = (TcpSession*) stream->data;

		//连接断开
		if (nread < 0) {
			log_debug("链接断开");
			session->Close();
			return;
		}

		session->recved += nread;

		switch (session->socketType)
		{

		#pragma region Tcp协议

		case SocketType::TcpSocket:
			OnTcpRecvData(session);
			break;

		#pragma endregion

		#pragma region WebSocket协议	
		case SocketType::WebSocket: 

			if (session->isWebSocketShakeHand == 0)
			{	//	shakeHand
				if (WebSocketProtocol::ShakeHand(session, session->recvBuf, session->recved))
				{	//握手成功
					log_debug("握手成功");
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
	static void TcpOnConnect(uv_stream_t* server, int status)
	{
#pragma region 接入客户端

		auto session = TcpSession::Create();

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
		log_debug("new client comming:\t%s:%d", session->clientAddress, session->clientPort);

#pragma endregion


		//开始监听消息
		uv_read_start((uv_stream_t*)pNewClient, tcp_str_alloc, tcp_after_read);
#pragma endregion


	} 
	
	#pragma endregion

	#pragma region Udp

	//Udp申请字符串空间
	static void udp_str_alloc(uv_handle_t* handle,
		size_t suggested_size,
		uv_buf_t* buf)
	{
		suggested_size = (suggested_size < 8192) ? 8192 : suggested_size;

		auto pBuf = (UdpRecvBuf*)handle->data;
		if (pBuf->maxRecvLen < suggested_size)
		{// 表明当前空间不够
			if (pBuf->data)
			{
				free(pBuf->data);
				pBuf->data = NULL;
			}
			pBuf->data = (char*)malloc(suggested_size);
			pBuf->maxRecvLen = suggested_size;
		
		}

		buf->base = pBuf->data;
		buf->len = suggested_size;
	}

	//Udp接收字符串完成
	static void udp_after_recv(uv_udp_t* handle,
		ssize_t nread,
		const uv_buf_t* buf,
		const struct sockaddr* addr,
		unsigned flags)
	{
		UdpSession us;
		us.udp_handler = (uv_udp_t*)handle;
		us.addr = addr;
		us.clientPort = ntohs(((struct sockaddr_in*)addr)->sin_port);
		uv_ip4_name((struct sockaddr_in*)addr, us.clientAddress, 128);

		log_debug("ip: %s:%d nread = %d", us.clientAddress,us.clientPort, nread);

		OnRecvCommond(us,(unsigned char*)buf->base,buf->len);
	}

	#pragma endregion

	
}

#pragma endregion


static void OnRecvCommond(AbstractSession& session,unsigned char* body,const int len)
{
	//test
	log_debug("client command!!");

	CmdPackage* msg=NULL;
	if (CmdPackageProtocol::DecodeCmdMsg(body, len, msg))
	{
		if (!ServiceManager::OnRecvCmdPackage(&session, msg))
		{
			session.Close();
		}

		//释放数据
		CmdPackageProtocol::FreeCmdMsg(msg);
	}
	else
	{
		log_debug("解码失败");
	}
}


#pragma region WebSocketProtocol


static void OnWebSocketRecvData(TcpSession* session)
{
	auto pkgData = (unsigned char*)(session->long_pkg != NULL ? session->long_pkg : session->recvBuf);

	while (session->recved > 0)
	{
		//是否是关闭包
		if (pkgData[0] == 0x88) {
			log_debug("收到关闭包");
			session->Close();
			return;
		}

		int pkgSize;
		int headSize;
		if (!WebSocketProtocol::ReadHeader(pkgData, session->recved, &pkgSize, &headSize))
		{// 读取包头失败
			log_debug("读取包头失败");
			break;
		}

		if (session->recved < pkgSize)
		{// 没有收完数据
			log_debug("没有收完数据");
			break;
		}

		//掩码位置紧随头部数据之后
		//body位置在掩码之后
		unsigned char* body = pkgData + headSize;
		unsigned char* mask = body - 4;

		//解析收到的纯数据
		WebSocketProtocol::ParserRecvData(body, mask, pkgSize - headSize);

		//处理收到的完整数据包
		OnRecvCommond(*session, body, pkgSize);
	
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

#pragma region TcpProtocol

static void OnTcpRecvData(TcpSession* session)
{
	auto pkgData = (unsigned char*)(session->long_pkg != NULL ? session->long_pkg : session->recvBuf);

	while (session->recved > 0)
	{
		int pkgSize;
		int headSize;
		if (!TcpProtocol::ReadHeader(pkgData, session->recved, &pkgSize, &headSize))
		{// 读取包头失败
			log_debug("读取包头失败");
			break;
		}
		if (session->recved < pkgSize)
		{// 没有收完数据
			log_debug("没有收完数据");
			break;
		}

		//body位置在掩码之后
		unsigned char* body = pkgData + headSize;

		//处理收到的完整数据包
		OnRecvCommond(*session, body, pkgSize-headSize);

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
		log_debug("bind error");
		free(listen);
		listen = NULL;
		return;
	}

	//强转记录socket类型
	listen->data = (void*)SocketType::TcpSocket;

	uv_listen((uv_stream_t*)listen, SOMAXCONN, TcpOnConnect);
	log_debug("Tcp 服务器已开机 %d",port);
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
		log_debug("bind error");
		free(listen);
		listen = NULL;
		return;
	}

	//强转记录socket类型
	listen->data = (void*)SocketType::WebSocket;

	uv_listen((uv_stream_t*)listen, SOMAXCONN, TcpOnConnect);
	log_debug("WebSocket 服务器已开机 %d", port);
}

void Netbus::StartUdpServer(int port) const
{
	auto server = (uv_udp_t*)malloc(sizeof(uv_udp_t));
	memset(server, 0, sizeof(uv_udp_t));

	uv_udp_init(uv_default_loop(), server);

	server->data = malloc(sizeof(UdpRecvBuf));
	memset(server->data, 0, sizeof(UdpRecvBuf));

	sockaddr_in addr;
	uv_ip4_addr("0.0.0.0", port, &addr);
	auto ret = uv_udp_bind(server, (const sockaddr*)&addr, 0);

	uv_udp_recv_start(server, udp_str_alloc, udp_after_recv);
	log_debug("Udp 服务器已开机 %d", port);
}

void Netbus::Run()const
{
	log_debug("开始监听");
	uv_run(uv_default_loop(), UV_RUN_DEFAULT);
}

void Netbus::Init() const
{
	CmdPackageProtocol::Init();
	ServiceManager::Init();
	InitAllocers();
}
