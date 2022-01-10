/******************************************************************************/
/*!
\file	CustomSaver.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	A save system meant to loosely replicate the playerprefs from unity

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

#define DOCUMENTSFOLDER "BogosBinted/settings.pog"
#define INTERNALFOLDER "Resources/internal.pog"

namespace PogplantDriver
{
	class CustomSaver
	{
	public:
		~CustomSaver() = default;

		template <typename T>
		static void Append(const std::string& key, const T& value, bool saveAsDocuments = true);

		template <typename T>
		static decltype(auto) GetValue(const std::string& key, bool loadFromDocuments = true);

		template <float>
		static float GetValue(const std::string& key, bool loadFromDocuments = true);

		static void Save();

		static CustomSaver& Instance();
	private:
		CustomSaver();

		/**> File directory of the save file*/
		std::wstring m_documentsSaveFile;
		std::string m_internalSaveFile;

		Json::Value m_internalJson;
		Json::Value m_documentJson;

		/**> singleton instance*/
		static std::unique_ptr<CustomSaver> m_instance;
		static std::once_flag m_onceFlag;
	};
}

#include "CustomSaver.hpp"

#endif // !CUSTOMSAVER_H
