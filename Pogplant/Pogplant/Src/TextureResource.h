#pragma once

#include <string>
#include <unordered_map>

namespace Pogplant
{
	struct TextureResource
	{
		static void InitResource();
		static std::unordered_map<std::string, unsigned int> m_TexturePool;
	};
}