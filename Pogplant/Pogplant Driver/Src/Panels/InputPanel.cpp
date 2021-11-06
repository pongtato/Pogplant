#include "InputPanel.h"
#include <imgui.h>
#include <IconsFontAwesome5.h>
#include <IconsKenney.h>

#include <sstream>
#include <string>
#include "../Input/InputSystem.h"
#include "../Input/GLFWInput.h"

namespace PPP
{
	void InputPanel::Init(ECS* ecs, entt::entity& current_entity)
	{
		m_ECS = ecs;
		m_CurrentEntity = current_entity;
	}

	void InputPanel::Render(entt::entity& current_entity)
	{
		(void)current_entity;

		ImGui::Begin("Input Editor");
		{
			if (ImGui::CollapsingHeader(ICON_FA_COG"  Key Mappings", ImGuiTreeNodeFlags_DefaultOpen))
			{
				auto& inputMap = PPI::InputSystem::GetInputMap();

				for (auto itr = inputMap.begin(); itr != inputMap.end(); ++itr)
				{
					if (ImGui::TreeNode(itr->first.c_str()))
					{
						int keyID = itr->second.keyboard;

						ImGui::Text(ICON_FA_KEYBOARD " Key");
						if (ImGui::BeginCombo("###StandardKeys", GetKeyString(keyID).c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (int i = -1; i < GLFW_KEY_LAST; ++i)
							{
								if (!IsValidKey(i))
									continue;

								const bool isSelected = (i == keyID);

								if (ImGui::Selectable(GetKeyString(i).c_str(), isSelected))
								{
									itr->second.keyboard = i;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}

						//Show button keypress
						if (itr->second.keyboard != -1 && PPI::GLFWInputManager::onKeyHeld(itr->second.keyboard))
						{
							ImGui::SameLine();
							ImGui::Text(ICON_FA_CHECK_CIRCLE);
						}

						int controllerKeyID = itr->second.controller;
						ImGui::Text(ICON_FA_GAMEPAD " Gamepad");
						if (ImGui::BeginCombo("###ControllerKeys", GetControllerString(controllerKeyID).c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (int i = -1; i < GLFW_GAMEPAD_BUTTON_LAST + GLFW_GAMEPAD_AXIS_LAST + 2; ++i)
							{
								if (!IsValidControllerKey(i))
									continue;

								const bool isSelected = (i == controllerKeyID);

								if (ImGui::Selectable(GetControllerString(i).c_str(), isSelected))
								{
									itr->second.controller = i;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}

						//Show button keypress
						if (itr->second.controller != -1 && PPI::GLFWInputManager::onControllerHeld(itr->second.controller))
						{
							ImGui::SameLine();
							ImGui::Text(ICON_FA_CHECK_CIRCLE);
						}

						//Remove key from map
						if (ImGui::Button(ICON_FA_MINUS_CIRCLE " Remove"))
						{
							inputMap.erase(itr);
							ImGui::TreePop();
							break;
						}

						ImGui::TreePop();
					}
				}

				//Add new key
				static char name_stuff[256] = "";
				ImGui::InputText("New Key Name", name_stuff, IM_ARRAYSIZE(name_stuff));

				if (ImGui::Button(ICON_FA_PLUS_CIRCLE " Add key"))
				{
					inputMap.insert({ std::string(name_stuff), PPI::InputSystem::keyCode{-1, -1} });
				}
			}

			//------------------------------------------------------------------------------------------------------------
			if (ImGui::CollapsingHeader(ICON_FA_COG"  Axis Mappings", ImGuiTreeNodeFlags_DefaultOpen))
			{
				auto& axisMap = PPI::InputSystem::GetAxisMap();

				for (auto itr = axisMap.begin(); itr != axisMap.end(); ++itr)
				{
					if (ImGui::TreeNode(itr->first.c_str()))
					{
						//Controller selection
						int axisKeyID = itr->second.axisID;
						ImGui::Text(ICON_FA_GAMEPAD " Gamepad");
						if (ImGui::BeginCombo("###AxisKeys", GetAxisString(axisKeyID).c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (int i = -1; i < GLFW_GAMEPAD_AXIS_LAST + 1; ++i)
							{
								if (!IsValidAxisKey(i))
									continue;

								const bool isSelected = (i == axisKeyID);

								if (ImGui::Selectable(GetAxisString(i).c_str(), isSelected))
								{
									itr->second.axisID = i;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}

						ImGui::Checkbox("Use Bounded Deadzone", &itr->second.UseBoundedDeadzone);

						if (itr->second.UseBoundedDeadzone)
							itr->second.UseMiddleDeadzone = false;


						ImGui::Checkbox("Use Middle Deadzone", &itr->second.UseMiddleDeadzone);

						if (itr->second.UseMiddleDeadzone)
							itr->second.UseBoundedDeadzone = false;

						if (itr->second.UseBoundedDeadzone)
						{
							ImGui::SliderFloat("Lower deadzone", &itr->second.lowerDeadzone, 0.001f, 1.f);
							ImGui::SliderFloat("Upper deadzone", &itr->second.upperDeadzone, 0.001f, 1.f);
						}
						if (itr->second.UseMiddleDeadzone)
						{
							ImGui::SliderFloat("Inner deadzone", &itr->second.lowerDeadzone, 0.001f, 1.f);
						}

						//Show axis values
						if (itr->second.axisID != -1)
						{
							std::stringstream ss;
							float axisValue = PPI::InputSystem::GetAxis(itr->first);
							float rawValue = PPI::GLFWInputManager::GetControllerAxis(itr->second.axisID);
							ss << std::setprecision(2) << "Raw: " << rawValue;
							ImGui::Text(ss.str().c_str());

							ImGui::ProgressBar((rawValue + 1.f) * 0.5f, ImVec2(-1.0f, 1.0f));

							ss.str("");
							ss.clear();
							ss << "Deadzoned: " << axisValue;
							ImGui::Text(ss.str().c_str());

							ImGui::ProgressBar((axisValue + 1.f) * 0.5f, ImVec2(-1.0f, 1.0f));
						}

						//Remove axis from map
						if (ImGui::Button(ICON_FA_MINUS_CIRCLE " Remove"))
						{
							axisMap.erase(itr);
							ImGui::TreePop();
							break;
						}

						ImGui::TreePop();
					}
				}

				//Add new axis
				static char name_stuff[256] = "";
				ImGui::InputText("New Axis Name", name_stuff, IM_ARRAYSIZE(name_stuff));

				if (ImGui::Button(ICON_FA_PLUS_CIRCLE " Add axis"))
				{
					axisMap.insert({ std::string(name_stuff), PPI::InputSystem::ControllerAxis{-1} });
				}
			}
		}
		ImGui::End();
	}

	std::string InputPanel::GetKeyString(int glfwKeyCode)
	{
		switch (glfwKeyCode)
		{
#define STR(r) case GLFW_KEY_ ##r: return #r
			STR(SPACE);
			STR(APOSTROPHE);
			STR(COMMA);
			STR(MINUS);
			STR(PERIOD);
			STR(SLASH);
			STR(0);
			STR(1);
			STR(2);
			STR(3);
			STR(4);
			STR(5);
			STR(6);
			STR(7);
			STR(8);
			STR(9);
			STR(SEMICOLON);
			STR(EQUAL);
			STR(A);
			STR(B);
			STR(C);
			STR(D);
			STR(E);
			STR(F);
			STR(G);
			STR(H);
			STR(I);
			STR(J);
			STR(K);
			STR(L);
			STR(M);
			STR(N);
			STR(O);
			STR(P);
			STR(Q);
			STR(R);
			STR(S);
			STR(T);
			STR(U);
			STR(V);
			STR(W);
			STR(X);
			STR(Y);
			STR(Z);
			STR(LEFT_BRACKET);
			STR(BACKSLASH);
			STR(RIGHT_BRACKET);
			STR(GRAVE_ACCENT);
			STR(WORLD_1);
			STR(WORLD_2);
			STR(ESCAPE);
			STR(ENTER);
			STR(TAB);
			STR(BACKSPACE);
			STR(INSERT);
			STR(DELETE);
			STR(RIGHT);
			STR(LEFT);
			STR(DOWN);
			STR(UP);
			STR(PAGE_UP);
			STR(PAGE_DOWN);
			STR(HOME);
			STR(END);
			STR(CAPS_LOCK);
			STR(SCROLL_LOCK);
			STR(NUM_LOCK);
			STR(PRINT_SCREEN);
			STR(PAUSE);
			STR(F1);
			STR(F2);
			STR(F3);
			STR(F4);
			STR(F5);
			STR(F6);
			STR(F7);
			STR(F8);
			STR(F9);
			STR(F10);
			STR(F11);
			STR(F12);
			STR(F13);
			STR(F14);
			STR(F15);
			STR(F16);
			STR(F17);
			STR(F18);
			STR(F19);
			STR(F20);
			STR(F21);
			STR(F22);
			STR(F23);
			STR(F24);
			STR(F25);
			STR(KP_0);
			STR(KP_1);
			STR(KP_2);
			STR(KP_3);
			STR(KP_4);
			STR(KP_5);
			STR(KP_6);
			STR(KP_7);
			STR(KP_8);
			STR(KP_9);
			STR(KP_DECIMAL);
			STR(KP_DIVIDE);
			STR(KP_MULTIPLY);
			STR(KP_SUBTRACT);
			STR(KP_ADD);
			STR(KP_ENTER);
			STR(KP_EQUAL);
			STR(LEFT_SHIFT);
			STR(LEFT_CONTROL);
			STR(LEFT_ALT);
			STR(LEFT_SUPER);
			STR(RIGHT_SHIFT);
			STR(RIGHT_CONTROL);
			STR(RIGHT_ALT);
			STR(RIGHT_SUPER);
			STR(MENU);
#undef STR
#define STR(r) case GLFW_ ##r: return #r
			STR(MOUSE_BUTTON_1);
			STR(MOUSE_BUTTON_2);
			STR(MOUSE_BUTTON_3);
			STR(MOUSE_BUTTON_4);
			STR(MOUSE_BUTTON_5);
			STR(MOUSE_BUTTON_6);
			STR(MOUSE_BUTTON_7);
			STR(MOUSE_BUTTON_8);
#undef STR
		case -1:
			return "None";
		default:
			return "UNKNOWN_KEY";
		}
	}

	bool InputPanel::IsValidKey(int glfwKeyCode)
	{
		return (GetKeyString(glfwKeyCode) != "UNKNOWN_KEY");
	}

	std::string InputPanel::GetControllerString(int glfwControllerCode)
	{
		switch (glfwControllerCode)
		{
#define STR(r) case GLFW_GAMEPAD_ ##r: return #r
			STR(BUTTON_A);
			STR(BUTTON_B);
			STR(BUTTON_X);
			STR(BUTTON_Y);
			STR(BUTTON_LEFT_BUMPER);
			STR(BUTTON_RIGHT_BUMPER);
			STR(BUTTON_BACK);
			STR(BUTTON_START);
			STR(BUTTON_GUIDE);
			STR(BUTTON_LEFT_THUMB);
			STR(BUTTON_RIGHT_THUMB);
			STR(BUTTON_DPAD_UP);
			STR(BUTTON_DPAD_RIGHT);
			STR(BUTTON_DPAD_DOWN);
			STR(BUTTON_DPAD_LEFT);
#undef STR

		case GLFW_GAMEPAD_AXIS_LEFT_X + GLFW_GAMEPAD_BUTTON_LAST + 1:
			return "GAMEPAD_AXIS_LEFT_X";
		case GLFW_GAMEPAD_AXIS_LEFT_Y + GLFW_GAMEPAD_BUTTON_LAST + 1:
			return "GAMEPAD_AXIS_LEFT_Y";
		case GLFW_GAMEPAD_AXIS_RIGHT_X + GLFW_GAMEPAD_BUTTON_LAST + 1:
			return "GAMEPAD_AXIS_RIGHT_X";
		case GLFW_GAMEPAD_AXIS_RIGHT_Y + GLFW_GAMEPAD_BUTTON_LAST + 1:
			return "GAMEPAD_AXIS_RIGHT_Y";
		case GLFW_GAMEPAD_AXIS_LEFT_TRIGGER + GLFW_GAMEPAD_BUTTON_LAST + 1:
			return "GAMEPAD_AXIS_LEFT_TRIGGER";
		case GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER + GLFW_GAMEPAD_BUTTON_LAST + 1:
			return "GAMEPAD_AXIS_RIGHT_TRIGGER";
		case -1:
			return "None";
		default:
			return "UNKNOWN_KEY";
		}
	}

	bool InputPanel::IsValidControllerKey(int glfwKeyCode)
	{
		return (GetControllerString(glfwKeyCode) != "UNKNOWN_KEY");
	}

	std::string InputPanel::GetAxisString(int glfwControllerCode)
	{
		switch (glfwControllerCode)
		{
#define STR(r) case GLFW_GAMEPAD_ ##r: return #r
			STR(AXIS_LEFT_X);
			STR(AXIS_LEFT_Y);
			STR(AXIS_RIGHT_X);
			STR(AXIS_RIGHT_Y);
			STR(AXIS_LEFT_TRIGGER);
			STR(AXIS_RIGHT_TRIGGER);
#undef STR
		case -1:
			return "None";
		default:
			return "UNKNOWN_KEY";
		}
	}

	bool InputPanel::IsValidAxisKey(int glfwKeyCode)
	{
		return (GetAxisString(glfwKeyCode) != "UNKNOWN_KEY");
	}
}