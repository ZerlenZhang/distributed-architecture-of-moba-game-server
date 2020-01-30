#ifndef __WEBSOCKETPROTOCOL_H__
#define __WEBSOCKETPROTOCOL_H__

class WebSocketProtocol
{ 
public:
	//返回是否握过手
	static bool ShakeHand(AbstractSession* session, char* body, int len);
	//读取WebSocket头
	static bool ReadHeader(unsigned char* pkgData, int pkgLen, int* out_pkgSize, int* out_header_size);
	//解析收到的数据
	static void ParserRecvData(unsigned char* rawData,unsigned char* mask,int rawLen);
	//打包数据
	static unsigned char* Package(const unsigned char* rawData, int rowDataLen, int * out_pkgLen);
	//释放打包数据使用的内存
	static void ReleasePackage(unsigned char* pkg);
};

#endif // !__WEBSOCKETPROTOCOL_H__



