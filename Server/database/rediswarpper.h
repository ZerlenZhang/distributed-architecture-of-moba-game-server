#ifndef __REDIS_WARPPER_H__
#define __REDIS_WARPPER_H__
#include <vector>
#include <string>
#include "hiredis.h"
struct RedisContext;

class redis_wrapper
{
public:
	static void connect(char* ip, int port,
		void(*open_cb)(const char* error,RedisContext* context));

	static void close_redis(RedisContext* context);

	static void query(RedisContext* context,
		char* sql,
		void(*callback)(const char* err, redisReply* result) = NULL);
};



#endif // !__REDIS_WARPPER_H__
