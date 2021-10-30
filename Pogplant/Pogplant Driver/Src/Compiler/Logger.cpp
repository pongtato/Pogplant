/*****************************************************************************/
/*!
\file	Logger.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Logger class that logs all necessary debug information to console

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

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

void Logger::Print()
{
	for (auto& logs : m_Logs)
	{
		std::string type;
		switch (logs.m_Type)
		{
		case LogEntry::LOGTYPE::ERROR:
			{
				type = "::ERROR";
			}
			break;
			case LogEntry::LOGTYPE::SUCCESS:
			{
				type = "::SUCCESS";
			}
			break;
			case LogEntry::LOGTYPE::WARNING:
			{
				type = "::WARNING";
			}
			break;
			case LogEntry::LOGTYPE::DEBUG_TEXT:
			{
				type = "::DEBUG_TEXT";
			}
			break;
			default:
			break;
		}
		std::cout << "[" << logs.m_Source << type << "] " << logs.m_Description << std::endl;
	}
}
