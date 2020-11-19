#include "AbstractSession.h"

AbstractSession::AbstractSession()
{
	this->isClient = false;
	this->utag = 0;
	this->uid = 0;
}

void AbstractSession::Enable()
{
}

void AbstractSession::Disable()
{
}
