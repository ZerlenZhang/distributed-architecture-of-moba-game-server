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
};

typedef void(*MysqlQueryCallback)(const char* err, const std::vector<std::vector<std::string>*>* result);

class mysql_wrapper
{
public:
	static void connect(char* ip, int port, char* dbName,
		char* uName, char* password,
		void(*open_cb)(const char* error,MysqlContext* context));

	static void close(MysqlContext* context);

	static void query(MysqlContext* context,
		char* sql,
		MysqlQueryCallback callback=NULL);
};



#endif // !__MYSQLWARPPER_H__
