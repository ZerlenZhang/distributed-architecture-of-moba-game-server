#include "mysqlwarpper.h"
#include "../utils/logger/logger.h"

#define my_alloc malloc
#define my_free free


//connect必须信息
struct connect_req
{
	char* ip;
	int port;
	char* dbName;
	char* uName;
	char* password;

	MysqlConnectCallback open_cb;

	char* error;
	MysqlContext* context;
};

//query必须信息
struct query_req
{
	MysqlContext* context;
	char* cmd;
	void(*query_cb)(const char* err, const std::vector<std::vector<std::string>*>* result);

	char* error;
	std::vector<std::vector<std::string>*>* result;
};

#pragma region 回调
static void connect_work(uv_work_t* req)
{
	auto pInfo = (connect_req*)req->data;
	
	//加锁，避免尚未链接数据库就有线程进行数据库读写操作
	uv_mutex_lock(&pInfo->context->lock);
	//log_debug("connect 加锁");

	pInfo->context->pConn = mysql_init(NULL);

	auto result = mysql_real_connect(
		pInfo->context->pConn,
		pInfo->ip,
		pInfo->uName,
		pInfo->password,
		pInfo->dbName,
		pInfo->port,
		NULL,
		0);

	if (result == NULL)
	{
		pInfo->error = strdup(mysql_error(pInfo->context->pConn));
	}

	//log_debug("connect 释放");
	uv_mutex_unlock(&pInfo->context->lock);
}

static void on_connect_complete(uv_work_t* req, int status)
{
	auto pInfo = (connect_req*)req->data;

	log_debug("Mysql 数据库链接成功");

	pInfo->open_cb(pInfo->error, pInfo->context);

	if (pInfo->ip)
		free(pInfo->ip);
	if (pInfo->dbName)
		free(pInfo->dbName);
	if (pInfo->uName)
		free(pInfo->uName);
	if (pInfo->password)
		free(pInfo->password);
	if (pInfo->error)
		free(pInfo->error);

	//my_free(pInfo->context);
	my_free(pInfo);
	my_free(req);

}

static void close_work(uv_work_t* req)
{
	auto pConn = (MysqlContext*)req->data;

	//加锁，等待正在进行的查询结束
	uv_mutex_lock(&pConn->lock);
	//log_debug("close 加锁");
	mysql_close(pConn->pConn);
	pConn->pConn = NULL;
	//log_debug("close 释放");
	uv_mutex_unlock(&pConn->lock);
}

static void on_close_complete(uv_work_t* req, int status)
{
	if (req->data)
		my_free(req->data);
	my_free(req);
	log_debug("Mysql 数据库断开连接");
}

static void query_work(uv_work_t* req)
{
	auto pInfo = (query_req*)req->data;

	if (pInfo->context->isClosed)
	{// 已经关闭此链接，就不要再操作了
		//log_debug("链接已经关闭，查询无效");
		return;
	}

	//线程锁
	uv_mutex_lock(&pInfo->context->lock);
	//log_debug("query 加锁");

	auto ret = mysql_query(pInfo->context->pConn, pInfo->cmd);

	if (ret != 0)
	{
		pInfo->error = strdup(mysql_error(pInfo->context->pConn));
		//释放线程锁
		//log_debug("query 释放");
		uv_mutex_unlock(&pInfo->context->lock);
		return;
	}

	auto result = mysql_store_result(pInfo->context->pConn);
	if (!result)
	{// 没有查询到任何数据
		//log_debug("query 释放");
		uv_mutex_unlock(&pInfo->context->lock);
		//释放线程锁
		return;
	}

	pInfo->result = new std::vector<std::vector<std::string>*>;
	

	auto num = mysql_num_fields(result);

	MYSQL_ROW row;
	while (row = mysql_fetch_row(result))
	{
		auto temp = new std::vector<std::string>;
		for (auto i = 0; i < num; i++)
		{
			temp->push_back(row[i]);
		}
		pInfo->result->push_back(temp);
	}
	mysql_free_result(result);
	//释放线程锁
	//log_debug("query 释放");
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
		for (auto v : *(pInfo->result))
		{
			if(v)
				delete v;
		}
	}

	delete pInfo->result;

	my_free(pInfo);
	my_free(req);
}
#pragma endregion




void mysql_wrapper::connect(char* ip, int port, char* dbName, char* uName, char* password, MysqlConnectCallback open_cb,void* udata)
{
	auto w = (uv_work_t * )my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	auto info = (connect_req*)my_alloc(sizeof(connect_req));
	memset(info, 0, sizeof(connect_req)); 

	auto lockContext = (MysqlContext*)my_alloc(sizeof(MysqlContext));
	memset(lockContext, 0, sizeof(MysqlContext));
	uv_mutex_init(&lockContext->lock);//初始化信号量

	info->ip = strdup(ip);
	info->port = port;
	info->dbName = strdup(dbName);
	info->uName = strdup(uName);
	info->password = strdup(password);
	info->open_cb = open_cb; 
	info->context = lockContext;

	info->context->udata = udata;

	w->data = info;

	uv_queue_work(uv_default_loop(), w, connect_work, on_connect_complete);
}

void mysql_wrapper::close(MysqlContext* conntext)
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

void mysql_wrapper::query(MysqlContext* context, char* sql, MysqlQueryCallback callback)
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
