
#include <cstring>
#include <cstdlib>
#include "CmdPackageProtocol.h"
#include "../../utils/logger/logger.h"


#pragma region 全局变量

static ProtoType g_protoType;
static map<int, string> g_pb_cmd_map;

#pragma endregion


void CmdPackageProtocol::Init(::ProtoType proto_type)
{
	g_protoType = proto_type;
}


void CmdPackageProtocol::RegisterProtoCmdMap(map<int, string>& map)
{
	for (auto x : map)
	{
		g_pb_cmd_map.insert(x);
	}
}

const char* CmdPackageProtocol::ProtoCmdTypeToName(int cmdType)
{
	if (g_pb_cmd_map.find(cmdType) == g_pb_cmd_map.end())
	{
		return NULL;
	}
	return g_pb_cmd_map[cmdType].c_str();
}

ProtoType CmdPackageProtocol::ProtoType()
{
	return g_protoType;
}

bool CmdPackageProtocol::DecodeCmdMsg(unsigned char* cmd, const int cmd_len, CmdPackage*& out_msg)
{
	out_msg = NULL;
	// serviceType (2 bytes) | cmdType (2 bytes) | userTag (4 bytes) | body
	if (cmd_len < CMD_HEADER_SIZE)
	{// 数据太短
		log_debug("数据太短");
		return false;
	}

	out_msg = (CmdPackage*)malloc(sizeof(CmdPackage));
	memset(out_msg, 0, sizeof(CmdPackage));

	out_msg->serviceType = cmd[0] | (cmd[1] << 8);
	out_msg->cmdType = cmd[2] | (cmd[3] << 8);
	out_msg->userTag = cmd[4] | (cmd[5] << 8) | (cmd[6] << 16) | (cmd[7] << 24);
	out_msg->body = NULL;

	if (cmd_len == CMD_HEADER_SIZE)
		return true;


	// 解密

	auto dataLen = cmd_len - CMD_HEADER_SIZE;
	char* tempCharPointer;
	google::protobuf::Message* tempMessagePointer;
	switch (g_protoType)
	{
	case ProtoType::Json:
		tempCharPointer = (char*)malloc(dataLen + 1);
		memcpy(tempCharPointer, cmd + CMD_HEADER_SIZE, dataLen);
		tempCharPointer[dataLen] = 0;
		out_msg->body = tempCharPointer;
		break;
	case ProtoType::Protobuf:
		//没有这个protobuf协议
		tempMessagePointer = CreateMessage(g_pb_cmd_map[out_msg->cmdType].c_str());
		if (tempMessagePointer == NULL)
		{
			log_debug("获取Message类型为空");
			out_msg = NULL;
			return false;
		}

		if (!tempMessagePointer->ParseFromArray(cmd + CMD_HEADER_SIZE, cmd_len - CMD_HEADER_SIZE))
		{
			log_debug("消息反序列化失败");
			free(out_msg);
			out_msg = NULL;
			ReleaseMessage(tempMessagePointer);
			return false;
		}

		out_msg->body = tempMessagePointer;


		break;
	}

	return true;
}

void CmdPackageProtocol::FreeCmdMsg(CmdPackage* msg)
{
	if (msg->body)
	{
		switch (g_protoType)
		{
		case ProtoType::Json:
			free(msg->body);
			msg->body = NULL;
			break;
		case ProtoType::Protobuf:
			auto pm = (google::protobuf::Message*)msg->body;
			delete pm;
			msg->body = NULL;
			break;
		}
	}

	free(msg);
}

unsigned char* CmdPackageProtocol::EncodeCmdPackageToRaw(const CmdPackage* msg, int* out_len)
{
	//加密


	unsigned char* rawData = NULL;

	char* tempCharPointer;
	int tempLen;
	google::protobuf::Message* tempMessagePointer;

	switch (g_protoType)
	{
	case ProtoType::Json:

		tempCharPointer = (char*)msg->body;
		tempLen = strlen(tempCharPointer) + 1;

		rawData = (unsigned char*)malloc(CMD_HEADER_SIZE + tempLen);
		memcpy(rawData + CMD_HEADER_SIZE, tempCharPointer, tempLen -1);

		rawData[CMD_HEADER_SIZE + tempLen - 1] = 0;

		//传出参数
		*out_len = CMD_HEADER_SIZE + tempLen;
		break;
	case ProtoType::Protobuf:
		tempMessagePointer = (google::protobuf::Message*)msg->body;
		tempLen = tempMessagePointer->ByteSize();
		rawData = (unsigned char*)malloc(CMD_HEADER_SIZE + tempLen);

		if (!tempMessagePointer->SerializePartialToArray(rawData + CMD_HEADER_SIZE, tempLen))
		{// 如果序列化不成功
			free(rawData);
			*out_len = 0;
			return NULL;
		}

		//传出参数
		*out_len = CMD_HEADER_SIZE + tempLen;
		break;
	}

	//写包头信息
	rawData[0] = msg->serviceType & 0x000000ff;
	rawData[1] = (msg->serviceType & 0x0000ff00) >> 8;
	rawData[2] = msg->cmdType & 0x000000ff;
	rawData[3] = (msg->cmdType & 0x0000ff00) >> 8;

	//userTag
	memcpy(rawData + 4, &msg->userTag, 4);

	return rawData;
}

void CmdPackageProtocol::FreeCmdPackageRaw(unsigned char* raw) 
{
	free(raw);
}

google::protobuf::Message* CmdPackageProtocol::CreateMessage(const char* typeName)
{
	google::protobuf::Message* msg = NULL;
	//根据名字，找到message的描述对象
	auto descriptor = google::protobuf::DescriptorPool::generated_pool()->FindMessageTypeByName(typeName);
	if (descriptor)
	{
		//根据描述对象从对象工厂中生成一个对应模板对象
		//根据模板复制出来一个
		auto protoType = google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
		if (protoType)
		{
			msg = protoType->New();
		}
	}
	return msg;
}

void CmdPackageProtocol::ReleaseMessage(google::protobuf::Message* msg)
{
	delete msg;
}
