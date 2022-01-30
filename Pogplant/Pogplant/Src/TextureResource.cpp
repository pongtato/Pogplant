#include "TextureResource.h"
#include "TextureLoader.h"
#include "Logger.h"

#include <json.h>
#include <fstream>
#include <iostream>

namespace Pogplant
{
	std::unordered_map<std::string, unsigned int> TextureResource::m_TexturePool;
	std::unordered_map<std::string, UsedTex> TextureResource::m_UsedTextures;
	bool TextureResource::m_Updated = false;
	unsigned int TextureResource::m_UsedTexCount = 0;

	void HandleCubemaps(const Json::Value& _Root)
	{
		Json::ValueConstIterator iter = _Root.begin();
		while (iter != _Root.end())
		{
			Json::Value subroot = _Root[iter.index()];
			auto& j_fileNames = subroot["FileName"];
			auto& j_dir = subroot["Dir"];

			std::vector<std::string> fileNames;
			std::string dir = "";

			if (j_fileNames)
			{
				Json::ValueConstIterator fileIt = j_fileNames.begin();
				while (fileIt != j_fileNames.end())
				{
					auto& fileName = j_fileNames[fileIt.index()];
					fileNames.push_back(fileName.asString());
					fileIt++;
				}
			}

			if (j_dir)
			{
				dir = j_dir.asString();
			}

			// Load
			TexLoader::LoadCubemap(fileNames, dir);

			iter++;
		}
	}

	void HandleSRGB(const Json::Value& _Root)
	{
		Json::ValueConstIterator iter = _Root.begin();
		while (iter != _Root.end())
		{
			Json::Value subroot = _Root[iter.index()];
			auto& j_fileName = subroot["FileName"];
			auto& j_dir = subroot["Dir"];
			auto& j_alpha = subroot["Alpha"];

			std::string fileName = "";
			std::string dir = "";
			bool alpha = false;

			if (j_fileName)
			{
				fileName = j_fileName.asString();
			}

			if (j_dir)
			{
				dir = j_dir.asString();
			}

			if (j_alpha)
			{
				alpha = j_alpha.asBool();
			}

			// Load
			TexLoader::LoadTextureSRGB(fileName, dir, alpha);

			iter++;
		}
	}

	void HandleDefault(const Json::Value& _Root)
	{
		Json::ValueConstIterator iter = _Root.begin();
		while (iter != _Root.end())
		{
			Json::Value subroot = _Root[iter.index()];
			auto& j_fileName = subroot["FileName"];
			auto& j_dir = subroot["Dir"];

			std::string fileName = "";
			std::string dir = "";

			if (j_fileName)
			{
				fileName = j_fileName.asString();
			}

			if (j_dir)
			{
				dir = j_dir.asString();
			}

			// Load
			TexLoader::LoadTexture(fileName, dir);

			iter++;
		}
	}

	void TextureResource::InitResource()
	{
		const std::string dir = "Resources/Textures/TextureList.json";

		// Read json file
		std::ifstream istream(dir, std::ios::in);
		if (istream.is_open())
		{
			Json::Value root;
			istream >> root[0];
			Json::ValueIterator iter = root.begin();
			/// Textures
			while (iter != root.end())
			{
				Json::Value texRoot = root[iter.index()];
				auto& cubemapTex = texRoot["CUBEMAP"];
				if (cubemapTex)
				{
					HandleCubemaps(cubemapTex);
				}

				auto& gammaTex = texRoot["GAMMA"];
				if (gammaTex)
				{
					HandleSRGB(gammaTex);
				}

				auto& defaultTex = texRoot["DEFAULT"];
				if (defaultTex)
				{
					HandleDefault(defaultTex);
				}
				iter++;
			}
		}

		//// Compact to index 0
		//for (auto& it : m_TexturePool)
		//{
		//	m_UsedTextures[it.second] = static_cast<int>(m_UsedTextures.size());
		//}

		for (auto it : m_TexturePool)
		{
			printf("%s\n", it.first.c_str());
		}
	}

	void TextureResource::ReloadTextures()
	{
		TexLoader::ClearTextures();
		InitResource();
	}

	//void TextureResource::UseTexture(std::string _TexName)
	//{
	//    // Dont overwrite
	//    if (m_UsedTextures.find(_TexName) == m_UsedTextures.end())
	//    {
	//        m_UsedTextures[_TexName] = static_cast<int>(m_UsedTextures.size());
	//    }
	//}

	//int TextureResource::GetUsedTextureID(std::string _TexName)
	//{
	//    if (m_UsedTextures.find(_TexName) != m_UsedTextures.end())
	//    {
	//        return m_UsedTextures[_TexName];
	//    }

	//    Logger::Log(LogEntry("TEXLOADER", LogEntry::LOGTYPE::ERROR, "This texture is unused"));
	//    return -1;
	//}
}