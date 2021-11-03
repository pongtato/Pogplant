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
		//All this control initialisation should be done in the editor eventually as an endgame

		InputSystem::Instance().AppendKey("LEFT", GLFW_KEY_A);
		InputSystem::Instance().AppendKey("RIGHT", GLFW_KEY_D);
		InputSystem::Instance().AppendKey("DOWN", GLFW_KEY_S);
		InputSystem::Instance().AppendKey("UP", GLFW_KEY_W);

		//SaveFileHandler::Instance().assignData<unsigned short>(SaveFileHandler::S_CONTROL_JUMP, keyGetter, GLFW_KEY_SPACE, false);
		InputSystem::Instance().setControllerLayout(1);

		/*Set menu buttons*/
		InputSystem::Instance().AppendKey("ESCAPE", GLFW_KEY_ESCAPE, GLFW_GAMEPAD_BUTTON_START);
		InputSystem::Instance().AppendKey("MENUSELECT", GLFW_KEY_ENTER, GLFW_GAMEPAD_BUTTON_A);
		InputSystem::Instance().AppendKey("MENUBACK", GLFW_KEY_BACKSPACE, GLFW_GAMEPAD_BUTTON_B);
		InputSystem::Instance().AppendKey("MENUUP", GLFW_KEY_W, GLFW_GAMEPAD_BUTTON_DPAD_UP);
		InputSystem::Instance().AppendKey("MENUDOWN", GLFW_KEY_S, GLFW_GAMEPAD_BUTTON_DPAD_DOWN);
		InputSystem::Instance().AppendKey("MENULEFT", GLFW_KEY_A, GLFW_GAMEPAD_BUTTON_DPAD_LEFT);
		InputSystem::Instance().AppendKey("MENURIGHT", GLFW_KEY_D, GLFW_GAMEPAD_BUTTON_DPAD_RIGHT);

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
	*/
	/******************************************************************************/
	void InputSystem::PollEvents()
	{
		/*static int layout = 1;
		if (GLFWInputManager::Instance().onControllerTriggered(GLFW_GAMEPAD_BUTTON_BACK))
		{
			++layout;
			layout = Quark::Math::WrapAround(layout, 0, 1);
			setControllerLayout(layout);
		}//*/
		GLFWInputManager::Instance().pollEvents();
	}

	void InputSystem::setControllerLayout(int layout)
	{
		switch (layout)
		{
		default:
		case 0:
			InputSystem::Instance().AppendKey("JUMP", GLFW_KEY_SPACE, GLFW_GAMEPAD_BUTTON_A);
			InputSystem::Instance().AppendKey("SPRINT", GLFW_KEY_LEFT_SHIFT, GLFW_GAMEPAD_BUTTON_X);
			InputSystem::Instance().AppendKey("ATTACK", GLFW_MOUSE_BUTTON_LEFT, GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER + JOYSTICKOSET);
			InputSystem::Instance().AppendKey("RECALL", GLFW_MOUSE_BUTTON_RIGHT, GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER);
			InputSystem::Instance().AppendKey("HEAL", GLFW_KEY_Q, GLFW_GAMEPAD_BUTTON_B);
			InputSystem::Instance().AppendKey("COUNTER", GLFW_KEY_E, GLFW_GAMEPAD_AXIS_LEFT_TRIGGER + JOYSTICKOSET);
			break;
		case 1:
			InputSystem::Instance().AppendKey("JUMP", GLFW_KEY_SPACE, GLFW_GAMEPAD_BUTTON_A);
			InputSystem::Instance().AppendKey("SPRINT", GLFW_KEY_LEFT_SHIFT, GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER);
			InputSystem::Instance().AppendKey("ATTACK", GLFW_MOUSE_BUTTON_LEFT, GLFW_GAMEPAD_BUTTON_B);
			InputSystem::Instance().AppendKey("RECALL", GLFW_MOUSE_BUTTON_RIGHT, GLFW_GAMEPAD_BUTTON_Y);
			InputSystem::Instance().AppendKey("HEAL", GLFW_KEY_Q, GLFW_GAMEPAD_BUTTON_X);
			InputSystem::Instance().AppendKey("COUNTER", GLFW_KEY_E, GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER + JOYSTICKOSET);
			break;
		}
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
		if (m_inputMap.find(keyID) != m_inputMap.end())
		{
			keyCode& button = m_inputMap[keyID];

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
		return Instance().onKeyHeld(_keyID);
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
		if (m_inputMap.find(keyID) != m_inputMap.end())
		{
			keyCode& button = m_inputMap[keyID];

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
		return Instance().onKeyTriggered(_keyID);
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
		if (m_inputMap.find(keyID) != m_inputMap.end())
		{
			keyCode& button = m_inputMap[keyID];

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
		return Instance().onKeyReleased(_keyID);
	}

	void InputSystem::AppendKey(std::string keyID, int keyboardKey, int controllerKey)
	{
		Instance().m_inputMap[keyID] = keyCode{ keyboardKey, controllerKey };
	}

	void InputSystem::ClearBindings()
	{
		Instance().m_inputMap.clear();
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