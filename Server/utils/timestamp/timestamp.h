#ifndef __TIMESTAMP_H__
#define __TIMESTAMP_H__

#ifdef __cplusplus
extern "C" {
#endif

	// 获取当前的时间戳
	unsigned long timestamp();

	// 获取给定日期的时间戳"%Y(年)%m(月)%d(日)%H(小时)%M(分)%S(秒)"
	unsigned long date2timestamp(const char* fmt_date, const char* date);

	// fmt_date "%Y(年)%m(月)%d(日)%H(小时)%M(分)%S(秒)"
	void timestamp2date(unsigned long t,const char*fmt_date, char* out_buf, int buf_len);


	unsigned long timestamp_today();

	unsigned long timestamp_yesterday();

#ifdef __cplusplus
}
#endif

#endif

