#ifndef _TEXTURES_H_
#define _TEXTURES_H_

#include <string>

struct Textures
{
	static void SetTextureFlip(bool _Flip);
	static unsigned int LoadTexture(std::string _Path, std::string _Directory);
};

#endif //_TEXTURES_H_