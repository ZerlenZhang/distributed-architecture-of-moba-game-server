#include "DebugUtil.h"
#include "../logger/logger.h"
#include <iostream>
#include <strstream>
#include <sstream>
#include <strstream>
#include <string>
using namespace std;
void DebugUtil::LogByteArray(unsigned char* data, int len)
{
	ostringstream os;
	for (auto i = 0; i < len; i++)
	{
		os << (int)(data[i]);
	}
	log_debug(os.str().c_str());
}
