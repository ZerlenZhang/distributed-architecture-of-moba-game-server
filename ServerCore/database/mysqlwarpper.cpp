#include "mysqlwarpper.h"
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

static void free_strdup(char* str)
{
	my_free(str);
}


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
	void(*query_cb)(const char* err, MysqlResult* result);

	char* error;
	MysqlResult* myReply;
};


#pragma region 回调

static void connect_work(uv_work_t* req)
{
	auto pInfo = (connect_req*)req->data;
	
	//加锁，避免尚未链接数据库就有线程进行数据库读写操作
	uv_mutex_lock(&pInfo->context->lock);
	//log_debug("connect 加锁");

	pInfo->context->pConn = mysql_init(NULL);

	if (!pInfo->context->pConn)
	{
		#pragma region 释放内存
		pInfo->error = my_strdup("链接数据库失败——mysql_init(NULL) 返回 NULL, ");
		if (pInfo->open_cb)
			pInfo->open_cb(pInfo->error, pInfo->context);
		if (pInfo->ip)
			free_strdup(pInfo->ip);
		if (pInfo->dbName)
			free_strdup(pInfo->dbName);
		if (pInfo->uName)
			free_strdup(pInfo->uName);
		if (pInfo->password)
			free_strdup(pInfo->password);
		if (pInfo->error)
			free_strdup(pInfo->error);
		auto context = pInfo->context;
		if (context)
		{
			if (context->udata && context->autoFreeUserData)
				free(context->udata);
			log_debug("1");
			uv_mutex_unlock(&context->lock);
			my_free(context);
		}

		log_debug("2");
		my_free(pInfo);
		my_free(req);

		#pragma endregion

		return;
	}

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
		pInfo->error = my_strdup(mysql_error(pInfo->context->pConn));
	}

	//log_debug("connect 释放");
	uv_mutex_unlock(&pInfo->context->lock);
	//my_free(req);
}

static void on_connect_complete(uv_work_t* req, int status)
{
	auto pInfo = (connect_req*)req->data;

	//log_debug("Mysql 数据库链接成功");
	if (pInfo)
	{
		if(pInfo->open_cb)
			pInfo->open_cb(pInfo->error, pInfo->context);
		if (pInfo->ip)
			free_strdup(pInfo->ip);
		if (pInfo->dbName)
			free_strdup(pInfo->dbName);
		if (pInfo->uName)
			free_strdup(pInfo->uName);
		if (pInfo->password)
			free_strdup(pInfo->password);
		if (pInfo->error)
			free_strdup(pInfo->error);
		my_free(pInfo);
	}



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
	auto context = (MysqlContext *) req->data;
	if (context)
	{
		if (context->udata && context->autoFreeUserData)
			free(context->udata);
		my_free(context);
	}
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
		pInfo->error = my_strdup(mysql_error(pInfo->context->pConn));
		//释放线程锁
		//log_debug("query 释放");
		uv_mutex_unlock(&pInfo->context->lock);
		return;
	}

	auto result = mysql_store_result(pInfo->context->pConn);

	pInfo->myReply->result = result;
	
	//释放线程锁
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
			free_strdup(pInfo->cmd);
		if (pInfo->error)
			free_strdup(pInfo->error);
		if (pInfo->myReply)
		{
			if (pInfo->myReply->udata && pInfo->myReply->autoFreeUserData)
				free(pInfo->myReply->udata);
			if (pInfo->myReply->result)
				mysql_free_result(pInfo->myReply->result);
			my_free(pInfo->myReply);
		}
		my_free(pInfo);
	}
	my_free(req);
}

#pragma endregion




void mysql_wrapper::connect(char* ip, int port, char* dbName, char* uName, char* password, MysqlConnectCallback open_cb,void* udata, bool autoFreeUdata)
{
	auto w = (uv_work_t * )my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	#pragma region info(connect_req)

	auto info = (connect_req*)my_alloc(sizeof(connect_req));
	memset(info, 0, sizeof(connect_req)); 


	info->ip = my_strdup(ip);
	info->port = port;
	info->dbName = my_strdup(dbName);
	info->uName = my_strdup(uName);
	info->password = my_strdup(password);
	info->open_cb = open_cb; 

	#pragma region lockContext(MysqlContext)

	auto lockContext = (MysqlContext*)my_alloc(sizeof(MysqlContext));
	memset(lockContext, 0, sizeof(MysqlContext));
	lockContext->autoFreeUserData = autoFreeUdata;
	lockContext->udata = udata;

	uv_mutex_init(&lockContext->lock);//初始化信号量

	#pragma endregion

	info->context = lockContext;

	#pragma endregion

	w->data = info;

	uv_queue_work(uv_default_loop(), w, connect_work, on_connect_complete);
}

void mysql_wrapper::close(MysqlContext* conntext)
{
	if (conntext->isClosed)
	{// 已经关了
		log_warning("Mysql 已经关了");
		return;
	}


	auto w = (uv_work_t*)my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	conntext->isClosed = true;

	w->data = conntext;

	uv_queue_work(uv_default_loop(), w, close_work, on_close_complete);
}

void mysql_wrapper::query(MysqlContext* context, char* sql, MysqlQueryCallback callback, void* udata,bool autoFreeUdata)
{
	auto w = (uv_work_t*)my_alloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	auto pInfo = (query_req*)my_alloc(sizeof(query_req));
	memset(pInfo, 0, sizeof(query_req));

	auto mysqlResult = (MysqlResult*)my_alloc(sizeof(MysqlResult));
	memset(mysqlResult, 0, sizeof(MysqlResult));

	mysqlResult->autoFreeUserData = autoFreeUdata;

	mysqlResult->udata = udata;

	pInfo->context = context;
	pInfo->cmd = my_strdup(sql);
	pInfo->query_cb = callback;
	pInfo->myReply = mysqlResult;

	w->data = pInfo;

	uv_queue_work(uv_default_loop(), w, query_work, on_query_complete);

}
