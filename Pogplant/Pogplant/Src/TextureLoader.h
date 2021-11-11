#pragma once

#include <string>
#include <vector>

namespace Pogplant
{
	struct TexLoader
	{
		static void SetTextureFlip(bool _Flip);
		static unsigned int LoadTexture(std::string _Path, std::string _Directory);
		static unsigned int LoadTextureSRGB(std::string _Path, std::string _Directory, bool _Alpha = true);
		static unsigned int LoadUncompressedTexture(std::string _Path, std::string _Directory, bool _Alpha = true);
		static unsigned int LoadCubemap(std::vector<std::string> _Paths, std::string _Directory);
		static bool LoadHeightMap(std::string _Path, std::string _Directory, std::vector<unsigned char>& _HeightMap, size_t& _Dim);
		static float GetHeight(int _X, int _Z, size_t _Dim, const std::vector<unsigned char>& _HeightMap);
		static void ClearTextures();
	};

}