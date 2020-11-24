#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
#include <fcntl.h>
#include <time.h>
#include <string>
using namespace std;

#include "logger.h"
#include "uv.h"


static string g_log_path;
static string g_prefix;
static uv_fs_t g_file_handle;
static uint32_t g_current_day;
static uint32_t g_last_second;
static char g_format_time[64] = { 0 };
static char* g_log_level[] = { (char*)"Debug", (char*)"Warning", (char*)"Error"};
static bool g_std_out = false;
static bool isInit = false;

static void 
open_file(tm* time_struct) {
	int result = 0;
	char fileName[128] = { 0 };

	if (g_file_handle.result != 0) {
		uv_fs_close(uv_default_loop(), &g_file_handle, g_file_handle.result, NULL);
		uv_fs_req_cleanup(&g_file_handle);
		g_file_handle.result = 0;
	}

	sprintf(fileName, "%s%s.%4d%02d%02d.log", g_log_path.c_str(), g_prefix.c_str(), time_struct->tm_year + 1900, time_struct->tm_mon + 1, time_struct->tm_mday);
	result = uv_fs_open(NULL, &g_file_handle, fileName, O_CREAT | O_RDWR | O_APPEND, S_IREAD | S_IWRITE, NULL);
	if (result < 0) {
		fprintf(stderr, "open file failed! name=%s, reason=%s", fileName, uv_strerror(result));
	}
}

static void 
prepare_file() {
	time_t  now = time(NULL);
	now += 8 * 60 * 60;
	tm* time_struct = gmtime(&now);
	if (g_file_handle.result == 0) {
		g_current_day = time_struct->tm_mday;
		open_file(time_struct);
	}
	else {
		if (g_current_day != time_struct->tm_mday) {
			g_current_day = time_struct->tm_mday;
			open_file(time_struct);
		}
	}

}

static void 
format_time() {
	time_t  now = time(NULL);
	now += 8 * 60 * 60;
	tm* time_struct = gmtime(&now);
	if (now != g_last_second) {
		g_last_second = (uint32_t)now;
		memset(g_format_time, 0, sizeof(g_format_time));
		sprintf(g_format_time, "%4d%02d%02d %02d:%02d:%02d ",
			time_struct->tm_year + 1900, time_struct->tm_mon + 1, time_struct->tm_mday,
			time_struct->tm_hour, time_struct->tm_min, time_struct->tm_sec);
	}
}

void 
logger::init(const char* path,const char* prefix, bool std_output) {
	if (isInit)
		return;
	isInit = true;

	g_prefix = prefix;
	g_log_path = path;
	g_std_out = std_output;

	if (*(g_log_path.end() - 1) != '/') {
		g_log_path += "/";
	}

	std::string tmp_path = g_log_path;
	int find = tmp_path.find("/");
	uv_fs_t req;
	int result;

	while (find != std::string::npos) {
		result = uv_fs_mkdir(uv_default_loop(), &req, tmp_path.substr(0, find).c_str(), 0755, NULL);
		find = tmp_path.find("/", find + 1);
	}
	uv_fs_req_cleanup(&req);
}

void 
logger::log(const char* file_name,
	int line_num,
	int level, const char* msg, ...) {
	prepare_file();
	format_time();
	static char msg_meta_info[1024] = { 0 };
	static char msg_content[1024 * 10] = { 0 };
	static char new_line = '\n';
	static char tabKey = '\t';

	va_list args;
	va_start(args, msg);
	vsnprintf(msg_content, sizeof(msg_content), msg, args);
	va_end(args);

	sprintf(msg_meta_info, "%s:%u  ", file_name, line_num);
	uv_buf_t buf[6]; // time level content fileandline newline
	buf[0] = uv_buf_init(g_format_time, static_cast<unsigned int>(strlen(g_format_time)));
	buf[1] = uv_buf_init(g_log_level[level], static_cast<unsigned int>(strlen(g_log_level[level])));
	buf[2] = uv_buf_init(msg_meta_info, static_cast<unsigned int>(strlen(msg_meta_info)));
	buf[3] = uv_buf_init(&tabKey, static_cast<unsigned int>(1));
	buf[4] = uv_buf_init(msg_content, static_cast<unsigned int>(strlen(msg_content)));
	buf[5] = uv_buf_init(&new_line, static_cast<unsigned int>(1));

	uv_fs_t writeReq;
	int result = uv_fs_write(NULL, &writeReq, g_file_handle.result, buf, sizeof(buf) / sizeof(buf[0]), -1, NULL);
	if (result < 0) {
		fprintf(stderr, "log failed %s%s%s%s", g_format_time, g_log_level[level], msg_meta_info, msg_content);
	}

	uv_fs_req_cleanup(&writeReq);

	if (g_std_out) {
		printf("[%s]\t %s\t\t<%s:%u>\n", g_log_level[level], msg_content, file_name, line_num);	
	}
}
								   


