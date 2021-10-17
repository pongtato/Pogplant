#include "ContentBrowser.h"

#include <IconsMaterialDesign.h>
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

		float pad = 16.f;
		float thumbsize = 64.f;
		float cellsize = pad + thumbsize;
		float panelsize = ImGui::GetContentRegionAvail().x;
		int col_count = (int)(panelsize / cellsize);
		if (col_count < 1)
			col_count = 1;

		if (ImGui::BeginTable("##DirectoryTable", col_count))
		{
			for (auto& entry : std::filesystem::directory_iterator(m_CurrentPath))
			{

				std::string path = entry.path().string();
				auto relativePath = std::filesystem::relative(entry.path(), m_AssetPath);
				std::string filenameString = relativePath.filename().string();

				ImGui::TableNextColumn();
				ImGui::PushStyleVar(ImGuiStyleVar_ButtonTextAlign, { 0.f ,1.f });
				ImGui::PushStyleColor(ImGuiCol_Button, ImVec4(0, 0, 0, 0));
				ImGui::PushID(filenameString.c_str());
				if (entry.is_directory())
				{
					ImGui::Button(ICON_MD_FOLDER, { thumbsize,thumbsize });
				}
				else
				{
					ImGui::Button(ICON_MD_DESCRIPTION, { thumbsize,thumbsize });
				}
				ImGui::PopID();
				ImGui::PopStyleVar();
				ImGui::PopStyleColor();
	

				if (ImGui::IsItemHovered() && ImGui::IsMouseDoubleClicked(ImGuiMouseButton_Left))
				{
					if (entry.is_directory())
						m_CurrentPath /= entry.path().filename();
				}
				ImGui::TextWrapped(filenameString.c_str());
			}
			ImGui::EndTable();
		}
	

	if (ImGui::Begin("Prefab"))
	{
	}
	ImGui::End();

	ImGui::End();
	}


}