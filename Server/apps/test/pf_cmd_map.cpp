#include "pf_cmd_map.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"

const char* pfCmdMap[] = {
	"LoginReq",
	"LoginRes",
};

void InitPfCmdMap()
{
	CmdPackageProtocol::RegisterPfCmdMap(pfCmdMap, sizeof(pfCmdMap) / sizeof(char*));
}
