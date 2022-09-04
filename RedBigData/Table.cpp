#include "Table.h"

RedBigData::DllTable RedBigData::ConvertTable(Table table)
{
	size_t size{ table.m_columns.size() };
	DllColumn* column{ new DllColumn[size] };
	for (size_t i{ 0 }; i < size; i++) {
		column[i] = DllColumn(convertString(table.m_columns[i].m_name),
			table.m_columns[i].m_byteLen);
	}
	return DllTable(DllArray(column, size), table.m_rows);
}
