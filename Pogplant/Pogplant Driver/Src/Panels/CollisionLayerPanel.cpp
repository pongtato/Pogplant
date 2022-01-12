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
				if (ImGui::BeginTable("##CollisionMatrix", static_cast<int>(physicsSystem.m_collisionLayers.size() + 1), ImGuiTableFlags_Borders))
				{
					ImGui::TableNextRow();
					
					auto rowItr = physicsSystem.m_collisionLayers.begin();

					for (int row = 0; row < physicsSystem.m_collisionLayers.size() + 1; ++row)
					{
						ImGui::TableNextColumn();

						if (row != 0)
							ImGui::Text(rowItr->first.c_str());

						ImGui::PushID(rowItr->second);

						int col = 0;
						for (auto colItr = physicsSystem.m_collisionLayers.rbegin(); colItr != physicsSystem.m_collisionLayers.rend(); ++colItr)
						{
							ImGui::TableNextColumn();
							
							if (row == 0)
							{
								ImGui::Text(colItr->first.c_str());
							}
							else
							{
								bool tempbool = physicsSystem.GetCollisionRule(rowItr->second, colItr->second) == Components::Collider::COLLISION_RULE::CR_COLLIDE;

								if (row + col <= physicsSystem.m_collisionLayers.size())
								{
									ImGui::PushID(colItr->second);
									if (ImGui::Checkbox("", &tempbool))
									{
										if (tempbool)
											physicsSystem.SetCollisionRule(rowItr->second, colItr->second, Components::Collider::COLLISION_RULE::CR_COLLIDE);
										else
											physicsSystem.SetCollisionRule(rowItr->second, colItr->second, Components::Collider::COLLISION_RULE::CR_IGNORE);
									}

									ImGui::PopID();
								}

								++col;
							}
						}

						ImGui::PopID();

						if (row != 0)
							++rowItr;

						ImGui::TableNextRow();
					}

					/*for (size_t row = 0; row < physicsSystem.m_collisionLayers.size(); row++)
					{
						ImGui::TableNextRow();

						for (size_t col = 0; col < physicsSystem.m_collisionLayers.size(); col++)
						{
							ImGui::TableNextColumn();
							ImGui::Text("Row %d, Col %d", row, col);
						}
					}//*/
				}
				ImGui::EndTable();

				if (ImGui::Button(ICON_FA_SAVE "  Save"))
					physicsSystem.SaveLayers();

			}

			if (ImGui::CollapsingHeader(ICON_FA_LAYER_GROUP"  Collision Layers", ImGuiTreeNodeFlags_DefaultOpen))
			{
				for (auto itr = physicsSystem.m_collisionLayers.begin(); itr != physicsSystem.m_collisionLayers.end(); ++itr)
				{
					std::stringstream ss;
					ss << itr->second << ": " << itr->first;
					ImGui::Text(ss.str().c_str());

					if (itr->second != 0)
					{
						ImGui::PushID(itr->second);
						ImGui::SameLine();

						if (ImGui::Button(ICON_FA_MINUS_CIRCLE " Remove"))
						{
							physicsSystem.DestroyCollisionLayer(itr->first);
							ImGui::PopID();
							break;
						}

						ImGui::PopID();
					}
				}

				static char name[256] = "";
				ImGui::InputText("Layer name", name, IM_ARRAYSIZE(name));

				if (ImGui::Button(ICON_FA_PLUS_CIRCLE " Add layer"))
					physicsSystem.CreateCollisionLayer(name);
			}

			static bool drawAABBtree = false;

			ImGui::Separator();

			ImGui::Checkbox(ICON_FA_TREE " Enable AABB Tree Draw", &drawAABBtree);

			ImGui::Text("Number of queries: %d", physicsSystem.GetCollisionQueryCount());

			if (drawAABBtree)
			{
				//Draw dynamic aabbtree
				auto boxes = physicsSystem.GetAABBTreeBoxes();
				for (auto& box : boxes)
				{
					PP::DebugDraw::DebugCube(box.m_min, box.m_max);
				}
			}
		}
		ImGui::End();
	}
}