#ifndef _LOGGER_H_
#define _LOGGER_H_

#include <list>
#include <unordered_map>
#include <string>
#include <sstream>

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

	static std::unordered_map<std::string, size_t> m_Keys;
	static std::list<LogEntry> m_Logs;
	static const size_t m_MaxLog;
};

#endif // _LOGGER_H_