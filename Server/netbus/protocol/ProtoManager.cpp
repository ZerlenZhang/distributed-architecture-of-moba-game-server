#include "ProtoManager.h"
#include <cstring>
#include <cstdlib>
#include "google/protobuf/message.h"


#define MAX_PF_MAP_SIZE 1024

#pragma region 全局变量

static ProtoType g_protoType;
static char* g_pf_map[MAX_PF_MAP_SIZE];
static int g_cmdCount = 0;

#pragma endregion

static google::protobuf::Message* CreateMessage(const char* typeName)
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

static void ReleaseMessage(google::protobuf::Message* msg)
{
	delete msg;
}


void ProtoManager::Init(::ProtoType proto_type)
{
	g_protoType = proto_type;
}

void ProtoManager::RegisterPfCmdMap(const char** pf_map, int len)
{
	len = MAX_PF_MAP_SIZE - g_cmdCount < len 
		? MAX_PF_MAP_SIZE - g_cmdCount 
		: len;

	for (auto i = 0; i < len; i++)
	{
		g_pf_map[g_cmdCount + i] = strdup(pf_map[i]);
	}

	g_cmdCount += len;

}

ProtoType ProtoManager::ProtoType()
{
	return g_protoType;
}


bool ProtoManager::DecodeCmdMsg(unsigned char* cmd, int cmd_len, CmdPackage*& out_msg)
{
	out_msg = NULL;
	// serviceType (2 bytes) | cmdType (2 bytes) | userTag (4 bytes) | body
	if (cmd_len < 8) 
	{// 数据太短
		return;
	}

	out_msg = (CmdPackage*)malloc(sizeof(CmdPackage));
	memset(out_msg, 0, sizeof(CmdPackage));

	out_msg->serviceType = cmd[0] | (cmd[1] << 8);
	out_msg->cmdType = cmd[2] | (cmd[3] << 8);
	out_msg->userTag = cmd[4] | (cmd[5] << 8) | (cmd[6] << 16) | (cmd[7] << 24);
	out_msg->body = NULL;

	if (cmd_len == 8)
		return true;

	auto dataLen = cmd_len - 8;

	switch (g_protoType)
	{
	case ProtoType::Json:
		auto jsonStr = (char*)malloc(dataLen + 1);
		memcpy(jsonStr, cmd + 8, dataLen);
		jsonStr[dataLen] = 0;
		out_msg->body = jsonStr;
		break;
	case ProtoType::Protobuf:
		//没有这个protobuf协议
		if (out_msg->cmdType < 0
			|| out_msg->cmdType >= g_cmdCount
			|| g_pf_map[out_msg->cmdType] == NULL)
		{
			free(out_msg);
			out_msg = NULL;
			return false;
		}

		auto pm = CreateMessage(g_pf_map[out_msg->cmdType]);
		if (pm == NULL)
		{
			free(out_msg);
			out_msg = NULL;
			return false;
		}

		if (!pm->ParseFromArray(cmd + 8, cmd_len - 8))
		{
			free(out_msg);
			out_msg = NULL;
			ReleaseMessage(pm);
			return false;
		}

		out_msg->body = pm;


		break;
	default:
		break;
	}

	return false;
}

void ProtoManager::FreeCmdMsg(CmdPackage* msg)
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
		default:
			break;
		}
	}

	free(msg);
}

unsigned char* ProtoManager::EncodeCmdMsgToRaw(const CmdPackage* msg, int* out_len)
{
	return nullptr;
}

void ProtoManager::FreeCmdMsgRaw(unsigned char* raw)
{
}
