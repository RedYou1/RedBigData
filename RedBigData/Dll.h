#pragma once
#include <string>

namespace RedBigData {
	typedef std::u16string string;
	typedef char16_t char_t;
	struct DllArray
	{
		void* start;
		size_t len;
	};
	struct DllString
	{
		char16_t* start;
		size_t len;
	};
	DllString convertString(string s);
	string stringConvert(DllString s);
}