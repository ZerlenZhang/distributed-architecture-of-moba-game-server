#include "UvSession.h"
#include "../../utils/cache_alloc.h"
#include "../../netbus/protocol/WebSocketProtocol.h"
#include "../../netbus/protocol/TcpPackageProtocol.h"

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
		sessionAllocer = create_cache_allocer(SESSION_CACHE_CAPCITY, sizeof(UvSession));
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

	//完成写请求的回调
	static void after_write(uv_write_t* req, int status)
	{
		//如果写请求成功
		if (status == 0)
		{
			printf("write success\n");
		}
		cache_free(wrAllocer, req);
	}
}
#pragma endregion


#pragma region Static

UvSession* UvSession::Create()
{
	//手动构造
	auto temp = (UvSession*)cache_alloc(sessionAllocer, sizeof(UvSession));
	temp->UvSession::UvSession();
	temp->Enable();
	return temp;
}

void UvSession::Destory(UvSession*& session)
{
	session->Disable();
	//手动析构
	session->UvSession::~UvSession();
	cache_free(sessionAllocer, session);

	session = NULL;

}

#pragma endregion


#pragma region Implement

void UvSession::Close()
{
	if (this->isShutDown) {
		return;
	}
	this->isShutDown = true;
	printf("主动关机\n");
	uv_shutdown(&this->shutdown, (uv_stream_t*)&this->tcpHandle, shutdown_cb);
}

void UvSession::SendData(unsigned char* body, int len)
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
			uv_write(w_req, (uv_stream_t*)&this->tcpHandle, &w_buf, 1, after_write);
			WebSocketProtocol::ReleasePackage(wsPkg);
		}
		else
		{// 没有握过手
			w_buf = uv_buf_init((char*)body, len);
			uv_write(w_req, (uv_stream_t*)&this->tcpHandle, &w_buf, 1, after_write);
		}
		break;
	#pragma endregion

	#pragma region Tcp协议
	case SocketType::TcpSocket:
		int pkgSize;
		auto tcpPkg = TcpProtocol::Package(body, len, &pkgSize);
		w_buf = uv_buf_init((char*)tcpPkg, pkgSize);
		uv_write(w_req, (uv_stream_t*)&this->tcpHandle, &w_buf, 1, after_write);
		TcpProtocol::ReleasePackage(tcpPkg);
		break;
	#pragma endregion

	}


}

const char* UvSession::GetAddress(int& clientPort) const
{
	clientPort = this->clientPort;
	return this->clientAddress;
}

#pragma endregion



#pragma region Override

void UvSession::Enable()
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
}

void UvSession::Disable()
{
	AbstractSession::Disable();
}

#pragma endregion


