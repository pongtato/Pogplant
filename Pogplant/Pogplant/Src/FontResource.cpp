#include "FontResource.h"
#include "Font.h"
#include "Logger.h"
#include "TextureLoader.h"

#include <json.h>
#include <glew.h>
#include <fstream>
#include <sstream>
#include <iostream>

namespace Pogplant
{
	std::unordered_map <std::string, Font*> FontResource::m_FontPool;
	std::unordered_map <std::string, std::vector<Dialogue>> FontResource::m_Subtitles;
	
	void FontResource::InitResource()
	{
		const std::string dir = "Resources/Fonts/";
		LoadFont(dir + "Ruda.json");
		LoadFont(dir + "Comfortaa.json");
		LoadFont(dir + "Dekko.json");
		LoadFont(dir + "Bangers.json");

		LoadSubtitles("Resources/Subtitles/Dialogue_lines.json");
	}

	void FontResource::CleanUpResource()
	{
		for (auto& it : m_FontPool)
		{
			delete it.second;
		}
	}

	void HandleBaseInfo(Font* _Font, const Json::Value& _Root)
	{
		auto& pages = _Root["pages"];
		if (pages)
		{
			_Font->m_Texture = pages[0].asString();
		}

		auto& distanceField = _Root["distanceField"];
		if (distanceField)
		{
			// distanceField[0] = "msdf"
			_Font->m_DistanceRange = distanceField["distanceRange"].asInt();
		}

		auto& common = _Root["common"];
		if (common)
		{
			// distanceField[0] = "msdf"
			_Font->m_ImageW = common["scaleW"].asFloat();
			_Font->m_ImageH = common["scaleH"].asFloat();
		}
	}

	void HandleChar(Font* _Font, const Json::Value& _Root)
	{
		auto& chars = _Root["chars"];
		if (chars)
		{
			Json::ValueConstIterator iter = chars.begin();
			while (iter != chars.end())
			{
				Json::Value subroot = chars[iter.index()];
				auto& id = subroot["id"];
				auto& index = subroot["index"];
				auto& chr = subroot["char"];
				auto& width = subroot["width"];
				auto& height = subroot["height"];
				auto& xoffset = subroot["xoffset"];
				auto& yoffset = subroot["yoffset"];
				auto& xadvance = subroot["xadvance"];
				auto& chnl = subroot["chnl"];
				auto& x = subroot["x"];
				auto& y = subroot["y"];
				bool failFlag = false;

				// Try to pass into a char
				Character c;
				if (id) { c.m_ID = id.asInt(); } else { failFlag = true; }
				if (index) { c.m_Index = index.asInt(); } else { failFlag = true; }
				if (chr) 
				{ 
					const std::string output = chr.asString();
					c.m_Char = output[0];
				} 
				else { failFlag = true; }
				if (width) { c.m_Size.x = width.asInt() / _Font->m_ImageW; } else { failFlag = true; }
				if (height) { c.m_Size.y = height.asInt() / _Font->m_ImageH; } else { failFlag = true; }
				if (xoffset) { c.m_Offsets.x = xoffset.asInt() / _Font->m_ImageW; } else { failFlag = true; }
				if (yoffset) { c.m_Offsets.y = 1.0f - yoffset.asInt() / _Font->m_ImageH; } else { failFlag = true; }
				if (xadvance) { c.m_Advance = xadvance.asInt() / _Font->m_ImageW; } else { failFlag = true; }
				if (chnl) { c.m_Chnl = chnl.asInt(); } else { failFlag = true; }
				if (x) { c.m_TexCoords.x = x.asInt() / _Font->m_ImageW; } else { failFlag = true; }
				if (y) { c.m_TexCoords.y = 1.0f - y.asInt() / _Font->m_ImageH; } else { failFlag = true; }

				// Assert
				if (failFlag)
				{
					Logger::Log({ "PP::FONT", LogEntry::LOGTYPE::ERROR, "Fail to load Character" });
				}

				// Update map
				_Font->m_Font[c.m_Char] = std::move(c);

				++iter;
			}
		}
	}

