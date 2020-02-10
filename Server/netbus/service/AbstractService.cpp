#include "../../netbus/session/AbstractSession.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"
#include "AbstractService.h"

// if return false, close socket
bool AbstractService::OnSessionRecvCmd(const AbstractSession* session, const CmdPackage* package) const
{
	return false;
}

bool AbstractService::OnSessionDisconnected(const AbstractSession* session) const
{
	return false;
}
