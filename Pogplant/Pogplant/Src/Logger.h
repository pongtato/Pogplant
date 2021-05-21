#pragma once

//#include <list>
#include <unordered_map>
#include <string>
#include <sstream>

namespace Pogplant
{
	struct LogEntry
	{
		enum TYPE
		{
			ERROR,
			SUCCESS,
			WARNING,
			DEBUG_TEXT,
			COUNT
		};

		std::string m_Source;
		TYPE m_Type;
		std::string m_Description;
	};

	struct Logger
	{
		static void Log(LogEntry _LogEntry);

		static std::unordered_map<std::string,LogEntry> m_Logs;
		static const size_t m_MaxLog;
	};
}