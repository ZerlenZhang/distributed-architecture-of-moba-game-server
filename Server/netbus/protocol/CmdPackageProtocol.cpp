
#include <cstring>
#include <cstdlib>
#include <io.h>
#include "CmdPackageProtocol.h"
#include "../../utils/logger/logger.h"
#include "../../utils/cache_alloc/small_alloc.h"
#include "../../utils/cache_alloc/cache_alloc.h"
#include <google\protobuf\dynamic_message.h>
#include <google\protobuf\compiler\importer.h>
#include "../../utils/win32/WinUtil.h"
using namespace google::protobuf;
using namespace google::protobuf::compiler;

extern cache_allocer* writeBufAllocer;

#define my_alloc small_alloc
#define my_free small_free

#pragma region 全局变量

static ProtoType g_protoType;
static map<int, map<int, string>*> g_cmdMaps;
static DynamicMessageFactory* factory;
static Importer* importer;

static bool error = false;
static string protoFileDir;
#pragma endregion

//错误收集器，输出.proto文件错误
class ErrorCollector:public MultiFileErrorCollector
{
	// 通过 MultiFileErrorCollector 继承
	virtual void AddError(const string& filename, int line, int column, const string& message) override
	{
		logger::log((filename).c_str(), line, (int)tag_ERROR, message.c_str());
	}

	virtual void AddWarning(const string& filename, int line, int column,
		const string& message)override 
	{
		logger::log((filename).c_str(), line, (int)tag_WARNING, message.c_str());
	}

};

void CmdPackageProtocol::Init(::ProtoType proto_type)
{
	g_protoType = proto_type;
}

void CmdPackageProtocol::Init(::ProtoType proto_type,const string& protoFileDir)
{
	::protoFileDir = protoFileDir;
	g_protoType = proto_type;
	if (ProtoType::Protobuf == proto_type)
	{
		_finddata_t file;
		//if (_findfirst(protoFileDir.c_str(),&file)==-1l)
		//{
		//	log_error("使用protobuf但指定目录无效：%s", protoFileDir.c_str());
		//	error = true;
		//	return;
		//}
		
		static DiskSourceTree sourceTree;
		static ErrorCollector ec;
		sourceTree.MapPath("", protoFileDir);
		importer = new Importer(&sourceTree, &ec);
		auto size = protoFileDir.size();

		WinUtil::SearchFromDir(protoFileDir, "*.proto", -1, SearchType::ENUM_FILE,
			[=](const string& dirPath, const string& fileName)
			{
				importer->Import(fileName);
				//表示加载完这个，继续加载后面的
				return true;
			});

		factory = new DynamicMessageFactory;
	}
}


void CmdPackageProtocol::RegisterProtobufCmdMap(map<int, map<int,string>*>& src)
{
	if (error)
		return;
	auto index = 0;
	for (auto sType_cmdMap : src)
	{
		auto stype = sType_cmdMap.first;
		auto cmdMaps = src[stype];
		if (cmdMaps == NULL)
			continue;
		//log_debug("注册CmdNameMap: %d : %s", index++, x.second.c_str());
		for (auto ctype_key : *cmdMaps)
		{
			auto ctype = ctype_key.first;
			//添加到本地
			if (g_cmdMaps.find(stype) == g_cmdMaps.end())
			{// 如果没有这个key
				auto pCmdMap = new std::map<int, string>;
				g_cmdMaps[stype] = pCmdMap;
			}

			(*g_cmdMaps[stype])[ctype] = ctype_key.second;
		}

		delete cmdMaps;
	}
}

const char* CmdPackageProtocol::ProtoCmdTypeToName(int stype, int ctype)
{
	if (error)
		return nullptr;

	if (g_cmdMaps.find(stype) == g_cmdMaps.end())
		return nullptr;

	auto s_cmdMap = g_cmdMaps[stype];
	if (s_cmdMap->find(ctype) == s_cmdMap->end())
		return nullptr;

	return (*s_cmdMap)[ctype].c_str();
}

ProtoType CmdPackageProtocol::ProtoType()
{
	return g_protoType;
}

bool CmdPackageProtocol::DecodeBytesToRawPackage(unsigned char* cmd, const int cmd_len, RawPackage* out_msg)
{
	if (error)
		return false;

	// serviceType (2 bytes) | cmdType (2 bytes) | userTag (4 bytes) | body
	if (cmd_len < CMD_HEADER_SIZE)
	{// 数据太短
		log_debug("数据太短");
		return false;
	}
	out_msg->serviceType = cmd[0] | (cmd[1] << 8);
	out_msg->cmdType = cmd[2] | (cmd[3] << 8);
	out_msg->userTag = cmd[4] | (cmd[5] << 8) | (cmd[6] << 16) | (cmd[7] << 24);
	out_msg->rawCmd = cmd;
	out_msg->rawLen = cmd_len;

	return true;
}

