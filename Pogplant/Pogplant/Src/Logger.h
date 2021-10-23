#pragma once

#include <list>
#include <unordered_map>
#include <string>
#include <sstream>

namespace Pogplant
{
	struct LogEntry
	{
		enum class LOGTYPE
		{
			ERROR,
			SUCCESS,
			WARNING,
			DEBUG_TEXT,
			COUNT
		};

		std::string m_Source;
		LOGTYPE m_Type;
		std::string m_Description;
	};

	struct Logger
	{
		static void Log(LogEntry _LogEntry, bool printDuplicates = false);

		static std::unordered_map<std::string, size_t> m_Keys;
		static std::list<LogEntry> m_Logs;
		static const size_t m_MaxLog;
	};
}