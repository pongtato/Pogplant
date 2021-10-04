#include "TextureResource.h"
#include "TextureLoader.h"

namespace Pogplant
{
	std::unordered_map<std::string, unsigned int> TextureResource::m_TexturePool;

	void TextureResource::InitResource()
	{
		m_TexturePool["SKYBOX"] = TexLoader::LoadCubemap
		(
			{
				"right.dds",
				"left.dds",
				"top.dds",
				"bottom.dds",
				"front.dds",
				"back.dds"
			}
			, "Resources/Textures/Skybox"
		);
	}
}