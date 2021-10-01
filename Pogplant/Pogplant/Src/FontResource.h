#pragma once
#include <unordered_map>

namespace Pogplant
{
	struct Font;

	struct FontResource
	{
		static void InitResource();
		static void CleanUpResource();

		static std::unordered_map<std::string, Font*> m_FontPool;
	private:
		static void LoadFont(std::string _FilePath);
	};
}