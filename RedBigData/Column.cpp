#include "Column.h"

RedBigData::DllColumn RedBigData::ConvertColumn(Column column)
{
	return DllColumn(convertString(column.m_name), column.m_byteLen);
}
