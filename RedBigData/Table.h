#pragma once
#include "Dll.h"
#include "Column.h"
#include <vector>

namespace RedBigData {
	struct Table {
		std::vector<Column> m_columns;

		size_t m_rows;
	};
	struct DllTable {
		DllArray m_columns;
		size_t m_rows;
	};
	DllTable ConvertTable(Table table);
}