#pragma once

#include <unordered_map>
#include "Character.h"

namespace Pogplant
{
	struct Font
	{
		std::unordered_map<unsigned char, Character> m_Font;
		std::string m_Texture;
		float m_ImageW; // Put this as float so i dont have to cast it later
		float m_ImageH;
		int m_DistanceRange;
		int m_TextureID;
	};
}