#include "Logger.h"
/******************************************************************************/
/*!
\file	CustomSaver.hpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	A save system meant to loosely replicate the playerprefs from unity,
	and just converting to json format

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
namespace PPU
{
	/**************************************************************************/
	/*!
	\brief
		Adds some value to be saved into a file.
	\param key
		The key of the item
	\param value
		The value to store
	\param saveAsDocuments
		If true, will save the data to the save file in documents folder,
		otherwise, will save to internal.pog in Resources
	*/
	/**************************************************************************/
	template<typename T>
	void CustomSaver::Append(const std::string& key, const T& value, bool saveAsDocuments)
	{
		auto& instance = Instance();

		if (saveAsDocuments)
			instance.m_documentJson[key] = value;
		else
			instance.m_internalJson[key] = value;
	}

	/**************************************************************************/
	/*!
	\brief
		Base template function for GetValue, supposed to be specialized
		individually per type to call correct Json functions
	*/
	/**************************************************************************/
	template<typename T>
	T CustomSaver::GetValue(const std::string& key, const T& defaultValue, bool loadFromDocuments)
	{
		(void)key;
		(void)loadFromDocuments;
		throw;
		//return T{};
	}

	/**************************************************************************/
	/*!
	\brief
		Attempts to get a float from the save file with the key

	\param key
		The key of the item

	\param defaultValue
		The default value to be returned if the key is not found

	\param loadFromDocuments
		If true, will load the data from the save file in documents folder,
		otherwise, will load from internal.pog in Resources

	\return
		Returns the value from the save file, if not found returns default
	*/
	/**************************************************************************/
	template <>
	float CustomSaver::GetValue<float>(const std::string& key, const float& defaultValue, bool loadFromDocuments)
	{
		auto& instance = Instance();

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
			Pogplant::LogEntry{ "CustomSaver::GetValue<float>", Pogplant::LogEntry::LOGTYPE::WARNING, ss.str() }, true);

		return defaultValue;
	}

	/**************************************************************************/
	/*!
	\brief
		Attempts to get a string from the save file with the key

	\param key
		The key of the item

	\param defaultValue
		The default value to be returned if the key is not found

	\param loadFromDocuments
		If true, will load the data from the save file in documents folder,
		otherwise, will load from internal.pog in Resources

	\return
		Returns the value from the save file, if not found returns default
	*/
	/**************************************************************************/
	template <>
	std::string CustomSaver::GetValue<std::string>(const std::string& key, const std::string& defaultValue, bool loadFromDocuments)
	{
		auto& instance = Instance();

		if (loadFromDocuments)
		{
			if (instance.m_documentJson[key])
				return instance.m_documentJson[key].asString();
		}
		else
		{
			if (instance.m_internalJson[key])
				return instance.m_internalJson[key].asString();
		}

		std::stringstream ss;

		ss << "Unable to load key: " << key;

		Pogplant::Logger::Log(
			Pogplant::LogEntry{ "CustomSaver::GetValue<std::string>", Pogplant::LogEntry::LOGTYPE::WARNING, ss.str() }, true);

		return defaultValue;
	}

	/**************************************************************************/
	/*!
	\brief
		Attempts to get a int from the save file with the key

	\param key
		The key of the item

	\param defaultValue
		The default value to be returned if the key is not found

	\param loadFromDocuments
		If true, will load the data from the save file in documents folder,
		otherwise, will load from internal.pog in Resources

	\return
		Returns the value from the save file, if not found returns default
	*/
	/**************************************************************************/
	template <>
	int CustomSaver::GetValue<int>(const std::string& key, const int& defaultValue, bool loadFromDocuments)
	{
		auto& instance = Instance();

		if (loadFromDocuments)
		{
			if (instance.m_documentJson[key])
				return instance.m_documentJson[key].asInt();
		}
		else
		{
			if (instance.m_internalJson[key])
				return instance.m_internalJson[key].asInt();
		}

		std::stringstream ss;

		ss << "Unable to load key: " << key;

		Pogplant::Logger::Log(
			Pogplant::LogEntry{ "CustomSaver::GetValue<int>", Pogplant::LogEntry::LOGTYPE::WARNING, ss.str() }, true);

		return defaultValue;
	}


	/**************************************************************************/
	/*!
	\brief
		Attempts to get a uint from the save file with the key

	\param key
		The key of the item

	\param defaultValue
		The default value to be returned if the key is not found

	\param loadFromDocuments
		If true, will load the data from the save file in documents folder,
		otherwise, will load from internal.pog in Resources

	\return
		Returns the value from the save file, if not found returns default
	*/
	/**************************************************************************/
	template <>
	unsigned int CustomSaver::GetValue<unsigned int>(const std::string& key, const unsigned int& defaultValue, bool loadFromDocuments)
	{
		auto& instance = Instance();

		if (loadFromDocuments)
		{
			if (instance.m_documentJson[key])
				return instance.m_documentJson[key].asUInt();
		}
		else
		{
			if (instance.m_internalJson[key])
				return instance.m_internalJson[key].asUInt();
		}

		std::stringstream ss;

		ss << "Unable to load key: " << key;

		Pogplant::Logger::Log(
			Pogplant::LogEntry{ "CustomSaver::GetValue<uint>", Pogplant::LogEntry::LOGTYPE::WARNING, ss.str() }, true);

		return defaultValue;
	}
}
