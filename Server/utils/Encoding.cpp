#include "Encoding.h"

string Encoding::UnicodeToUTF8(const wstring& sour)
{
	std::wstring_convert<std::codecvt_utf8<wchar_t>> conv;
	return conv.to_bytes(sour);
}

wstring Encoding::UTF8ToUnicode(const string& sour)
{
	std::wstring_convert<std::codecvt_utf8<wchar_t>> conv;
	return conv.from_bytes(sour);
}

wstring Encoding::AsciiToUnicode(const string& sour)
{
	int unicodeLen = MultiByteToWideChar(CP_ACP, 0, sour.c_str(), -1, nullptr, 0);
	wchar_t* pUnicode = (wchar_t*)malloc(sizeof(wchar_t) * unicodeLen);
	MultiByteToWideChar(CP_ACP, 0, sour.c_str(), -1, pUnicode, unicodeLen);
	std::wstring ret_str = pUnicode;
	free(pUnicode);
	return ret_str;
}

string Encoding::UnicodeToAscii(const wstring& sour)
{
	int ansiiLen = WideCharToMultiByte(CP_ACP, 0, sour.c_str(), -1, nullptr, 0, nullptr, nullptr);
	char* pAssii = (char*)malloc(sizeof(char) * ansiiLen);
	WideCharToMultiByte(CP_ACP, 0, sour.c_str(), -1, pAssii, ansiiLen, nullptr, nullptr);
	std::string ret_str = pAssii;
	free(pAssii);
	return ret_str;
}

string Encoding::AsciiToUTF8(const string& sour)
{
	return UnicodeToUTF8(AsciiToUnicode(sour));
}

string Encoding::UTF8ToAscii(const string& sour)
{
	return UnicodeToAscii(UTF8ToUnicode(sour));
}
