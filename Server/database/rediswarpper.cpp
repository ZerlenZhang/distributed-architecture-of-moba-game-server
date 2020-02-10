
#include <HiredisWrapper.h>
#include <uv.h>
#include "rediswarpper.h"
#include "../utils/logger/logger.h"

#define my_alloc malloc
#define my_free free

#pragma region 信息结构体

//控制线程安全-加锁
struct RedisContext
{
	//MYSQL* pConn;
	bool isClosed = false;
	uv_mutex_t lock;
	redisContext* pConn;
};

//connect必须信息
struct connect_req
{
	char* ip;
	int port;

	void(*open_cb)(const char* error, RedisContext* context);

	char* error;
	RedisContext* context;
};

//query必须信息
struct query_req
{
	RedisContext* context;
	char* cmd;
	void(*query_cb)(const char* err, redisReply* result);
	char* error;
	redisReply* result;
};
#pragma endregion

#pragma region 回调

static void connect_work(uv_work_t* req) 
{
	auto pInfo = (connect_req*)req->data;
	timeval timeout = { 5.0};
	auto pConn = RedisConnectWithTimeout(pInfo->ip, pInfo->port, timeout);
	if (pConn->err)
	{
		pInfo->error = strdup(pConn->errstr);
		RedisFree(pConn);
		return;
	}

	pInfo->context->pConn = pConn;
}

static void on_connect_complete(uv_work_t* req, int status)
{
	auto pInfo = (connect_req*)req->data;

	pInfo->open_cb(pInfo->error, pInfo->context);

	if (pInfo->ip)
		free(pInfo->ip);
	if (pInfo->error)
		free(pInfo->error);

	my_free(pInfo);
	my_free(req);

}

static void close_work(uv_work_t* req)
{
	auto pConn = (RedisContext*)req->data;

	//加锁，等待正在进行的查询结束
	uv_mutex_lock(&pConn->lock);
	log_debug("close 加锁");
	RedisFree(pConn->pConn);
	pConn->pConn = NULL;
	log_debug("close 释放");
	uv_mutex_unlock(&pConn->lock);
}

static void on_close_complete(uv_work_t* req, int status)
{
	if (req->data)
		my_free(req->data);
	my_free(req);
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
	log_debug("query 加锁");

	auto replay = (redisReply*)RedisCommand(pInfo->context->pConn, pInfo->cmd);
	if (replay)
	{
		pInfo->result = replay;
	}
	log_debug("query 释放");
	uv_mutex_unlock(&pInfo->context->lock);

}

static void on_query_complete(uv_work_t* req, int status)
{
	auto pInfo = (query_req*)req->data;
	
	if(pInfo->query_cb)
		pInfo->query_cb(pInfo->error, pInfo->result);

	if (pInfo->cmd)
		free(pInfo->cmd);
	if (pInfo->error)
		free(pInfo->error);
	if (pInfo->result)
	{
		FreeReplyObject(pInfo->result);
	}
	
	my_free(pInfo);
	my_free(req);
}

#pragma endregion

#pragma region redis_wrapper
void redis_wrapper::connect(char* ip, int port, void(*open_cb)(const char* error, RedisContext* context))
{
	auto w = (uv_work_t * )my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	auto info = (connect_req*)my_alloc(sizeof(connect_req));
	memset(info, 0, sizeof(connect_req)); 

	auto lockContext = (RedisContext*)my_alloc(sizeof(RedisContext));
	memset(lockContext, 0, sizeof(RedisContext));
	uv_mutex_init(&lockContext->lock);//初始化信号量

	info->ip = strdup(ip);
	info->port = port;
	info->open_cb = open_cb; 
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

void redis_wrapper::query(RedisContext* context, char* sql, void(*callback)(const char* err, redisReply* result))
{
	auto w = (uv_work_t*)my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	auto pInfo = (query_req*)my_alloc(sizeof(query_req));
	memset(pInfo, 0, sizeof(query_req));

	pInfo->context = context;
	pInfo->cmd = strdup(sql);
	pInfo->query_cb = callback;

	w->data = pInfo;

	uv_queue_work(uv_default_loop(), w, query_work, on_query_complete);

}

#pragma endregion


