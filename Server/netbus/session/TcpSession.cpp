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
	//关闭链接回调
	static void close_cb(uv_handle_t* handle) {
		log_debug("用户断开链接");

		auto session = (TcpSession*)handle->data;
		TcpSession::Destory(session);
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
			log_debug("write success");
		}
		cache_free(wrAllocer, req);
	}
}
#pragma endregion


#pragma region Static

TcpSession* TcpSession::Create()
{
	//手动构造
	auto temp = (TcpSession*)cache_alloc(sessionAllocer, sizeof(TcpSession));
	temp->TcpSession::TcpSession();
	temp->Enable();
	return temp;
}

void TcpSession::Destory(TcpSession*& session)
{
	session->Disable();
	//手动析构
	session->TcpSession::~TcpSession();
	cache_free(sessionAllocer, session);

	session = NULL;

}

#pragma endregion


#pragma region Implement

void TcpSession::Close()
{
	if (this->isShutDown) {
		return;
	}

	log_debug("主动关机");

	//通知所有父物有链接断开
	ServiceManager::OnSessionDisconnected(this);


	this->isShutDown = true;
	uv_shutdown(&this->shutdown, (uv_stream_t*)&this->tcpHandle, shutdown_cb);
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

const char* TcpSession::GetAddress(int& clientPort) const
{
	clientPort = this->clientPort;
	return this->clientAddress;
}

void TcpSession::SendCmdPackage(CmdPackage* msg)
{
	int bodyLen;
	auto rawData = CmdPackageProtocol::EncodeCmdPackageToRaw(msg, &bodyLen);
	if (rawData)
	{// 编码成功 

		//发送数据
		SendData(rawData, bodyLen);

		//释放数据
		CmdPackageProtocol::FreeCmdPackageRaw(rawData);
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
}

void TcpSession::Disable()
{
	AbstractSession::Disable();
}

#pragma endregion
