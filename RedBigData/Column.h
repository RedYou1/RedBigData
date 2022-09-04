#pragma once
#include <filesystem>
#include "Dll.h"

namespace RedBigData {
	struct Column {
		string m_name;

		// 0 = dynamic
		size_t m_byteLen;
	};
	struct DllColumn {
		DllString m_name;
		size_t m_byteLen;
	};
	DllColumn ConvertColumn(Column column);
}