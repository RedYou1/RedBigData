#pragma once
#include "Dll.h"
#include <filesystem>

namespace RedBigData {
	class Database {
	public:
		string m_path;

		Database(string path);
	};
}