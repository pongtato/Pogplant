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

std::unique_ptr<PogplantDriver::CustomSaver> PogplantDriver::CustomSaver::m_instance = nullptr;
std::once_flag PogplantDriver::CustomSaver::m_onceFlag;

namespace PogplantDriver
{
	CustomSaver& CustomSaver::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new CustomSaver);
			});

		return *m_instance.get();
	}

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

		m_documentsSaveFile = ss.str();
		m_internalSaveFile = INTERNALFOLDER;

		//load Data
	}

	void CustomSaver::Save()
	{
		auto& instance = Instance();

		std::ofstream documentStream(DOCUMENTSFOLDER, std::ios::out);

		if (documentStream.is_open())
		{
			/*for (int i = 0; i < root.size(); ++i)
			{
				Json::Value subroot = SaveComponents(*entity);
				root[i] = subroot;
			}//*/

			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(instance.m_documentJson, &documentStream);

			delete writer;
			documentStream.close();
		}

		std::ofstream internalStream(DOCUMENTSFOLDER, std::ios::out);

		if (internalStream.is_open())
		{
			/*for (int i = 0; i < root.size(); ++i)
			{
				Json::Value subroot = SaveComponents(*entity);
				root[i] = subroot;
			}//*/

			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(instance.m_internalJson, &internalStream);

			delete writer;
			internalStream.close();
		}

	}
}