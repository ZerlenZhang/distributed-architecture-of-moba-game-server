#include "../../netbus/session/AbstractSession.h"
#include "../../netbus/protocol/CmdPackageProtocol.h"
#include "AbstractService.h"

AbstractService::AbstractService()
{
	useRawPackage = false;
}

bool AbstractService::OnSessionRecvRawPackage(const AbstractSession* session, const RawPackage* package) const
{
	return false;
}

// if return false, close socket
bool AbstractService::OnSessionRecvCmdPackage(const AbstractSession* session, const CmdPackage* package) const
{
	return false;
}

bool AbstractService::OnSessionDisconnected(const AbstractSession* session, const int& serviceType) const
{
	return false;
}
