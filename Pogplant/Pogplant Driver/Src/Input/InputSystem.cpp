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
	InputSystem* InputSystem::m_instance = nullptr;

	/******************************************************************************/
	/*!
	\brief
		Initialise the inputsystem controls
	*/
	/******************************************************************************/
	void InputSystem::Init(GLFWwindow* window)
	{
		//All this control initialisation should be done in the editor eventually as an endgame

		InputSystem::Instance()->appendKey("LEFT", GLFW_KEY_A);
		InputSystem::Instance()->appendKey("RIGHT", GLFW_KEY_D);

		InputSystem::Instance()->appendKey("DOWN", GLFW_KEY_S, GLFW_GAMEPAD_AXIS_LEFT_Y + JOYSTICKOSET);

		//SaveFileHandler::Instance()->assignData<unsigned short>(SaveFileHandler::S_CONTROL_JUMP, keyGetter, GLFW_KEY_SPACE, false);
		setControllerLayout(1);

		/*Set menu buttons*/
		InputSystem::Instance()->appendKey("ESCAPE", GLFW_KEY_ESCAPE, GLFW_GAMEPAD_BUTTON_START);
		InputSystem::Instance()->appendKey("MENUSELECT", GLFW_KEY_ENTER, GLFW_GAMEPAD_BUTTON_A);
		InputSystem::Instance()->appendKey("MENUBACK", GLFW_KEY_BACKSPACE, GLFW_GAMEPAD_BUTTON_B);
		InputSystem::Instance()->appendKey("MENUUP", GLFW_KEY_W, GLFW_GAMEPAD_BUTTON_DPAD_UP);
		InputSystem::Instance()->appendKey("MENUDOWN", GLFW_KEY_S, GLFW_GAMEPAD_BUTTON_DPAD_DOWN);
		InputSystem::Instance()->appendKey("MENULEFT", GLFW_KEY_A, GLFW_GAMEPAD_BUTTON_DPAD_LEFT);
		InputSystem::Instance()->appendKey("MENURIGHT", GLFW_KEY_D, GLFW_GAMEPAD_BUTTON_DPAD_RIGHT);

		GLFWInputManager::Instance()->SetActiveWindow(window);

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
	void InputSystem::pollEvents()
	{
		/*static int layout = 1;
		if (GLFWInputManager::Instance()->onControllerTriggered(GLFW_GAMEPAD_BUTTON_BACK))
		{
			++layout;
			layout = Quark::Math::WrapAround(layout, 0, 1);
			setControllerLayout(layout);
		}//*/
		GLFWInputManager::Instance()->pollEvents();
	}

	void InputSystem::setControllerLayout(int layout)
	{
		std::cout << "Controller Layout (" << layout << ") ";
		switch (layout)
		{
		default:
		case 0:
			std::cout << "Michael's Layout ";
			InputSystem::Instance()->appendKey("JUMP", GLFW_KEY_SPACE, GLFW_GAMEPAD_BUTTON_A);
			InputSystem::Instance()->appendKey("SPRINT", GLFW_KEY_LEFT_SHIFT, GLFW_GAMEPAD_BUTTON_X);
			InputSystem::Instance()->appendKey("ATTACK", GLFW_MOUSE_BUTTON_LEFT, GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER + JOYSTICKOSET);
			InputSystem::Instance()->appendKey("RECALL", GLFW_MOUSE_BUTTON_RIGHT, GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER);
			InputSystem::Instance()->appendKey("HEAL", GLFW_KEY_Q, GLFW_GAMEPAD_BUTTON_B);
			InputSystem::Instance()->appendKey("COUNTER", GLFW_KEY_E, GLFW_GAMEPAD_AXIS_LEFT_TRIGGER + JOYSTICKOSET);
			break;
		case 1:
			std::cout << "Monster Hunter Style ";
			InputSystem::Instance()->appendKey("JUMP", GLFW_KEY_SPACE, GLFW_GAMEPAD_BUTTON_A);
			InputSystem::Instance()->appendKey("SPRINT", GLFW_KEY_LEFT_SHIFT, GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER);
			InputSystem::Instance()->appendKey("ATTACK", GLFW_MOUSE_BUTTON_LEFT, GLFW_GAMEPAD_BUTTON_B);
			InputSystem::Instance()->appendKey("RECALL", GLFW_MOUSE_BUTTON_RIGHT, GLFW_GAMEPAD_BUTTON_Y);
			InputSystem::Instance()->appendKey("HEAL", GLFW_KEY_Q, GLFW_GAMEPAD_BUTTON_X);
			InputSystem::Instance()->appendKey("COUNTER", GLFW_KEY_E, GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER + JOYSTICKOSET);
			break;
		}

		std::cout << "loaded" << std::endl;
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

			if (button.controller >= 0 && GLFWInputManager::Instance()->onControllerHeld(button.controller))
				return true;

			if (GLFWInputManager::Instance()->onKeyHeld(button.keyboard))
				return true;
		}
		return false;
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

			if (button.controller >= 0 && GLFWInputManager::Instance()->onControllerTriggered(button.controller))
				return true;

			if (GLFWInputManager::Instance()->onKeyTriggered(button.keyboard))
				return true;
		}
		return false;
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

			if (button.controller >= 0 && GLFWInputManager::Instance()->onControllerReleased(button.controller))
				return true;

			if (GLFWInputManager::Instance()->onKeyReleased(button.keyboard))
				return true;
		}
		return false;
	}

	void InputSystem::appendKey(std::string keyID, int keyboardKey, int controllerKey)
	{
		m_inputMap[keyID] = keyCode{ keyboardKey, controllerKey };
	}

	/******************************************************************************/
	/*!
	\brief
		Checks if a controller is connected
	\returns
		returns true if there is more than 1 controller
	*/
	/******************************************************************************/
	bool InputSystem::controllerConnected()
	{
		return (GLFWInputManager::Instance()->controllersConnected() > 0);
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
	InputSystem* InputSystem::Instance()
	{
		if (!m_instance)
			m_instance = new InputSystem;

		return m_instance;
	}

	/******************************************************************************/
	/*!
	\brief
		Destroys the singleton instance of InputSystem
	*/
	/******************************************************************************/
	void InputSystem::Destroy()
	{
		GLFWInputManager::Destroy();

		if (m_instance)
			delete m_instance;
		m_instance = nullptr;
	}
}