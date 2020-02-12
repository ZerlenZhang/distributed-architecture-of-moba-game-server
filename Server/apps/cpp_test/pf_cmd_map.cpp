#include "pf_cmd_map.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"
#include <map>
#include <string>
using std::map;
using std::string;

static map<int, string> cmd_map = {
	{0,"LoginReq"},
	{1,"LoginRes"}
};

void InitPfCmdMap()
{
	CmdPackageProtocol::RegisterProtobufCmdMap(cmd_map);
}
