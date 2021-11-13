#include "ContentBrowser.h"

#include <IconsMaterialDesign.h>
#include <imgui.h>
#include "Application.h"
#include "Serialiser/Serializer.h"

namespace PogplantDriver
{
	//Use current path to test
	Directory::Directory() : 
		pad(16.f), thumbsize(64.f), cellsize( pad + thumbsize),
		panelsize(0), col_count(0),
		m_AssetPath{ "Resources"},
		m_CurrentPath{"Resources"}

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
		else if (ImGui::IsItemHovered() && ImGui::IsMouseClicked(ImGuiMouseButton_Right))
		{
			if (entry.path().extension() == ".prefab")
				current_file = path;
		}
		//else if (!ImGui::IsItemHovered() && ImGui::IsMouseClicked(ImGuiMouseButton_Right))
		//{
		//	current_file = "";
		//}

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
			if (ImGui::IsMouseDown(0) && ImGui::IsWindowHovered())
				current_file = "";

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

		//detect right click on icon
		if (current_file != "")
		{
			if (ImGui::BeginPopupContextWindow("DirectoryPopUp", ImGuiPopupFlags_MouseButtonRight))
			{
				if (ImGui::MenuItem("Add to scene"))
				{
					auto m_ecs = Application::GetInstance().m_activeECS;
					if (!m_ecs->m_prefab_map.contains(current_file))
					{
						Serializer serialiser{ *Application::GetInstance().m_activeECS };
						serialiser.LoadPrefab(current_file, true);
					}

					assert(m_ecs->m_prefab_map.contains(current_file));

					auto _entity = m_ecs->CopyEntity(m_ecs->m_prefab_map[current_file]);
					auto _prefab = m_ecs->GetReg().get<Components::Guid>(m_ecs->m_prefab_map[current_file]);
					m_ecs->GetReg().emplace<Components::PrefabInstance>(_entity, _prefab.m_guid);
				}

				if (ImGui::MenuItem("Edit prefab"))
				{
					Application::GetInstance().StartPrefabEditing(current_file);
				}
				ImGui::EndPopup();
			}
		}
		

		ImGui::End();
	}


}