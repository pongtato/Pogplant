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
                "px.dds",
                "nx.dds",
                "py.dds",
                "ny.dds",
                "pz.dds",
                "nz.dds"
            }
            , "Resources/Textures/Skybox"
        );
	}
}