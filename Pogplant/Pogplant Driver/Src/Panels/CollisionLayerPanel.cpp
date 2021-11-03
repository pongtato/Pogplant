#include "CollisionLayerPanel.h"
#include <imgui.h>
#include <IconsFontAwesome5.h>
#include <IconsKenney.h>

#include <sstream>
#include <string>

#include "../Application.h"

namespace PPP
{
	void CollisionLayerPanel::Init(ECS* ecs, entt::entity& current_entity)
	{
		m_ECS = ecs;
		m_CurrentEntity = current_entity;
	}

	void CollisionLayerPanel::Render(entt::entity& current_entity)
	{
		(void)current_entity;
		auto& physicsSystem = PogplantDriver::Application::GetInstance().m_sPhysicsSystem;

		ImGui::Begin("Collision Layer Editor");
		{
			if (ImGui::CollapsingHeader(ICON_FA_CAR_CRASH"  Collision Behaviours", ImGuiTreeNodeFlags_DefaultOpen))
			{
				for (auto it = physicsSystem.m_collisionMatrix.begin(); it != physicsSystem.m_collisionMatrix.end(); ++it)
				{
					it->first.first;//1
					it->first.second;//2
					it->second;//Behaviour
				}
			}

			if (ImGui::CollapsingHeader(ICON_FA_LAYER_GROUP"  Collision Layers", ImGuiTreeNodeFlags_DefaultOpen))
			{
				for (size_t i = 0; i < physicsSystem.m_collisionLayers.size(); i++)
				{
					std::stringstream ss;
					ss << i << ": " << physicsSystem.m_collisionLayers[i];
					ImGui::Text(ss.str().c_str());
					
					if (i != 0)
					{
						ImGui::PushID((int)i);
						ImGui::SameLine();

						if (ImGui::Button(ICON_FA_MINUS_CIRCLE " Remove"))
						{
							physicsSystem.m_collisionLayers.erase(physicsSystem.m_collisionLayers.begin() + i);
							ImGui::PopID();
							break;
						}

						ImGui::PopID();
					}
				}

				static char name[256] = "";
				ImGui::InputText("Layer name", name, IM_ARRAYSIZE(name));

				if (ImGui::Button(ICON_FA_PLUS_CIRCLE " Add layer"))
				{
					physicsSystem.m_collisionLayers.push_back(name);
				}
			}
		}
		ImGui::End();
	}
}