	void FontResource::LoadFont(std::string _FilePath)
	{
		// Get the font name
		auto splitPos = _FilePath.find_first_of(".");
		if (_FilePath.empty() || splitPos >= _FilePath.size())
		{
			Logger::Log({ "PP::FONT", LogEntry::LOGTYPE::ERROR, "Fail to load font, bad ID or Path" });
			return;
		}
		// Get font ID to save 
		auto startSplitPos = _FilePath.find_last_of("/") ;
		auto startPos = startSplitPos >= _FilePath.size() ? 0 : startSplitPos + 1;
		const std::string id = _FilePath.substr(startPos, splitPos - startPos);

		Font* newFont = new Font();

		// Read json file
		std::ifstream istream(_FilePath, std::ios::in);
		if (istream.is_open())
		{
			Json::Value root;
			istream >> root[0];

			Json::ValueIterator iter = root.begin();

			/// Get dimensions
			while (iter != root.end())
			{
				Json::Value subroot = root[iter.index()];
				// Update distance field and texture to use
				HandleBaseInfo(newFont, subroot);

				// Exit once data has been found
				if (newFont->m_ImageW == 0 || newFont->m_ImageH == 0)
				{
					++iter;
				}
				else
				{
					break;
				}
			}

			/// Update chars
			iter = root.begin();
			while (iter != root.end())
			{
				Json::Value subroot = root[iter.index()];
				if (newFont->m_ImageW == 0 || newFont->m_ImageH == 0)
				{
					Logger::Log({ "PP::FONT", LogEntry::LOGTYPE::ERROR, "Bad atlas size" });
					break;
				}
				// Update the bearings of each char
				HandleChar(newFont, subroot);
				++iter;
			}

			istream.close();
		}

		// Prepare textures
		newFont->m_TextureID = TexLoader::LoadUncompressedTexture(newFont->m_Texture, "Resources/Fonts/");

		// Update pool
		m_FontPool[id] = newFont;
	}

	void FontResource::LoadSubtitles(std::string _FilePath)
	{
		auto splitPos = _FilePath.find_first_of(".");
		if (_FilePath.empty() || splitPos >= _FilePath.size())
		{
			Logger::Log({ "PP::FONT", LogEntry::LOGTYPE::ERROR, "Fail to load subtitles, bad Path" });
			return;
		}

		// Read json file
		std::ifstream istream(_FilePath, std::ios::in);
		if (istream.is_open())
		{
			Json::Value root;
			istream >> root[0];
			Json::ValueConstIterator iter = root.begin();

			/// Base root
			if (iter != root.end())
			{
				Json::Value subroot = root[iter.index()];
				const auto& subtitles = subroot["Subtitles"];
				if (subtitles)
				{
					/// Level iterators
					Json::ValueConstIterator sub_iter = subtitles.begin();
					while (sub_iter != subtitles.end())
					{
						/// Get the ID for this level to be stored
						Json::Value subtitlesSubRoot = subtitles[sub_iter.index()];
						const auto& level = subtitlesSubRoot["Level"];
						if (level)
						{
							//std::cout << level.asString() << std::endl;
							auto& currLevel = m_Subtitles[level.asString()];

							/// Get the lines for this level
							const auto& lines = subtitlesSubRoot["Lines"];
							if (lines)
							{
								Json::ValueConstIterator line_iter = lines.begin();
								while (line_iter != lines.end())
								{
									Json::Value lineSubRoot = lines[line_iter.index()];
									const auto& text = lineSubRoot["Text"];
									const auto& timer = lineSubRoot["Timer"];
									if (text && timer)
									{
										//std::cout << text.asString() << std::endl;
										//std::cout << timer.asFloat() << std::endl;
										currLevel.push_back({ text.asString() ,timer.asFloat() });
									}
									line_iter++;
								}
							}
						}
						++sub_iter;
					}
				}
			}
			istream.close();
		}

		/// Debug print
		//for (const auto& it : m_Subtitles)
		//{
		//	std::cout << it.first << std::endl;

		//	for (const auto& it2 : it.second)
		//	{
		//		std::cout << it2.m_Text << std::endl;
		//		std::cout << it2.m_Delay << std::endl;
		//	}
		//}
	}


}

