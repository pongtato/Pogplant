#pragma once

#include <string>

namespace Pogplant
{
	struct TexLoader
	{
		static void SetTextureFlip(bool _Flip);
		static unsigned int LoadTexture(std::string _Path, std::string _Directory);
	};
}