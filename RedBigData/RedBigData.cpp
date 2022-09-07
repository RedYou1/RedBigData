#include "RedBigData.h"
#include <iostream>
#include <fstream>

RedBigData::RedBigData::RedBigData(string path)
	: m_path(path), m_database(nullptr)
{}

RedBigData::Table RedBigData::RedBigData::GetTable(string table)
{
	if (m_database == nullptr)
		throw "no database selected";

	string path{ m_database->m_path + u'\\' + table + u"\\info" };

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
	return Table(columns, rows);
}

std::pair<RedBigData::Column, size_t> RedBigData::RedBigData::GetColumn(string table, string column)
{
	if (m_database == nullptr)
		throw "no database selected";

	string path{ m_database->m_path + u'\\' + table + u"\\info" };

	if (!std::filesystem::exists(path))
		throw "not a table";

	std::ifstream file(std::filesystem::path(path), std::ifstream::in | std::ifstream::binary);
	size_t rows{ 0 };
	file.read((char*)&rows, sizeof(size_t));
	size_t columnsLen{ 0 };
	file.read((char*)&columnsLen, sizeof(size_t));
	for (size_t i{ 0 }; i < columnsLen; i++) {
		size_t nameLen{ 0 };
		file.read((char*)&nameLen, sizeof(size_t));
		char_t* name{ new char_t[nameLen] };
		file.read((char*)name, sizeof(char_t) * nameLen);
		size_t byteLen{ 0 };
		file.read((char*)&byteLen, sizeof(size_t));

		string cName{ string(name, nameLen) };
		if (cName == column) {
			file.close();
			return std::make_pair(Column(cName, byteLen), rows);
		}
	}
	file.close();
	throw u"Not found " + path + u" " + column;
}

void RedBigData::RedBigData::AddRow(string table, string column)
{
	string path{ m_database->m_path + u'\\' + table + u"\\info" };

	if (!std::filesystem::exists(path))
		throw "table not found";

	std::ifstream filei(std::filesystem::path(path), std::ifstream::in | std::ifstream::binary);
	size_t rows{ 0 };
	filei.read((char*)&rows, sizeof(size_t));
	size_t columnsLen;
	filei.read((char*)&columnsLen, sizeof(size_t));
	for (size_t i{ 0 }; i < columnsLen; i++) {
		size_t nameLen{ 0 };
		filei.read((char*)&nameLen, sizeof(size_t));
		char_t* name{ new char_t[nameLen] };
		filei.read((char*)name, sizeof(char_t) * nameLen);
		size_t byteLen{ 0 };
		filei.read((char*)&byteLen, sizeof(size_t));

		if (byteLen == 0) {
			std::string s{ std::to_string(rows) };
			string path2{ m_database->m_path + u'\\' + table + u'\\' + string(name, nameLen) + u'\\' };
			for (auto c = s.begin(); c != s.end(); c++) {
				path2 += *c;
			}
			std::ofstream fileo(std::filesystem::path(path2), std::ofstream::out | std::ofstream::binary);
			fileo.close();
		}
		else {
			string path2{ m_database->m_path + u'\\' + table + u'\\' + string(name, nameLen) };
			std::ofstream fileo(std::filesystem::path(path2), std::ofstream::out | std::ofstream::binary | std::ofstream::app);
			char* c{ new char[byteLen] };
			for (size_t t{ 0 }; t < byteLen; t++) {
				c[t] = '\x0';
			}
			fileo.write(c, byteLen);
			fileo.close();
		}
	}
	filei.close();
	rows++;

	size_t fs{ std::filesystem::file_size(path) - sizeof(size_t) };
	char* data{ new char[fs] };
	filei = std::ifstream(std::filesystem::path(path), std::ifstream::in | std::ifstream::binary);
	filei.seekg(sizeof(size_t));
	filei.read((char*)data, fs);
	filei.close();

	std::ofstream fileo(std::filesystem::path(path), std::ofstream::out | std::ofstream::binary);
	fileo.write((char*)&rows, sizeof(size_t));
	fileo.write(data, fs);
	fileo.close();
}

