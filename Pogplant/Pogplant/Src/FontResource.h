#pragma once
#include <unordered_map>

namespace Pogplant
{
	struct Font;

	struct Dialogue
	{
		std::string m_Text;
		float m_Delay;
	};

	struct FontResource
	{
		static void InitResource();
		static void CleanUpResource();

		static std::unordered_map<std::string, Font*> m_FontPool;
		static std::unordered_map <std::string, std::vector<Dialogue>> m_Subtitles;
	private:
		static void LoadFont(std::string _FilePath);
		static void LoadSubtitles(std::string _FilePath);
	};
}