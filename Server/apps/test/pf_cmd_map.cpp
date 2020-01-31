#include "pf_cmd_map.h"
#include "../../netbus/protocol/ProtoManager.h"

const char* pfCmdMap[] = {
	"LoginReq",
	"LoginRes",
};

void InitPfCmdMap()
{
	ProtoManager::RegisterPfCmdMap(pfCmdMap, sizeof(pfCmdMap) / sizeof(char*));
}
