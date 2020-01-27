#ifndef __WEBSOCKETPROTOCOL_H__
#define __WEBSOCKETPROTOCOL_H__

class WebSocketProtocol
{ 
public:
	//返回是否握过手
	static bool ShakeHand(AbstractSession* session, char* body, int len);
	//读取WebSocket头
	static int ReadHeader(unsigned char* pkgData, int pkgLen,int pkgSize,int& out_header_size);
	//解析收到的数据
	static void ParserRecvData(unsigned char* rawData,unsigned char* mask,int rawLen);
	//打包数据
	static unsigned char* PackageData(const unsigned char* rawData, int len, int* dataLen);
	//释放打包数据使用的内存
	static void FreePackageData(unsigned char* pkg);
};

#endif // !__WEBSOCKETPROTOCOL_H__



