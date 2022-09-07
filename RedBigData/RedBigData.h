#pragma once
#include <vector>
#include <string>
#include <filesystem>
#include "Dll.h"
#include "Database.h"
#include "Table.h"
#include <utility>

namespace RedBigData {
	class RedBigData {
	public:
		string m_path;
		Database* m_database;

		RedBigData(string path);

		Table GetTable(string table);
		std::pair<Column, size_t> GetColumn(string table, string column);

		void AddRow(string table, string column);
		void SetData(string table, string column, size_t index, char* data, int len);

		static __declspec(dllexport) RedBigData* Constructor(const char_t* path, int len);
		static __declspec(dllexport) void Destructor(RedBigData* ptr);
		static __declspec(dllexport) DllString GetPath(RedBigData* ptr);
		static __declspec(dllexport) void CreateDatabase(RedBigData* ptr, const char_t* database, int len);
		static __declspec(dllexport) DllArray GetDatabases(RedBigData* ptr);
		static __declspec(dllexport) void UseDatabase(RedBigData* ptr, const char_t* database, int len);
		static __declspec(dllexport) DllString GetDatabase(RedBigData* ptr);

		static __declspec(dllexport) DllArray GetTables(RedBigData* ptr);

		static __declspec(dllexport) DllTable GetTable(RedBigData* ptr, const char_t* table, int len);
		static __declspec(dllexport) void CreateTable(RedBigData* ptr, const char_t* table, int len, const char_t** columnsName, int* columnsNameLen, size_t* byteLen, size_t columnsLen);

		static __declspec(dllexport) DllSelect SelectColumn(RedBigData* ptr, const char_t* table, int len, const char_t* columnsName, int columnsNameLen);
		static __declspec(dllexport) void InsertColumn(RedBigData* ptr, const char_t* table, int len, const char_t* columnsName, int columnsNameLen, size_t index, char* data, int dataLen);
	};
}