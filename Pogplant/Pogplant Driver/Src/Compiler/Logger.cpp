#include "Logger.h"

std::unordered_map<std::string, size_t> Logger::m_Keys;
std::list<LogEntry> Logger::m_Logs;
const size_t Logger::m_MaxLog = 420;

void Logger::Log(LogEntry _LogEntry)
{
	// Pop front
	if (m_Logs.size() >= m_MaxLog)
	{
		// Remove keys
		m_Keys.erase(m_Keys.find(m_Logs.front().m_Description));
		// Then pop front
		m_Logs.pop_front();
	}

	// Prevent duplicates
	if (m_Keys[_LogEntry.m_Description] == 0)
	{
		m_Logs.push_back(_LogEntry);
		// Set key index to + 1 of actual index, we dont really care about the index.
		// Just to prevent dupes
		m_Keys[_LogEntry.m_Description] = m_Logs.size();
	}
}