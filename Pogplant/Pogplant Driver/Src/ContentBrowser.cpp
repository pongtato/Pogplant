#include "ContentBrowser.h"

#include <IconsMaterialDesign.h>
#include <imgui.h>

namespace PogplantDriver
{
	//Use current path to test
	Directory::Directory() : 
		pad(16.f), thumbsize(64.f), cellsize( pad + thumbsize),
		panelsize(0), col_count(0),
		m_AssetPath{"..//Pogplant Driver//Resources"},
		m_CurrentPath{"..//Pogplant Driver//Resources"}

	{
	}

	void Directory::DrawIndividual(const std::filesystem::directory_entry& entry)
	{

		std::string path = entry.path().string();
		auto relativePath = std::filesystem::relative(entry.path(), m_AssetPath);
		std::string filenameString = relativePath.filename().string();

		ImGui::TableNextColumn();
		ImGui::PushStyleVar(ImGuiStyleVar_ButtonTextAlign, { 0.f ,1.f });
		ImGui::PushStyleColor(ImGuiCol_Button, ImVec4(0, 0, 0, 0));
		ImGui::PushID(filenameString.c_str());

		ImGui::Button(entry.is_directory() ? ICON_MD_FOLDER : ICON_MD_DESCRIPTION, { thumbsize,thumbsize });
	
		ImGui::PopID();
		ImGui::PopStyleColor();
		ImGui::PopStyleVar();


		if (ImGui::IsItemHovered() && ImGui::IsMouseDoubleClicked(ImGuiMouseButton_Left))
		{
			if (entry.is_directory())
				m_CurrentPath /= entry.path().filename();
		}
		ImGui::TextWrapped(filenameString.c_str());
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

		panelsize = ImGui::GetContentRegionAvail().x;
		col_count = (int)(panelsize / cellsize);

		if (col_count < 1)
			col_count = 1;

		if (ImGui::BeginTable("##DirectoryTable", col_count))
		{
			std::vector<std::filesystem::directory_entry> asd;
			for (const auto& entry : std::filesystem::directory_iterator(m_CurrentPath))
			{
				if (!entry.is_directory())
				{
					asd.push_back(entry);
					continue;
				}
				DrawIndividual(entry);
			}

			for (auto& entry : asd)
				DrawIndividual(entry);

			ImGui::EndTable();
		}
		ImGui::End();
	}


}