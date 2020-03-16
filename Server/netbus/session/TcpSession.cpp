#include <uv.h>
#include "../../netbus/protocol/WebSocketProtocol.h"
#include "../../netbus/protocol/TcpPackageProtocol.h"
#include "../protocol/CmdPackageProtocol.h"
#include "../service/ServiceManager.h"
#include "../../utils/logger/logger.h"
#include "../../utils/cache_alloc/cache_alloc.h"
#include "TcpSession.h"


#pragma region 内存管理

#define SESSION_CACHE_CAPCITY 3000
#define	WRITEREQ_CACHE_CAPCITY 2048
#define WRITEBUF_CACHE_CAPCITY 1024
#define CMD_CACHE_SIZE 1024

//初始化内存分配器
static cache_allocer* sessionAllocer = NULL;
static cache_allocer* wrAllocer = NULL;
cache_allocer* writeBufAllocer = NULL;

void InitAllocers()
{
	if (NULL == sessionAllocer)
	{
		sessionAllocer = create_cache_allocer(SESSION_CACHE_CAPCITY, sizeof(TcpSession));
	}
	if (NULL == wrAllocer)
	{
		wrAllocer = create_cache_allocer(WRITEREQ_CACHE_CAPCITY, sizeof(uv_write_t));
	}
	if (NULL == writeBufAllocer)
	{
		writeBufAllocer = create_cache_allocer(WRITEBUF_CACHE_CAPCITY, CMD_CACHE_SIZE);
	}
}

#pragma endregion


#pragma region 回调函数
extern "C"
{

	static void ws_after_write(uv_write_t* req, int status)
	{
		//如果写请求成功
		if (status != 0)
		{
			log_error("websocket write failed");
		}
		WebSocketProtocol::ReleasePackage((unsigned char*)req->write_buffer.base);
		cache_free(wrAllocer, req);
	}
	//关闭链接回调
	static void close_cb(uv_handle_t* handle) {
		//log_debug("用户断开链接");

		auto session = (TcpSession*)handle->data;
		TcpSession::Destory(session);
	}

	//断开链接的回调
	static void shutdown_cb(uv_shutdown_t* req, int status) {
		if (status != 0)
		{
			log_error("tcp shutdown failed");
		}	
		uv_close((uv_handle_t*)(req->handle), close_cb);
	}

	//完成写请求的回调
	static void tcp_after_write(uv_write_t* req, int status)
	{
		//如果写请求成功
		if (status != 0)
		{
			log_error("tcp write failed");
		}
		TcpProtocol::ReleasePackage((unsigned char*)req->write_buffer.base);
		cache_free(wrAllocer, req);
	}
}
#pragma endregion


#pragma region Static

TcpSession* TcpSession::Create()
{
	auto temp = new TcpSession();
	temp->Enable();
	return temp;
}

void TcpSession::Destory(TcpSession*& session)
{
	session->Disable();
	delete session;
	session = NULL;
}

#pragma endregion

void TcpSession::Init(SocketType socketType)
{
	this->socketType = socketType;

	#pragma region 获取接入者IP和端口
	
	sockaddr_in addr;
	int len = sizeof(addr);
	uv_tcp_getpeername(&this->tcpHandle, (sockaddr*)&addr, &len);
	uv_ip4_name(&addr, this->clientAddress, sizeof(this->clientAddress));
	this->clientPort = ntohs(addr.sin_port);

	#pragma endregion

	//加入事件循环
	uv_tcp_init(uv_default_loop(), &this->tcpHandle);
}
void TcpSession::Init(SocketType socketType, const char* ip, int port, bool isClient)
{
	this->isClient = isClient;
	this->socketType = socketType;
	strcpy(this->clientAddress, ip);
	this->clientPort = port;

	//加入事件循环
	uv_tcp_init(uv_default_loop(), &this->tcpHandle);
}
void* TcpSession::operator new(size_t size)
{
	return cache_alloc(sessionAllocer, sizeof(TcpSession));
}

void TcpSession::operator delete(void* mem)
{
	cache_free(sessionAllocer, mem);
}

#pragma region Implement

void TcpSession::Close()
{
	if (this->isShutDown) {
		return;
	}

	//通知所有父物有链接断开
	ServiceManager::OnSessionDisconnected(this);


	this->isShutDown = true;
	memset(&this->shutdown, 0, sizeof(uv_shutdown_t));
	auto ret = uv_shutdown(&this->shutdown, (uv_stream_t*)&this->tcpHandle, shutdown_cb);

	if (ret)
	{
		uv_close((uv_handle_t*)&this->tcpHandle, close_cb);
	}
}

void TcpSession::SendData(unsigned char* body, int len)
{
	//测试发送给我们的客户端
	auto w_req = (uv_write_t*)cache_alloc(wrAllocer, sizeof(uv_write_t));
	uv_buf_t w_buf;
	switch (this->socketType)
	{

	#pragma region WebSocket协议
	case SocketType::WebSocket:
		if (this->isWebSocketShakeHand)
		{// 握过手
			int pkgSize;
			auto wsPkg = WebSocketProtocol::Package(body, len, &pkgSize);
			w_buf = uv_buf_init((char*)wsPkg, pkgSize);
			w_req->write_buffer.base = (char*)wsPkg;
			w_req->write_buffer.len = pkgSize;
			uv_write(w_req, (uv_stream_t*)&this->tcpHandle, &w_buf, 1, ws_after_write);
		}
		else
		{// 没有握过手
			static char respones[512];
			memcpy(respones, body, len);
			w_buf = uv_buf_init((char*)respones, len);
			uv_write(w_req, (uv_stream_t*)&this->tcpHandle, &w_buf, 1, NULL);
		}
		break;
	#pragma endregion

	#pragma region Tcp协议
	case SocketType::TcpSocket:
		int pkgSize;
		auto tcpPkg = TcpProtocol::Package(body, len, &pkgSize);
		w_buf = uv_buf_init((char*)tcpPkg, pkgSize);
		w_req->write_buffer.base = (char*)tcpPkg;
		w_req->write_buffer.len = pkgSize;
		uv_write(w_req, (uv_stream_t*)&this->tcpHandle, &w_buf, 1, tcp_after_write);
		break;
	#pragma endregion

	}
}

const char* TcpSession::GetAddress(int& clientPort) const
{
	clientPort = this->clientPort;
	return this->clientAddress;
}

void TcpSession::SendCmdPackage(CmdPackage* msg)
{
	int bodyLen;
	auto rawData = CmdPackageProtocol::EncodeCmdPackageToBytes(msg, &bodyLen);
	if (rawData)
	{// 编码成功 

		//发送数据
		SendData(rawData, bodyLen);

		//释放数据
		CmdPackageProtocol::FreeCmdPackageBytes(rawData);
	}
	else
	{
		log_debug("编码失败");
	}
}

#pragma endregion




#pragma region Override

void TcpSession::Enable()
{
	AbstractSession::Enable();
	isShutDown = false;
	memset(&this->shutdown, 0, sizeof(this->shutdown));
	memset(&this->tcpHandle, 0, sizeof(this->tcpHandle));
	memset(this->clientAddress, 0, sizeof(this->clientAddress));
	this->clientPort = 0;
	this->recved = 0;
	this->isWebSocketShakeHand = 0;
	this->long_pkg = NULL;
	this->long_pkg_size = 0;

	this->tcpHandle.data = this;
}

void TcpSession::Disable()
{
	AbstractSession::Disable();
}



void TcpSession::SendRawPackage(RawPackage* pkg)
{
	this->SendData(pkg->body, pkg->rawLen);
}

#pragma endregion
