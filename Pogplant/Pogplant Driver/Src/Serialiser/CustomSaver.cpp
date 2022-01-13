/******************************************************************************/
/*!
\file	CustomSaver.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	A save system meant to loosely replicate the playerprefs from unity

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "CustomSaver.h"
#include <fstream>

#include <filesystem>

#define NOMINMAX
#include <Windows.h>

#include <fileapi.h>
#include <ShlObj.h>

#include <KnownFolders.h>
#include <wchar.h>

std::unique_ptr<PPU::CustomSaver> PPU::CustomSaver::m_instance = nullptr;
std::once_flag PPU::CustomSaver::m_onceFlag;

namespace PPU
{
	/**************************************************************************/
	/*!
	\brief
		Gets the CustomSaver singleton instance
	*/
	/**************************************************************************/
	CustomSaver& CustomSaver::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new CustomSaver);
			});

		return *m_instance.get();
	}

	/**************************************************************************/
	/*!
	\brief
		Default constructor
	*/
	/**************************************************************************/
	CustomSaver::CustomSaver()
	{
		std::wstringstream ss;

		wchar_t* Documentspath = 0;

		HRESULT hr = SHGetKnownFolderPath(FOLDERID_Documents, 0, NULL, &Documentspath);

		if (SUCCEEDED(hr))
		{
			ss << Documentspath << "\\TakoFactory\\";
			CreateDirectoryW(ss.str().c_str(), NULL);
			ss << DOCUMENTSFOLDER;
		}
		else
		{
			ss << DOCUMENTSFOLDER;
		}

		CoTaskMemFree(static_cast<void*>(Documentspath));

		m_documentsFilePath = ss.str();
		m_internalFilePath = INTERNALFOLDER;

		//load Data
		Load();
	}

	/**************************************************************************/
	/*!
	\brief
		Loads the configured files into the Json::Value for easy access
	*/
	/**************************************************************************/
	void CustomSaver::Load()
	{
		if (std::filesystem::exists(m_documentsFilePath))
		{
			std::ifstream documentStream(m_documentsFilePath, std::ios::in);

			if (documentStream.is_open())
			{
				documentStream >> m_documentJson;

				documentStream.close();
			}
		}

		if(std::filesystem::exists(m_internalFilePath))
		{
			std::ifstream internalStream(m_internalFilePath, std::ios::in);
			if (internalStream.is_open())
			{
				internalStream >> m_internalJson;

				internalStream.close();
			}
		}
	}

#undef ERROR

	/**************************************************************************/
	/*!
	\brief
		Attempts to get a Json::Value from the save file with the key
	\param key
		The key of the item
	\param loadFromDocuments
		If true, will load the data from the save file in documents folder,
		otherwise, will load from internal.pog in Resources
	*/
	/**************************************************************************/
	Json::Value CustomSaver::GetValueJson(const std::string& key, bool loadFromDocuments)
	{
		auto& instance = Instance();

		if (loadFromDocuments)
		{
			if (instance.m_documentJson[key])
				return instance.m_documentJson[key];
		}
		else
		{
			if (instance.m_internalJson[key])
				return instance.m_internalJson[key];
		}

		std::stringstream ss;

		ss << "Unable to load key: " << key;

		Pogplant::Logger::Log(
			Pogplant::LogEntry{ "CustomSaver::GetValueJson", Pogplant::LogEntry::LOGTYPE::ERROR, ss.str() }, true);

		return Json::Value{};
	}

	/**************************************************************************/
	/*!
	\brief
		Saves all the data to both documents and resources folder
	*/
	/**************************************************************************/
	void CustomSaver::Save()
	{
		auto& instance = Instance();

		std::ofstream documentStream(instance.m_documentsFilePath, std::ios::out | std::ios::trunc);

		if (documentStream.is_open())
		{
			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(instance.m_documentJson, &documentStream);

			delete writer;
			documentStream.close();
		}

		std::ofstream internalStream(instance.m_internalFilePath, std::ios::out | std::ios::trunc);

		if (internalStream.is_open())
		{
			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(instance.m_internalJson, &internalStream);

			delete writer;
			internalStream.close();
		}

	}
}