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
				"right.jpg",
				"left.jpg",
				"top.jpg",
				"bottom.jpg",
				"front.jpg",
				"back.jpg"
			}
			, "Textures/Skybox"
		);

		m_TexturePool["SKYBOX2"] = TexLoader::LoadCubemap
		(
			{
				"px.png",
				"nx.png",
				"py.png",
				"ny.png",
				"pz.png",
				"nz.png"
			}
			, "Textures/Skybox2"
		);

		m_TexturePool["SKYBOX3"] = TexLoader::LoadCubemap
		(
			{
				"px.png",
				"nx.png",
				"py.png",
				"ny.png",
				"pz.png",
				"nz.png"
			}
			, "Textures/Skybox3"
		);

		m_TexturePool["SKYBOX4"] = TexLoader::LoadCubemap
		(
			{
				"px.png",
				"nx.png",
				"py.png",
				"ny.png",
				"pz.png",
				"nz.png"
			}
			, "Textures/Skybox4"
		);

	}
}