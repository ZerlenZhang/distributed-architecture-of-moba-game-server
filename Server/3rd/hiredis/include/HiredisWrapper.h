#ifndef __HIREDIS_H__
#define __HIREDIS_H__

#define MAX_REDIS_COMMAND_LEN 256
#include <hiredis.h>
#define NO_QFORKIMPL
#include <Win32_Interop/win32fixes.h>

extern "C"
{
	_declspec(dllexport)  redisContext* RedisConnectWithTimeout(const char* ip, int port, const struct timeval tv);

	_declspec(dllexport)  void RedisFree(redisContext* c);

	_declspec(dllexport) void* RedisCommand(redisContext* c, const char* format, ...);

	_declspec(dllexport) void FreeReplyObject(void* reply);
}

#endif // !__HIREDIS_H__
