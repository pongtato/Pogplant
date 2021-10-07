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

        m_TexturePool["MUD_DIFF"] = TexLoader::LoadTexture
        (
            "mud_diff.dds",
            "Resources/Textures/Mud"
        );

        m_TexturePool["MUD_BUMP"] = TexLoader::LoadTexture
        (
            "mud_bump.dds",
            "Resources/Textures/Mud"
        );

        m_TexturePool["MUD_NORM"] = TexLoader::LoadTexture
        (
            "mud_norm.dds",
            "Resources/Textures/Mud"
        );

        m_TexturePool["MUD_SPEC"] = TexLoader::LoadTexture
        (
            "mud_rough.dds",
            "Resources/Textures/Mud"
        );

		//m_TexturePool["SNOW_DIFF"] = TexLoader::LoadTexture
		//(
		//	"snow_diff.dds",
		//	"Resources/Textures/Snow"
		//);

		//m_TexturePool["SNOW_BUMP"] = TexLoader::LoadTexture
		//(
		//	"snow_bump.dds",
		//	"Resources/Textures/Snow"
		//);

		//m_TexturePool["SNOW_NORM"] = TexLoader::LoadTexture
		//(
		//	"snow_norm.dds",
		//	"Resources/Textures/Snow"
		//);

		//m_TexturePool["SNOW_SPEC"] = TexLoader::LoadTexture
		//(
		//	"snow_rough.dds",
		//	"Resources/Textures/Snow"
		//);

  //      m_TexturePool["ROCKS_DIFF"] = TexLoader::LoadTexture
  //      (
  //          "rocks_diff.dds",
  //          "Resources/Textures/Rocks"
  //      );

  //      m_TexturePool["ROCKS_BUMP"] = TexLoader::LoadTexture
  //      (
  //          "rocks_bump.dds",
  //          "Resources/Textures/Rocks"
  //      );

  //      m_TexturePool["ROCKS_NORM"] = TexLoader::LoadTexture
  //      (
  //          "rocks_norm.dds",
  //          "Resources/Textures/Rocks"
  //      );

  //      m_TexturePool["ROCKS_SPEC"] = TexLoader::LoadTexture
  //      (
  //          "rocks_rough.dds",
  //          "Resources/Textures/Rocks"
  //      );

        //m_TexturePool["FOREST_DIFF"] = TexLoader::LoadTexture
        //(
        //    "forest_diff.dds",
        //    "Resources/Textures/Forest"
        //);

        //m_TexturePool["FOREST_BUMP"] = TexLoader::LoadTexture
        //(
        //    "forest_bump.dds",
        //    "Resources/Textures/Forest"
        //);

        //m_TexturePool["FOREST_NORM"] = TexLoader::LoadTexture
        //(
        //    "forest_norm.dds",
        //    "Resources/Textures/Forest"
        //);

        //m_TexturePool["FOREST_SPEC"] = TexLoader::LoadTexture
        //(
        //    "forest_rough.dds",
        //    "Resources/Textures/Forest"
        //);

		m_TexturePool["GRASS_DIFF"] = TexLoader::LoadTexture
		(
			"grass_diff.dds",
			"Resources/Textures/Grass"
		);

		m_TexturePool["GRASS_BUMP"] = TexLoader::LoadTexture
		(
			"grass_bump.dds",
			"Resources/Textures/Grass"
		);

		m_TexturePool["GRASS_NORM"] = TexLoader::LoadTexture
		(
			"grass_norm.dds",
			"Resources/Textures/Grass"
		);

		m_TexturePool["GRASS_SPEC"] = TexLoader::LoadTexture
		(
			"grass_rough.dds",
			"Resources/Textures/Grass"
		);
	}
}