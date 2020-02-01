#ifndef __LOGGER_H__
#define __LOGGER_H__

enum {
	tag_DEBUG = 0,
	tag_WARNING,
	tag_ERROR,
};

#define log_debug(msg, ...) logger::log(__FILE__, __LINE__, tag_DEBUG, (msg), ## __VA_ARGS__);
#define log_warning(msg, ...) logger::log(__FILE__, __LINE__, tag_WARNING, (msg), ## __VA_ARGS__);
#define log_error(msg, ...) logger::log(__FILE__, __LINE__, tag_ERROR, (msg), ## __VA_ARGS__);

class logger {
public:
	static void init(const char* path, const char* prefix, bool std_output = false);
	static void log(const char* file_name, 
	                int line_num, 
	                int level, const char* msg, ...);
};


#endif

