#ifndef __TCPPACKAGEPROTOCOL_H__
#define __TCPPACKAGEPROTOCOL_H__

//TCP包传输协议，处理粘包，拆包
class TcpProtocol
{
public:
	static bool ReadHeader(unsigned char* data, int dataLen,int* out_pkgSize,int* out_headerSize);
	static unsigned char* Package(const unsigned char* rawData, int rawDataLen, int* pkgLen);
	static void ReleasePackage(unsigned char* package);
};

#endif // !__TCPPACKAGEPROTOCOL_H__



