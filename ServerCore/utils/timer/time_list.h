#ifndef __MY_TIMER_LIST_H__
#define __MY_TIMER_LIST_H__

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus


// on_timer是一个回掉函数,当timer触发的时候调用;
// udata: 是用户传的自定义的数据结构;
// on_timer执行的时候 udata,就是你这个udata;
// after_sec: 多少秒开始执行;
// repeat_count: 执行多少次, repeat_count == -1一直执行;
// 返回timer的句柄;
struct timer;


struct timer*
schedule_repeat(void(*on_timer)(void* udata), 
         void* udata, 
		 int delay,
		 int repeat_count,
		int repeat_msec);


// 取消掉这个timer;
void 
cancel_timer(struct timer* t);

struct timer*
schedule_once(void(*on_timer)(void* udata), 
              void* udata, 
			  int after_msec);

void* get_timer_udata(struct timer* t);

#ifdef __cplusplus
}
#endif // __cplusplus


#endif

