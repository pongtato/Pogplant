#include "Logger.h"
/******************************************************************************/
/*!
\file	CustomSaver.hpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	A save system meant to loosely replicate the playerprefs from unity

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
namespace PogplantDriver
{
	template<typename T>
	void CustomSaver::Append(const std::string& key, const T& value, bool saveAsDocuments)
	{
		auto instance = Instance();

		if (saveAsDocuments)
			instance.m_documentJson[key] = value;
		else
			instance.m_internalJson[key] = value;
	}

	template<typename T>
	decltype(auto) CustomSaver::GetValue(const std::string& key, bool loadFromDocuments)
	{
		assert(false && "Unsupported type!!");
	}

	template <float>
	static float CustomSaver::GetValue(const std::string& key, bool loadFromDocuments)
	{
		auto instance = Instance();

		if (loadFromDocuments)
		{
			if (instance.m_documentJson[key])
				return instance.m_documentJson[key].asFloat();
		}
		else
		{
			if (instance.m_internalJson[key])
				return instance.m_internalJson[key].asFloat();
		}

		std::stringstream ss;


		ss << "Unable to load key: " << key;

		Pogplant::Logger::Log(
			Pogplant::LogEntry{ "CustomSaver::GetValue", PP::LogEntry::LOGTYPE::ERROR, ss.str() }, true);

		return 0.f;
	}
}