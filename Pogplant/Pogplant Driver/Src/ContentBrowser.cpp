#include "ContentBrowser.h"
#include <imgui.h>

namespace PogplantDriver
{
	//Use current path to test
	Directory::Directory() : m_AssetPath{"..//Pogplant Driver//Resources"} , m_CurrentPath{"..//Pogplant Driver//Resources"}
	{
		
	}

	void Directory::RenderDirectory()
	{
		ImGui::Begin("Directory");

		if (m_CurrentPath != std::filesystem::path(m_AssetPath))
		{
			if (ImGui::Button("<<"))
			{
				m_CurrentPath = m_CurrentPath.parent_path();
			}
		}

		for (auto& entry : std::filesystem::directory_iterator(m_CurrentPath))
		{
				std::string path = entry.path().string();
				auto relativePath = std::filesystem::relative(entry.path(), m_AssetPath);
				std::string filenameString = relativePath.filename().string();

				if(entry.is_directory())
				{
					if (ImGui::Button(filenameString.c_str()))
					{
						m_CurrentPath /= entry.path().filename();
					}
				}
				else
				{
					if (ImGui::MenuItem(filenameString.c_str()))
					{
					}
				}
		}

		ImGui::End();
	}


}