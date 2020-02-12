#ifndef __PROTOMANAGER_H__
#define __PROTOMANAGER_H__

#include "google/protobuf/message.h"
#include <map>
#include <string>
using std::map;
using std::string;

//文本传输方式
enum class ProtoType{
	Json = 0,
	Protobuf = 1,
};

//自定义协议包
struct CmdPackage {

#define CMD_HEADER_SIZE 8
	// 包结构
	// serviceType (2 bytes) | cmdType (2 bytes) | userTag (4 bytes) | body

	int serviceType;	//服务号
	int cmdType;		//命令号
	unsigned int userTag;//用户标识
	void* body;			// JSON str 或者是message;
};

//自定义协议的管理者
//控制传输数据的加密，编码，解码
class CmdPackageProtocol
{
public:
	//初始化协议
	static void Init(ProtoType proto_type=ProtoType::Protobuf);
	
	static void RegisterProtobufCmdMap(map<int, string>& map);


	static const char* ProtoCmdTypeToName(int cmdType);
	
	static ProtoType ProtoType();

	static bool DecodeCmdMsg(unsigned char* cmd, const int cmd_len, struct CmdPackage*& out_msg);
	static void FreeCmdMsg(struct CmdPackage* msg);

	static unsigned char* EncodeCmdPackageToRaw(const struct CmdPackage* msg, int* out_len);
	static void FreeCmdPackageRaw(unsigned char* raw);

	static google::protobuf::Message* CreateMessage(const char* typeName);
	static void ReleaseMessage(google::protobuf::Message* msg);
};




#endif // !__PROTOMANAGER_H__

