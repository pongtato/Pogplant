#include "Logger.h"

namespace Pogplant
{
	std::unordered_map<std::string, LogEntry> Logger::m_Logs;
	const size_t Logger::m_MaxLog = 50;

	void Logger::Log(LogEntry _LogEntry)
	{
		// Pop front
		if (m_Logs.size() >= m_MaxLog)
		{
			m_Logs.erase(m_Logs.begin());
		}

		// This way description duplicates will just act as key and not contribute to size
		m_Logs[_LogEntry.m_Description] = _LogEntry;
	}
}