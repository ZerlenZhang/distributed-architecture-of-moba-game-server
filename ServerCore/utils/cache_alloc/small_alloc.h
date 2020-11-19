#ifndef __SMALL_ALLOC_H__
#define __SMALL_ALLOC_H__

#ifdef __cplusplus
extern "C" {
#endif
	void* small_alloc(int size);
	void small_free(void* mem);

#ifdef __cplusplus
}
#endif


#endif // !__SMALL_ALLOC_H__
