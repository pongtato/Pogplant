/******************************************************************************/
/*!
\file	CustomSaver.h
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
#ifndef CUSTOMSAVER_H_
#define CUSTOMSAVER_H_

#include <unordered_map>
#include <memory>
#include <mutex>
#include <string>
#include <json.h>

#define DOCUMENTSFOLDER "data.pog"
#define INTERNALFOLDER "Resources/internal.pog"

#pragma warning(disable : 4499)

namespace PPU
{
	/**************************************************************************/
	/*!
	\brief
		CustomSaver class to save/load data easily
	*/
	/**************************************************************************/
	class CustomSaver
	{
	public:
		inline ~CustomSaver()
		{
			SaveGame();
		}

		template <typename T>
		static void Append(const std::string& key, const T& value, bool saveAsDocuments = true);

		template <typename T>
		inline static T GetValue(const std::string& key, const T& defaultValue, bool loadFromDocuments = true);

		template <>
		inline static float GetValue<float>(const std::string& key, const float& defaultValue, bool loadFromDocuments);

		template <>
		inline static std::string GetValue<std::string>(const std::string& key, const std::string& defaultValue, bool loadFromDocuments);

		template <>
		inline static int GetValue<int>(const std::string& key, const int& defaultValue, bool loadFromDocuments);

		static Json::Value GetValueJson(const std::string& key, bool loadFromDocuments = true);

		static void Save();
		static void SaveGame();

		static CustomSaver& Instance();
	private:
		CustomSaver();

		void Load();

		/**> File directory of the save file*/
		std::wstring m_documentsFilePath;
		std::string m_internalFilePath;

		Json::Value m_internalJson;
		Json::Value m_documentJson;

		/**> singleton instance*/
		static std::unique_ptr<CustomSaver> m_instance;
		static std::once_flag m_onceFlag;
	};
}

#include "CustomSaver.hpp"

#endif // !CUSTOMSAVER_H
