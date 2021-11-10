#pragma once

#include <string>
#include <unordered_map>

namespace Pogplant
{
	struct TextureResource
	{
		static void InitResource();
		//static void UseTexture(std::string _TexName);
		//static int GetUsedTextureID(std::string _TexName);
		static std::unordered_map<std::string, unsigned int> m_TexturePool;
		static std::unordered_map<unsigned int, int> m_UsedTextures;
	};
}