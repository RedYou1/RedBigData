#include "RedBigData.h"
#include <iostream>
#include <fstream>

RedBigData::RedBigData::RedBigData(string path)
	: m_path(path), m_database(nullptr)
{}

extern "C"
{
	RedBigData::RedBigData*
		RedBigData::RedBigData::Constructor(const char_t* dllPath, int len)
	{
		string path{ string(dllPath,len) };
		if (!std::filesystem::exists(path) || !std::filesystem::is_directory(path)) {
			throw "path doesn't already exists or not a directory";
		}
		return new RedBigData(path);
	}

	void RedBigData::RedBigData::Destructor(RedBigData* ptr)
	{
		delete ptr->m_database;
		delete ptr;
	}

	RedBigData::DllString RedBigData::RedBigData::GetPath(RedBigData* ptr)
	{
		return convertString(ptr->m_path);
	}

	void RedBigData::RedBigData::CreateDatabase(RedBigData* ptr, const char_t* database, int len)
	{
		string path{ ptr->m_path + u"\\" + string(database,len) };
		if (std::filesystem::exists(path)) {
			throw "path already exists";
		}
		std::filesystem::create_directories(path);
	}

	RedBigData::DllArray RedBigData::RedBigData::GetDatabases(RedBigData* ptr)
	{
		std::vector<string> res{};

		for (auto entry : std::filesystem::directory_iterator(ptr->m_path))
			res.push_back(entry.path().u16string());

		size_t size{ res.size() };
		DllString* strings{ new DllString[size] };
		size_t i{ 0 };
		for (auto it{ res.begin() }; it != res.end(); it++)
		{
			strings[i] = convertString(*it);
			i++;
		}
		return { strings,size };
	}

	void RedBigData::RedBigData::UseDatabase(RedBigData* ptr, const char_t* database, int len)
	{
		string path{ ptr->m_path + u"\\" + string(database,len) };
		if (!std::filesystem::exists(path) || !std::filesystem::is_directory(path)) {
			throw "path doesn't already exists or not a directory";
		}
		ptr->m_database = new Database(path);
	}

	RedBigData::DllString RedBigData::RedBigData::GetDatabase(RedBigData* ptr)
	{
		if (ptr->m_database == nullptr)
			throw "no database selected";
		return convertString(ptr->m_database->m_path);
	}

	RedBigData::DllArray RedBigData::RedBigData::GetTables(RedBigData* ptr)
	{
		if (ptr->m_database == nullptr)
			throw "no database selected";

		std::vector<string> res{};

		for (auto entry : std::filesystem::directory_iterator(ptr->m_database->m_path))
			res.push_back(entry.path().u16string());

		size_t size{ res.size() };
		DllString* strings{ new DllString[size] };
		size_t i{ 0 };
		for (auto it{ res.begin() }; it != res.end(); it++)
		{
			strings[i] = convertString(*it);
			i++;
		}
		return { strings,size };
	}

	RedBigData::DllTable RedBigData::RedBigData::GetTable(RedBigData* ptr, const char_t* name, int len)
	{
		if (ptr->m_database == nullptr)
			throw "no database selected";

		string path{ ptr->m_database->m_path + u'\\' + string(name,len) + u"\\info" };

		if (!std::filesystem::exists(path))
			throw "not a table";

		std::ifstream file(std::filesystem::path(path), std::ifstream::in | std::ifstream::binary);
		size_t rows;
		file.read((char*)&rows, sizeof(size_t));
		size_t columnsLen;
		file.read((char*)&columnsLen, sizeof(size_t));
		std::vector<Column> columns{ columnsLen };
		for (size_t i{ 0 }; i < columnsLen; i++) {
			size_t nameLen{ 0 };
			file.read((char*)&nameLen, sizeof(size_t));
			char_t* name{ new char_t[nameLen] };
			file.read((char*)name, sizeof(char_t) * nameLen);
			size_t byteLen{ 0 };
			file.read((char*)&byteLen, sizeof(size_t));
			columns[i] = Column(string(name, nameLen), byteLen);
		}
		file.close();
		return ConvertTable(Table(columns, rows));
	}

	void RedBigData::RedBigData::CreateTable(RedBigData* ptr, const char_t* tableName, int len, const char_t** columnsNames, int* columnsNamesLen, size_t* byteLen, size_t columnsLen)
	{
		if (ptr->m_database == nullptr)
			throw "no database selected";

		string tablePath{ ptr->m_database->m_path + u'\\' + string(tableName,len) };

		if (std::filesystem::exists(tablePath))
			throw "table already exists";

		std::filesystem::create_directories(tablePath);
		string path{ tablePath + u"\\info" };

		if (std::filesystem::exists(path))
			throw "table already exists";

		std::ofstream file(std::filesystem::path(path), std::ofstream::out | std::ofstream::binary);

		if (!file.is_open())
			throw "won't open file";

		size_t rows{ 0 };
		file.write((char*)&rows, sizeof(size_t));
		file.write((char*)&columnsLen, sizeof(size_t));
		for (size_t i{ 0 }; i < columnsLen; i++) {
			file.write((char*)&columnsNamesLen[i], sizeof(size_t));
			file.write((char*)columnsNames[i], sizeof(char_t) * columnsNamesLen[i]);
			file.write((char*)&byteLen[i], sizeof(size_t));
		}
		file.close();
	}
}
