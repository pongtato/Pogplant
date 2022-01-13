/******************************************************************************/
/*!
\file	InputSystem.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Input system to handle linking of input detection
	to actual input response execution

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "InputSystem.h"
#include "GLFWInput.h"
#include "WindowsXInput.h"

#include <iostream>

#define JOYSTICKOSET GLFW_GAMEPAD_BUTTON_LAST + 1

namespace PPI
{
	std::unique_ptr<InputSystem> InputSystem::m_instance = nullptr;
	std::once_flag InputSystem::m_onceFlag;

	/******************************************************************************/
	/*!
	\brief
		Initialise the inputsystem controls
	*/
	/******************************************************************************/
	void InputSystem::Init(GLFWwindow* window)
	{
		//All this control initialisation should be loaded and save into a save file eventually

		InputSystem::Instance().AppendKey("LEFT", GLFW_KEY_A, GLFW_GAMEPAD_BUTTON_DPAD_LEFT);
		InputSystem::Instance().AppendKey("RIGHT", GLFW_KEY_D, GLFW_GAMEPAD_BUTTON_DPAD_RIGHT);
		InputSystem::Instance().AppendKey("DOWN", GLFW_KEY_S, GLFW_GAMEPAD_BUTTON_DPAD_UP);
		InputSystem::Instance().AppendKey("UP", GLFW_KEY_W, GLFW_GAMEPAD_BUTTON_DPAD_DOWN);
		InputSystem::Instance().AppendKey("SHOOT", GLFW_KEY_SPACE, GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER + JOYSTICKOSET);



		/*Set menu buttons*/
		InputSystem::Instance().AppendKey("ESCAPE", GLFW_KEY_F1, GLFW_GAMEPAD_BUTTON_START);
		InputSystem::Instance().AppendKey("MENUSELECT", GLFW_KEY_ENTER, GLFW_GAMEPAD_BUTTON_A);
		InputSystem::Instance().AppendKey("MENUBACK", GLFW_KEY_BACKSPACE, GLFW_GAMEPAD_BUTTON_B);
		InputSystem::Instance().AppendKey("MENUUP", GLFW_KEY_W, GLFW_GAMEPAD_BUTTON_DPAD_UP);
		InputSystem::Instance().AppendKey("MENUDOWN", GLFW_KEY_S, GLFW_GAMEPAD_BUTTON_DPAD_DOWN);
		InputSystem::Instance().AppendKey("MENULEFT", GLFW_KEY_A, GLFW_GAMEPAD_BUTTON_DPAD_LEFT);
		InputSystem::Instance().AppendKey("MENURIGHT", GLFW_KEY_D, GLFW_GAMEPAD_BUTTON_DPAD_RIGHT);

		InputSystem::Instance().AppendAxis("MOVEX", GLFW_GAMEPAD_AXIS_LEFT_X, false, true, 0.2f, 0.f);
		InputSystem::Instance().AppendAxis("MOVEY", GLFW_GAMEPAD_AXIS_LEFT_Y, false, true, 0.2f, 0.f);

		InputSystem::Instance().AppendAxis("AIMX", GLFW_GAMEPAD_AXIS_RIGHT_X, false, true, 0.2f, 0.f);
		InputSystem::Instance().AppendAxis("AIMY", GLFW_GAMEPAD_AXIS_RIGHT_Y, false, true, 0.2f, 0.f);

		//Temp buttons for mouse input test pelase clean up
		InputSystem::Instance().AppendKey("RIGHTCLICK", GLFW_MOUSE_BUTTON_RIGHT);
		InputSystem::Instance().AppendKey("LEFTCLICK", GLFW_MOUSE_BUTTON_LEFT);

		GLFWInputManager::Instance().SetActiveWindow(window);

		glfwSetKeyCallback(window, GLFWInputManager::key_cb);
		glfwSetMouseButtonCallback(window, GLFWInputManager::mousebutton_cb);

		glfwSetJoystickCallback(GLFWInputManager::joystick_callback);

		for (int i = 0; i <= GLFW_JOYSTICK_LAST; ++i)
		{
			if (glfwJoystickPresent(i))
				GLFWInputManager::joystick_callback(i, GLFW_CONNECTED);
		}
	}

	/******************************************************************************/
	/*!
	\brief
		Update booleans to correctly reflect triggered/release behaviour
	\param c_dt
		The delta time for this frame
	*/
	/******************************************************************************/
	void InputSystem::PollEvents(float c_dt)
	{
		/*static int layout = 1;
		if (GLFWInputManager::Instance().onControllerTriggered(GLFW_GAMEPAD_BUTTON_BACK))
		{
			++layout;
			layout = Quark::Math::WrapAround(layout, 0, 1);
			setControllerLayout(layout);
		}//*/
		GLFWInputManager::Instance().pollEvents();

		float lightVibration = 0.f;
		float heavyVibration = 0.f;

		for (int i = 0; i < m_instance->m_vibrationStack.size(); i++)
		{
			m_instance->m_vibrationStack[i].m_timer -= c_dt;

			if (m_instance->m_vibrationStack[i].m_motor == VibrateInfo::Motor::LIGHT)
			{
				lightVibration += m_instance->m_vibrationStack[i].m_vibrationStrength;
			}
			else
			{
				heavyVibration += m_instance->m_vibrationStack[i].m_vibrationStrength;
			}

			if (m_instance->m_vibrationStack[i].m_timer <= 0)
			{
				m_instance->m_vibrationStack.erase(m_instance->m_vibrationStack.begin() + i);
				i--;
			}
		}

		xInput::Instance().Vibrate(lightVibration, heavyVibration);
		xInput::Instance().UpdateControllerVibration();
	}

	/******************************************************************************/
	/*!
	\brief
		Vibrates the heavy controller motor if a controller is connected
	\param amount
		The amount of force of vibration, range (0 ~ 1)
	\param duration
		The duration to vibrate for in seconds
	\param compounded
		Whether this should stack and compound if called multiple times
		if set to false, will completely override old values
	*/
	/******************************************************************************/
	void InputSystem::VibrateControllerLightMotor(float amount, float duration, bool compounded)
	{
		if (!compounded)
			Instance().m_vibrationStack.clear();

		Instance().m_vibrationStack.push_back({ VibrateInfo::Motor::LIGHT, amount, duration });
	}

	/******************************************************************************/
	/*!
	\brief
		Vibrates the heavy controller motor if a controller is connected
	\param amount
		The amount of force of vibration, range (0 ~ 1)
	\param duration
		The duration to vibrate for in seconds
	\param compounded
		Whether this should stack and compound if called multiple times
		if set to false, will completely override old values
	*/
	/******************************************************************************/
	void InputSystem::VibrateControllerHeavyMotor(float amount, float duration, bool compounded)
	{
		if (!compounded)
			Instance().m_vibrationStack.clear();

		Instance().m_vibrationStack.push_back({ VibrateInfo::Motor::HEAVY, amount, duration });
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the key is held down
	\param keyID
		the key
	\returns
		returns true if key is held down
	*/
	/******************************************************************************/
	bool InputSystem::onKeyHeld(std::string keyID)
	{
		auto& inputMap = Instance().m_inputMap;

		if (inputMap.find(keyID) != inputMap.end())
		{
			keyCode& button = inputMap[keyID];

			if (button.controller >= 0 && GLFWInputManager::Instance().onControllerHeld(button.controller))
				return true;

			if (GLFWInputManager::Instance().onKeyHeld(button.keyboard))
				return true;
		}
		return false;
	}

	// For Mono
	bool InputSystem::onKeyHeldMono(MonoString* keyID)
	{
		const char* _keyID = mono_string_to_utf8(keyID);
		return onKeyHeld(_keyID);
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the key is triggered
	\param keyID
		the key
	\returns
		returns true if key is triggered
	*/
	/******************************************************************************/
	bool InputSystem::onKeyTriggered(std::string keyID)
	{
		auto& inputMap = Instance().m_inputMap;

		if (inputMap.find(keyID) != inputMap.end())
		{
			keyCode& button = inputMap[keyID];

			if (button.controller >= 0 && GLFWInputManager::Instance().onControllerTriggered(button.controller))
				return true;

			if (GLFWInputManager::Instance().onKeyTriggered(button.keyboard))
				return true;
		}
		return false;
	}

	// For Mono
	bool InputSystem::onKeyTriggeredMono(MonoString* keyID)
	{
		const char* _keyID = mono_string_to_utf8(keyID);
		return onKeyTriggered(_keyID);
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the key is released
	\param keyID
		the key
	\returns
		returns true if key is released
	*/
	/******************************************************************************/
	bool InputSystem::onKeyReleased(std::string keyID)
	{
		auto& inputMap = Instance().m_inputMap;

		if (inputMap.find(keyID) != inputMap.end())
		{
			keyCode& button = inputMap[keyID];

			if (button.controller >= 0 && GLFWInputManager::Instance().onControllerReleased(button.controller))
				return true;

			if (GLFWInputManager::Instance().onKeyReleased(button.keyboard))
				return true;
		}
		return false;
	}

	// For Mono
	bool InputSystem::onKeyReleasedMono(MonoString* keyID)
	{
		const char* _keyID = mono_string_to_utf8(keyID);
		return onKeyReleased(_keyID);
	}

	bool InputSystem::onKeyTriggered(int key)
	{
		return PPI::GLFWInputManager::Instance().onKeyTriggered(key);
	}

	bool InputSystem::onKeyReleased(int key)
	{
		return PPI::GLFWInputManager::Instance().onKeyReleased(key);
	}

	bool InputSystem::onKeyHeld(int key)
	{
		return PPI::GLFWInputManager::Instance().onKeyHeld(key);
	}

	/******************************************************************************/
	/*!
	\brief
		Returns true if any key is pressed on controller or keyboard
	*/
	/******************************************************************************/
	bool InputSystem::onAnyKey()
	{
		return PPI::GLFWInputManager::Instance().onAnyKey();
	}

	/******************************************************************************/
	/*!
	\brief
		Returns true if controller is being used
	*/
	/******************************************************************************/
	bool InputSystem::IsUsingController()
	{
		return PPI::GLFWInputManager::Instance().IsUsingController();
	}

	/******************************************************************************/
	/*!
	\brief
		Gets the controller axis
	*/
	/******************************************************************************/
	float InputSystem::GetAxis(const std::string& axisID)
	{
		auto axis = Instance().m_axisMap.find(axisID);

		if (axis != Instance().m_axisMap.end())
		{
			auto rawAxis = GLFWInputManager::Instance().GetControllerAxis((*axis).second.axisID);

			//2

			
			//2 - (*axis).second.upperDeadzone - (*axis).second.lowerDeadzone

			//Compute deadzone

			if ((*axis).second.UseBoundedDeadzone)
			{
				if (rawAxis < -1 + (*axis).second.lowerDeadzone)
					return -1.f;
				if (rawAxis > 1 - (*axis).second.upperDeadzone)
					return 1.f;
				else
				{
					rawAxis *= 2 / (2 - (*axis).second.upperDeadzone - (*axis).second.lowerDeadzone);
					rawAxis += (*axis).second.upperDeadzone - (*axis).second.lowerDeadzone;
				}
			}
			else if ((*axis).second.UseMiddleDeadzone)
			{
				if (rawAxis < -(*axis).second.lowerDeadzone)
				{
					rawAxis *= (1.f + ((*axis).second.upperDeadzone + (*axis).second.lowerDeadzone));
					rawAxis += (*axis).second.lowerDeadzone;
				}
				else if (rawAxis > (*axis).second.lowerDeadzone)
				{
					rawAxis *= (1.f + ((*axis).second.upperDeadzone + (*axis).second.lowerDeadzone));
					rawAxis -= (*axis).second.lowerDeadzone;
					
				}
				else
					return 0.0f;
			}

			return glm::clamp(rawAxis, -1.f, 1.f);
		}
		return 0.0f;
	}

	float InputSystem::GetAxisMono(MonoString* axisID)
	{
		const char* _axisID = mono_string_to_utf8(axisID);
		return GetAxis(_axisID);
	}

	/******************************************************************************/
	/*!
	\brief
		Adds a keyboard/button control
	*/
	/******************************************************************************/
	void InputSystem::AppendKey(const std::string& keyID, int keyboardKey, int controllerKey)
	{
		Instance().m_inputMap[keyID] = keyCode{ keyboardKey, controllerKey };
	}

	/******************************************************************************/
	/*!
	\brief
		Adds a axis control
	*/
	/******************************************************************************/
	void InputSystem::AppendAxis(const std::string& keyID, int axisID, bool enableBoundDeadzone, bool enableMiddleDeadzone, float lowerBound, float upperBound)
	{
		Instance().m_axisMap[keyID] = ControllerAxis{ axisID, enableMiddleDeadzone, enableBoundDeadzone, lowerBound, upperBound };
	}

	/******************************************************************************/
	/*!
	\brief
		Clears all bindings
	*/
	/******************************************************************************/
	void InputSystem::ClearBindings()
	{
		Instance().m_inputMap.clear();
		Instance().m_axisMap.clear();
	}

	/******************************************************************************/
	/*!
	\brief
		Checks if a controller is connected
	\returns
		returns true if there is more than 1 controller
	*/
	/******************************************************************************/
	bool InputSystem::ControllerConnected()
	{
		return (GLFWInputManager::Instance().controllersConnected() > 0);
	}

	/******************************************************************************/
	/*!
	\brief
		Gets the singleton instance of InputSystem, creates one if doesn't
		exist
	\returns
		returns a InputSystem instance
	*/
	/******************************************************************************/
	InputSystem& InputSystem::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new InputSystem);
			});

		return *m_instance.get();
	}
}