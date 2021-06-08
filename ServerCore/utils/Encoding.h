#pragma once
#ifndef __Encoding_H__
#define __Encoding_H__
#include <windows.h>
#include <string>
#include <iostream>
#include <locale>
#include <codecvt>
#include <fstream>
using namespace std;
class Encoding
{
public:
	static string UnicodeToUTF8(const wstring& sour);
	static wstring UTF8ToUnicode(const string& sour);
	static wstring AsciiToUnicode(const string& sour);
	static string UnicodeToAscii(const wstring& sour);
	static string AsciiToUTF8(const string& sour);
	static string UTF8ToAscii(const string& sour);
};
#endif // !__Encoding_H__


