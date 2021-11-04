#include "TextureResource.h"
#include "TextureLoader.h"
#include "Logger.h"

namespace Pogplant
{
	std::unordered_map<std::string, unsigned int> TextureResource::m_TexturePool;
    //std::unordered_map<std::string, int> TextureResource::m_UsedTextures;

	void TextureResource::InitResource()
	{
        TexLoader::LoadCubemap
        (
            {
                "px.dds",
                "nx.dds",
                "py.dds",
                "ny.dds",
                "pz.dds",
                "nz.dds"
            }
            , "Resources\\Textures\\Skybox"
        );

        // Must group all SRGB together

        TexLoader::LoadTextureSRGB
        (
            "ParticleTest.dds",
            "Resources/Textures/Particle"
        );

        TexLoader::LoadTextureSRGB
        (
            "grass_diff.dds",
            "Resources/Textures/Grass"
        );

        TexLoader::LoadTextureSRGB
        (
            "mud_diff.dds",
            "Resources/Textures/Mud"
        );

        // 

        TexLoader::LoadTexture
        (
            "mud_bump.dds",
            "Resources/Textures/Mud"
        );

        TexLoader::LoadTexture
        (
            "mud_norm.dds",
            "Resources/Textures/Mud"
        );

        TexLoader::LoadTexture
        (
            "mud_rough.dds",
            "Resources/Textures/Mud"
        );

		TexLoader::LoadTexture
		(
			"grass_bump.dds",
			"Resources/Textures/Grass"
		);

		TexLoader::LoadTexture
		(
			"grass_norm.dds",
			"Resources/Textures/Grass"
		);

		TexLoader::LoadTexture
		(
			"grass_rough.dds",
			"Resources/Textures/Grass"
		);

        TexLoader::LoadTexture
        (
            "rocks_diff.dds",
            "Resources/Textures/Rocks"
        );

        TexLoader::LoadTexture
        (
            "snow_diff.dds",
            "Resources/Textures/Snow"
        );
	}

    //void TextureResource::UseTexture(std::string _TexName)
    //{
    //    // Dont overwrite
    //    if (m_UsedTextures.find(_TexName) == m_UsedTextures.end())
    //    {
    //        m_UsedTextures[_TexName] = static_cast<int>(m_UsedTextures.size());
    //    }
    //}

    //int TextureResource::GetUsedTextureID(std::string _TexName)
    //{
    //    if (m_UsedTextures.find(_TexName) != m_UsedTextures.end())
    //    {
    //        return m_UsedTextures[_TexName];
    //    }

    //    Logger::Log(LogEntry("TEXLOADER", LogEntry::LOGTYPE::ERROR, "This texture is unused"));
    //    return -1;
    //}
}