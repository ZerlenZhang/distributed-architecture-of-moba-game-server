#include <time.h>
#include <stdint.h>
#include <string.h>
#include <stdlib.h>

#define my_malloc malloc
#define my_free free

#define SMALL_CHUNK 256

char*
base64_encode(const uint8_t* text, int sz, int* out_esz) {
	static const char* encoding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

	int encode_sz = (sz + 2) / 3 * 4;
	
	char *buffer = NULL;
	if (encode_sz >= SMALL_CHUNK) {
		buffer = my_malloc(encode_sz + 1);
	}
	else {
		buffer = my_malloc(SMALL_CHUNK);
	}
	int i, j;
	j = 0;
	for (i = 0; i<(int)sz - 2; i += 3) {
		uint32_t v = text[i] << 16 | text[i + 1] << 8 | text[i + 2];
		buffer[j] = encoding[v >> 18];
		buffer[j + 1] = encoding[(v >> 12) & 0x3f];
		buffer[j + 2] = encoding[(v >> 6) & 0x3f];
		buffer[j + 3] = encoding[(v)& 0x3f];
		j += 4;
	}
	int padding = sz - i;
	uint32_t v;
	switch (padding) {
	case 1:
		v = text[i];
		buffer[j] = encoding[v >> 2];
		buffer[j + 1] = encoding[(v & 3) << 4];
		buffer[j + 2] = '=';
		buffer[j + 3] = '=';
		break;
	case 2:
		v = text[i] << 8 | text[i + 1];
		buffer[j] = encoding[v >> 10];
		buffer[j + 1] = encoding[(v >> 4) & 0x3f];
		buffer[j + 2] = encoding[(v & 0xf) << 2];
		buffer[j + 3] = '=';
		break;
	}
	buffer[encode_sz] = 0;
	*out_esz = encode_sz;
	return buffer;
}

void
base64_encode_free(char* result) {
	my_free(result);
}
