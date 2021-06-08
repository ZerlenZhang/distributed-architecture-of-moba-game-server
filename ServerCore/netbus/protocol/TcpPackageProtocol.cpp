#include "TcpPackageProtocol.h"
#include <stdlib.h>
#include <string.h>

#include <stdio.h>
#include "../../utils/cache_alloc/cache_alloc.h"
#include "../../utils/logger/logger.h"

extern cache_allocer* writeBufAllocer;



bool TcpProtocol::ReadHeader(unsigned char* data, int dataLen, int* out_pkgSize, int* out_headerSize)
{
	if (dataLen < 2) 
	{// 太短，无法解析
		log_debug("太短，无法解析");
		return false;
	}

	*out_pkgSize = data[0] | (data[1] << 8);
	*out_headerSize = 2;

	return true;
}

unsigned char* TcpProtocol::Package(const unsigned char* rawData, int len, int* pkgLen)
{
	int headSize = 2; 
	unsigned char* dataBuf = (unsigned char*)cache_alloc(writeBufAllocer, headSize + len);
	
	//记录包体长度
	dataBuf[0] = (headSize+len) & 0x000000ff;
	dataBuf[1] = ((headSize+len) & 0x0000ff00) >> 8;
	
	//将数据拷贝到长度后面
	memcpy(dataBuf + headSize, rawData, len);
	*pkgLen = headSize + len;
	
	return dataBuf;
}

void TcpProtocol::ReleasePackage(unsigned char* package)
{
	cache_free(writeBufAllocer, package);
}
