#ifndef __WIN_UTIL_H__
#define __WIN_UTIL_H__


#include <functional>
#include <io.h>
#include <string>
using std::string;
//————————————————
//版权声明：本文为CSDN博主「liuqx0717」的原创文章，遵循 CC 4.0 BY - SA 版权协议，转载请附上原文出处链接及本声明。
//原文链接：https ://blog.csdn.net/liuqx97bb/article/details/77074833
enum class SearchType {
	ENUM_FILE = 1,
	ENUM_DIR,
	ENUM_BOTH
};

class WinUtil
{
public:
	static string GetDirPath(const string& filePath);
	static bool SearchFromDir(
		const std::wstring& dir_with_back_slant,  //根目录，比如: "L"C:\\", L"E:\\test\\"       
		const std::wstring& filename,             //比如: L"*", L"123.txt", L"*.exe", L"123.???"
		unsigned int maxdepth,                    //最大深度。0则不搜索子文件夹，-1则搜索所有子文件夹
		SearchType flags,                          //返回结果为文件，还是文件夹，还是都返回
		std::function<bool(const std::wstring & dir, _wfinddata_t & attrib)> callback //return true继续搜索，return false停止搜索
	);

	static bool SearchFromDir(
		const std::string& dir_with_back_slant,  //根目录，比如: "L"C:\\", L"E:\\test\\"       
		const std::string& filename,             //比如: L"*", L"123.txt", L"*.exe", L"123.???"
		unsigned int maxdepth,                    //最大深度。0则不搜索子文件夹，-1则搜索所有子文件夹
		SearchType flags,                          //返回结果为文件，还是文件夹，还是都返回
		std::function<bool(const std::string & dirPath, const string & fileName)> callback //return true继续搜索，return false停止搜索
	);
};

//返回值：
//如果callback返回false终止了搜索，则enumsubfiles也返回false，
//否则搜索完毕后返回true


#endif // !__WIN_UTIL_H__




