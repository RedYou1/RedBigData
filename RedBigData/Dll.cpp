#include "Dll.h"

RedBigData::DllString RedBigData::convertString(string s)
{
	size_t size{ s.size() };
	char_t* c = new char_t[size];
	for (size_t i = 0; i < size; i++) {
		c[i] = s[i];
	}
	return { c,size };
}

RedBigData::string RedBigData::stringConvert(DllString s)
{
	string a{ u"" };
	for (size_t i{ 0 }; i < s.len; i++) {
		a += s.start[i];
	}
	return a;
}