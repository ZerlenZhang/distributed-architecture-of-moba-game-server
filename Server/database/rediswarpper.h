#ifndef __REDIS_WARPPER_H__
#define __REDIS_WARPPER_H__
#include <hiredis.h>
#include <uv.h>

struct RedisContext
{
	//MYSQL* pConn;
	bool isClosed = false;
	uv_mutex_t lock;
	redisContext* pConn;

	void* udata;
	bool autoFreeUdata;
};

struct RedisReply
{
	redisReply* reply;
	void* udata;
	bool autoFreeUdata;
};


typedef void(*RedisConnectCallback)(const char* error, RedisContext* context);
typedef void(*RedisQueryCallback)(const char* err, RedisReply* result);

class redis_wrapper
{
public:
	static void connect(char* ip, int port,
		RedisConnectCallback callBack=NULL, void* udata = NULL, bool autoFreeUdata=true);

	static void close_redis(RedisContext* context);

	static void query(RedisContext* context,
		char* sql,
		RedisQueryCallback callback = NULL,void* udata=NULL,bool autoFreeUdata=true);
};



#endif // !__REDIS_WARPPER_H__
