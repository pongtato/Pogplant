#include "Logger.h"

namespace Pogplant
{
	std::list<LogEntry> Logger::m_Logs;
	const size_t Logger::m_MaxLog = 50;

	void Logger::Log(LogEntry _LogEntry)
	{
		if (m_Logs.size() >= m_MaxLog)
		{
			m_Logs.pop_front();
		}

		m_Logs.push_back(_LogEntry);
	}
}