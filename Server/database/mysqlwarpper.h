#ifndef __MYSQLWARPPER_H__
#define __MYSQLWARPPER_H__
#include <vector>
#include <string>

#include <uv.h>
#include <mysql.h>

//控制线程安全-加锁
struct MysqlContext
{
	MYSQL* pConn;
	bool isClosed = false;
	uv_mutex_t lock;
	void* udata;
};

typedef void(*MysqlQueryCallback)(const char* err, const std::vector<std::vector<std::string>*>* result);
typedef void(*MysqlConnectCallback)(const char* error, MysqlContext* context);

class mysql_wrapper
{
public:
	static void connect(char* ip, int port, char* dbName,
		char* uName, char* password,
		MysqlConnectCallback,void* udata=NULL);

	static void close(MysqlContext* context);

	static void query(MysqlContext* context,
		char* sql,
		MysqlQueryCallback callback=NULL);
};



#endif // !__MYSQLWARPPER_H__
