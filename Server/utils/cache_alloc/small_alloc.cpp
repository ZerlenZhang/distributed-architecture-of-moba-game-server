#include "small_alloc.h"
#include <stdlib.h>
#include "cache_alloc.h" 

#define SMALL_ELEM_NUM 10*1024
#define SMALL_ELEM_SIZE 128

static struct cache_allocer* allocer = NULL;

void* small_alloc(int size)
{
	if (allocer == NULL)
	{
		allocer = create_cache_allocer(SMALL_ELEM_NUM, SMALL_ELEM_SIZE);			   		 
	}
	return cache_alloc(allocer, size);

}

void small_free(void* mem)
{
	if (allocer == NULL)
	{
		allocer = create_cache_allocer(SMALL_ELEM_NUM, SMALL_ELEM_SIZE);
	}
	cache_free(allocer, mem);
}
