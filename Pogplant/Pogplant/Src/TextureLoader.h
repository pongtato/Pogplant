#pragma once

#include <string>
#include <vector>

namespace Pogplant
{
	struct TexLoader
	{
		static void SetTextureFlip(bool _Flip);
		static unsigned int LoadTexture(std::string _Path, std::string _Directory);
		static unsigned int LoadUncompressedTexture(std::string _Path, std::string _Directory, bool _Alpha = true);
		static unsigned int LoadCubemap(std::vector<std::string> _Paths, std::string _Directory);
	};

}