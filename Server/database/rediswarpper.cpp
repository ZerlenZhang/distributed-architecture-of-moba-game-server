
#include <HiredisWrapper.h>
#include <uv.h>
#include "rediswarpper.h"
#include "../utils/logger/logger.h"

#include "../utils/cache_alloc/small_alloc.h"

#define my_alloc small_alloc
#define my_free small_free

static char* my_strdup(const char* src)
{
	auto len = strlen(src) + 1;
	auto dst = (char*)my_alloc(len);
	strcpy(dst, src);
	return dst;
}

static void free_my_strdup(char* str)
{
	my_free(str);
}

#pragma region 信息结构体



//connect必须信息
struct connect_req
{
	char* ip;
	int port;

	RedisConnectCallback open_cb;

	char* error;
	RedisContext* context;
};

//query必须信息
struct query_req
{
	RedisContext* context;
	char* cmd;
	RedisQueryCallback query_cb;
	char* error;
	RedisReply* myReply;
};



#pragma endregion

#pragma region 回调

static void connect_work(uv_work_t* req) 
{
	auto pInfo = (connect_req*)req->data;
	timeval timeout = { 5.0};
	pInfo->context->pConn = RedisConnectWithTimeout(pInfo->ip, pInfo->port, timeout);
	if (pInfo->context->pConn->err)
	{
		pInfo->error = my_strdup(pInfo->context->pConn->errstr);
		RedisFree(pInfo->context->pConn);
		pInfo->context->pConn = NULL;
		return;
	}
}

static void on_connect_complete(uv_work_t* req, int status)
{
	auto pInfo = (connect_req*)req->data;

	log_debug("Redis 数据库链接成功");

	if (pInfo)
	{
		if (pInfo->open_cb)
			pInfo->open_cb(pInfo->error, pInfo->context);

		if (pInfo->ip)
			free_my_strdup(pInfo->ip);
		if (pInfo->error)
			free_my_strdup(pInfo->error);

		my_free(pInfo);
	}

	my_free(req);
}

static void close_work(uv_work_t* req)
{
	auto pConn = (RedisContext*)req->data;

	//加锁，等待正在进行的查询结束
	uv_mutex_lock(&pConn->lock);
	//log_debug("close 加锁");
	RedisFree(pConn->pConn);
	pConn->pConn = NULL;
	//log_debug("close 释放");
	uv_mutex_unlock(&pConn->lock);
}

static void on_close_complete(uv_work_t* req, int status)
{
	auto info = (RedisContext*)req->data;

	if (info)
	{
		if (info->udata && info->autoFreeUdata)
			free(info->udata);
		my_free(info);

	}
	my_free(req);
	log_debug("Redis 数据库断开链接");
}

static void query_work(uv_work_t* req)
{
	auto pInfo = (query_req*)req->data;

	if (pInfo->context->isClosed)
	{// 已经关闭此链接，就不要再操作了
		log_debug("链接已经关闭，查询无效");
		return;
	}
	//线程锁
	uv_mutex_lock(&pInfo->context->lock);
	//log_debug("query 加锁");

	auto replay = (redisReply*)RedisCommand(pInfo->context->pConn, pInfo->cmd);
	
	if (replay)
	{
		if (replay->type == REDIS_REPLY_ERROR)
		{
			pInfo->error = my_strdup(replay->str);
			FreeReplyObject(replay);
		}
		else
		{
			pInfo->myReply->reply = replay;
		}

	}
	//log_debug("query 释放");
	uv_mutex_unlock(&pInfo->context->lock);

}

static void on_query_complete(uv_work_t* req, int status)
{
	auto pInfo = (query_req*)req->data;

	if (pInfo)
	{
		if(pInfo->query_cb)
			pInfo->query_cb(pInfo->error, pInfo->myReply);

		if (pInfo->cmd)
			free_my_strdup(pInfo->cmd);
		if (pInfo->error)
			free_my_strdup(pInfo->error);

		if (pInfo->myReply)
		{
			if(pInfo->myReply->reply)
				FreeReplyObject(pInfo->myReply->reply);

			if (pInfo->myReply->udata && pInfo->myReply->autoFreeUdata)
				free(pInfo->myReply->udata);
			my_free(pInfo->myReply);
		}
		my_free(pInfo);
	}
	my_free(req);
}

#pragma endregion

#pragma region redis_wrapper
void redis_wrapper::connect(char* ip, int port, RedisConnectCallback callback,void* udata,bool autoFreeUdata)
{
	auto w = (uv_work_t * )my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	auto info = (connect_req*)my_alloc(sizeof(connect_req));
	memset(info, 0, sizeof(connect_req)); 

	auto lockContext = (RedisContext*)my_alloc(sizeof(RedisContext));
	memset(lockContext, 0, sizeof(RedisContext));
	uv_mutex_init(&lockContext->lock);//初始化信号量
	lockContext->udata = udata;
	lockContext->autoFreeUdata = autoFreeUdata;

	info->ip = my_strdup(ip);
	info->port = port;
	info->open_cb = callback;
	info->context = lockContext;

	w->data = info;

	uv_queue_work(uv_default_loop(), w, connect_work, on_connect_complete);
}

void redis_wrapper::close_redis(RedisContext* conntext)
{
	if (conntext->isClosed)
	{// 已经关了
		return;
	}


	auto w = (uv_work_t*)my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	conntext->isClosed = true;

	w->data = conntext;

	uv_queue_work(uv_default_loop(), w, close_work, on_close_complete);
} 

void redis_wrapper::query(RedisContext* context, char* sql, RedisQueryCallback callback,void* udata,bool autoFreeUdata)
{
	auto w = (uv_work_t*)my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	auto pInfo = (query_req*)my_alloc(sizeof(query_req));
	memset(pInfo, 0, sizeof(query_req));

	auto myReply = (RedisReply*)my_alloc(sizeof(RedisReply));
	memset(myReply, 0, sizeof(RedisReply));
	myReply->udata = udata;
	myReply->autoFreeUdata = autoFreeUdata;

	pInfo->context = context;
	pInfo->cmd = my_strdup(sql);
	pInfo->query_cb = callback;
	pInfo->myReply = myReply;

	w->data = pInfo;

	uv_queue_work(uv_default_loop(), w, query_work, on_query_complete);

}

#pragma endregion


