#pragma once

#include <string>

namespace Pogplant
{
	struct TexLoader
	{
		static unsigned int LoadTexture(std::string _Path, std::string _Directory);
	};
}