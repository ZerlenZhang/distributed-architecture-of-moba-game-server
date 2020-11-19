#ifndef __MYSQLWARPPER_H__
#define __MYSQLWARPPER_H__
#include <uv.h>
#include <mysql.h>

//控制线程安全-加锁
struct MysqlContext
{
	MYSQL* pConn;
	bool isClosed = false;
	uv_mutex_t lock;
	void* udata;
	//默认再uData不为空时free
	bool autoFreeUserData;
};

struct MysqlResult
{
	MYSQL_RES* result;
	void* udata;
	//默认再uData不为空时free
	bool autoFreeUserData;
};

typedef void(*MysqlQueryCallback)(const char* err, MysqlResult* result);
typedef void(*MysqlConnectCallback)(const char* error, MysqlContext* context);

class mysql_wrapper
{
public:
	static void connect(char* ip, int port, char* dbName,
		char* uName, char* password,
		MysqlConnectCallback,void* udata=NULL, bool autoFreeUdata = true);

	static void close(MysqlContext* context);

	static void query(MysqlContext* context,
		char* sql,
		MysqlQueryCallback callback=NULL,void* udata=NULL,bool autoFreeUdata=true);	
};



#endif // !__MYSQLWARPPER_H__
