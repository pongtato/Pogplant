/*****************************************************************************/
/*!
\file	Logger.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Logger class that logs all necessary debug information to console

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#ifndef _LOGGER_H_
#define _LOGGER_H_

#include <list>
#include <unordered_map>
#include <string>
#include <sstream>
#include <iostream>

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
	static void Log(LogEntry _LogEntry);
	static void Print();
	static std::unordered_map<std::string, size_t> m_Keys;
	static std::list<LogEntry> m_Logs;
	static const size_t m_MaxLog;
};

#endif // _LOGGER_H_