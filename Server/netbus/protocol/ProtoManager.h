#ifndef __PROTOMANAGER_H__
#define __PROTOMANAGER_H__


//文本传输方式
enum class ProtoType{
	Json = 0,
	Protobuf = 1,
};

//自定义协议包
struct CmdPackage {

	// 包结构
	// serviceType (2 bytes) | cmdType (2 bytes) | userTag (4 bytes) | body

	int serviceType;	//服务号
	int cmdType;		//命令号
	unsigned int userTag;//用户标识
	void* body;			// JSON str 或者是message;
};

//自定义协议的管理者
//控制传输数据的加密，编码，解码
class ProtoManager
{
public:
	//初始化协议
	static void Init(ProtoType proto_type=ProtoType::Protobuf);
	static void RegisterPfCmdMap(const char** pf_map, int len);
	static ProtoType ProtoType();

	static bool DecodeCmdMsg(unsigned char* cmd, int cmd_len, struct CmdPackage*& out_msg);
	static void FreeCmdMsg(struct CmdPackage* msg);

	static unsigned char* EncodeCmdMsgToRaw(const struct CmdPackage* msg, int* out_len);
	static void FreeCmdMsgRaw(unsigned char* raw);
};




#endif // !__PROTOMANAGER_H__