void RedBigData::RedBigData::SetData(string table, string column, size_t index, char* data, int len)
{
	std::pair<Column, size_t> pair{ GetColumn(table, column) };

	if (index >= pair.second) {
		string a{ pair.first.m_name };
		std::string temp{ " out of bound " + std::to_string(index) + " sur " + std::to_string(pair.second) };
		throw a + string(temp.begin(), temp.end());
	}

	if (pair.first.m_byteLen == 0) {
		std::string s{ std::to_string(index) };
		string path{ m_database->m_path + u'\\' + table + u'\\' + column + u'\\' };
		for (auto c = s.begin(); c != s.end(); c++) {
			path += *c;
		}
		std::ofstream fileo(std::filesystem::path(path), std::ofstream::out | std::ofstream::binary);
		fileo.write(data, len);
		fileo.close();
	}
	else {
		if (pair.first.m_byteLen != len) {
			throw "not same len";
		}

		string path{ m_database->m_path + u'\\' + table + u'\\' + column };

		size_t fsize{ std::filesystem::file_size(path) };
		char* t{ new char[fsize] };
		std::ifstream filei(std::filesystem::path(path), std::ifstream::in | std::ifstream::binary);
		filei.read(t, fsize);
		filei.close();

		std::ofstream fileo(std::filesystem::path(path), std::ofstream::out | std::ofstream::binary);
		fileo.write(t, index * len);
		fileo.write(data, len);
		fileo.write(&t[(index + 1) * len], fsize - (index + 1) * len);
		fileo.close();
		delete[] t;
	}
}

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
		return ConvertTable(ptr->GetTable(string(name, len)));
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
			if (byteLen[i] != 0)
			{
				std::ofstream f(std::filesystem::path(tablePath + u"\\" + string(columnsNames[i], columnsNamesLen[i])),
					std::ofstream::out | std::ofstream::binary);
				f.close();
			}
		}
		file.close();
	}

	RedBigData::DllSelect RedBigData::RedBigData::SelectColumn(RedBigData* ptr, const char_t* tableName, int tableNameLen, const char_t* columnName, int columnNameLen)
	{
		string t{ string(tableName,tableNameLen) };
		string c{ string(columnName,columnNameLen) };
		std::pair<Column, size_t> pair{ ptr->GetColumn(t,c) };
		string path{ ptr->m_database->m_path + u'\\' + t + u'\\' + c };
		if (pair.first.m_byteLen == 0) {
			size_t size1{ pair.second };
			DllArray* data{ (DllArray*)malloc(sizeof(DllArray) * size1) };

			for (size_t i{ 0 }; i < size1; i++) {

				string path2{ path };
				std::string is{ std::to_string(i) };
				for (size_t s{ 0 }; s < is.length(); s++) {
					path2 += is[s];
				}

				size_t size2{ std::filesystem::file_size(path2) };
				char* data2{ (char*)malloc(size2) };
				std::ifstream file{ std::filesystem::path(path2) };
				file.read((char*)data, size2);
				file.close();

				data[i] = DllArray(data2, size2);
			}

			return DllSelect(DllArray(data, size1), pair.first.m_byteLen);
		}
		else {
			size_t size{ pair.first.m_byteLen * pair.second };
			void* data{ malloc(size) };
			std::ifstream file{ std::filesystem::path(path) };
			file.read((char*)data, size);
			file.close();
			return DllSelect(DllArray(data, pair.second), pair.first.m_byteLen);
		}
	}

	void RedBigData::RedBigData::InsertColumn(RedBigData* ptr, const char_t* tableC, int len, const char_t* columnsName, int columnsNameLen, size_t index, char* data, int dataLen)
	{
		string tableName{ string(tableC, len) };
		string columnName{ string(columnsName, columnsNameLen) };
		std::pair<Column, size_t> pair{ ptr->GetColumn(tableName, columnName) };

		if (pair.first.m_byteLen != dataLen && pair.first.m_byteLen != 0) {
			throw "wrong data len";
		}

		while (index >= pair.second) {
			ptr->AddRow(tableName, columnName);
			pair.second++;
		}

		ptr->SetData(tableName, columnName, index, data, dataLen);
	}
}
