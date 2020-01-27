#include "AbstractSession.h"
#include "WebSocketProtocol.h"


bool WebSocketProtocol::ShakeHand(AbstractSession* session, char* body, int len)
{
	return true;
}

int WebSocketProtocol::ReadHeader(unsigned char* pkgData, int pkgLen, int pkgSize, int& out_header_size)
{
	return 0;
}

void WebSocketProtocol::ParserRecvData(unsigned char* rawData, unsigned char* mask, int rawLen)
{
}

unsigned char* WebSocketProtocol::PackageData(const unsigned char* rawData, int len, int* dataLen)
{
	return nullptr;
}

void WebSocketProtocol::FreePackageData(unsigned char* pkg)
{
}