bool CmdPackageProtocol::DecodeBytesToCmdPackage(unsigned char* cmd, const int cmd_len, CmdPackage*& out_msg)
{
	out_msg = NULL;

	if (error)
		return false;

	// serviceType (2 bytes) | cmdType (2 bytes) | userTag (4 bytes) | body
	if (cmd_len < CMD_HEADER_SIZE)
	{// 数据太短
		log_debug("数据太短");
		return false;
	}

	out_msg = (CmdPackage*)my_alloc(sizeof(CmdPackage));
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
		tempCharPointer = (char*)cache_alloc(writeBufAllocer, dataLen + 1);
		memcpy(tempCharPointer, cmd + CMD_HEADER_SIZE, dataLen);
		tempCharPointer[dataLen] = 0;
		out_msg->body = tempCharPointer;
		break;
	case ProtoType::Protobuf:
		//没有这个protobuf协议
		auto msgName = CmdPackageProtocol::ProtoCmdTypeToName(out_msg->serviceType, out_msg->cmdType);
		if (nullptr == msgName)
		{
			log_error("获取Message名失败为空, serviceType: %d, cmdType: %d", out_msg->serviceType, out_msg->cmdType);
			out_msg = NULL;
			return false;
		}


		tempMessagePointer = CreateMessage(msgName);
		if (tempMessagePointer == NULL)
		{
			log_error("获取Message类型为空, serviceType: %d, cmdType: %d, messageName: %s", out_msg->serviceType, out_msg->cmdType, msgName);
			my_free(tempMessagePointer);
			out_msg = NULL;
			return false;
		}

		if (!tempMessagePointer->ParseFromArray(cmd + CMD_HEADER_SIZE, cmd_len - CMD_HEADER_SIZE))
		{
			log_error("消息反序列化失败");
			my_free(out_msg);
			out_msg = NULL;
			ReleaseMessage(tempMessagePointer);
			return false;
		}

		out_msg->body = tempMessagePointer;


		break;
	}

	return true;
}

void CmdPackageProtocol::FreeCmdPackage(CmdPackage* msg)
{
	if (msg == NULL)
		return;
	if (msg->body)
	{
		switch (g_protoType)
		{
		case ProtoType::Json:
			cache_free(writeBufAllocer, msg->body);
			msg->body = NULL;
			break;
		case ProtoType::Protobuf:
			auto pm = (google::protobuf::Message*)msg->body;
			delete pm;
			msg->body = NULL;
			break;
		}
	}

	my_free(msg);
}

unsigned char* CmdPackageProtocol::EncodeCmdPackageToBytes(const CmdPackage* msg, int* out_len)
{

	if (error)
		return NULL;
	//加密


	unsigned char* rawData = NULL;

	char* tempCharPointer=NULL;
	int tempLen=0;
	google::protobuf::Message* tempMessagePointer=NULL;


	switch (g_protoType)
	{
	case ProtoType::Json:
		if (msg->body)
		{
			tempCharPointer = (char*)msg->body;
			tempLen = strlen(tempCharPointer);
		}

		rawData = (unsigned char*)cache_alloc(writeBufAllocer, CMD_HEADER_SIZE + tempLen);
		
		if (msg->body)
		{
			memcpy(rawData + CMD_HEADER_SIZE, tempCharPointer, tempLen);
		}
		break;
	case ProtoType::Protobuf:
		if (msg->body)
		{
			tempMessagePointer = (google::protobuf::Message*)msg->body;
			tempLen = tempMessagePointer->ByteSize();
		}

		rawData = (unsigned char*)cache_alloc(writeBufAllocer, CMD_HEADER_SIZE + tempLen);

		if(tempMessagePointer && !tempMessagePointer->SerializePartialToArray(rawData + CMD_HEADER_SIZE, tempLen))
		{// 如果序列化不成功
			cache_free(writeBufAllocer, rawData);
			*out_len = 0;
			return NULL;
		}

		//传出参数
		break;
	}
	

	*out_len = CMD_HEADER_SIZE + tempLen;

	//写包头信息
	rawData[0] = msg->serviceType & 0x000000ff;
	rawData[1] = (msg->serviceType & 0x0000ff00) >> 8;
	rawData[2] = msg->cmdType & 0x000000ff;
	rawData[3] = (msg->cmdType & 0x0000ff00) >> 8;

	//userTag
	memcpy(rawData + 4, &msg->userTag, 4);

	return rawData;
}

void CmdPackageProtocol::FreeCmdPackageBytes(unsigned char* raw) 
{
	cache_free(writeBufAllocer, raw);
}

google::protobuf::Message* CmdPackageProtocol::CreateMessage(const char* typeName)
{
	if (error)
		return nullptr;
	google::protobuf::Message* message = NULL;
	const google::protobuf::Descriptor* descriptor = importer->pool()->FindMessageTypeByName(typeName);//generated_pool
	if (descriptor) {
		const google::protobuf::Message* prototype = factory->GetPrototype(descriptor);
		if (prototype) {
			message = prototype->New();
		}
	}
	return message;
}

void CmdPackageProtocol::ReleaseMessage(google::protobuf::Message* msg)
{
	delete msg;
}
