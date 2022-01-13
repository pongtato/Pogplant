/******************************************************************************/
/*!
\file	GLFWInput.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	A input handler to handle GLFW keycallbacks

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "GLFWInput.h"

#include <imgui_impl_glfw.h>
#include <iostream>

#define JOYSTICKOSET GLFW_GAMEPAD_BUTTON_LAST + 1
#define TRIGGERBUTTONPOINT 0.6f

namespace PPI
{
	std::unique_ptr<GLFWInputManager> GLFWInputManager::m_instance = nullptr;
	std::once_flag GLFWInputManager::m_onceFlag;

	/******************************************************************************/
	/*!
	\brief
		Update booleans to correctly reflect triggered/release behaviour
	*/
	/******************************************************************************/
	void GLFWInputManager::pollEvents()
	{
		for (auto it = m_activeControllerKeys.begin(); it != m_activeControllerKeys.end();)
		{
			it->second.triggered = false;
			if (it->second.released)
			{
				m_activeControllerKeys.erase(it++);
			}
			else
				++it;
		}

		for (auto it = m_activeKeys.begin(); it != m_activeKeys.end();)
		{
			it->second.triggered = false;
			if (it->second.released)
			{
				m_activeKeys.erase(it++);
			}
			else
				++it;
		}

		if (!m_connectedControllers.empty())
		{
			/*Controller checking*/
			/*if (m_connectedControllers.size() >= 2)
			{
				auto it = m_connectedControllers.cbegin();
				while (it != m_connectedControllers.cend())
				{
					GLFWgamepadstate state;

					if (glfwGetGamepadState((*it), &state))
					{
						if (state.buttons[GLFW_GAMEPAD_BUTTON_A] || state.buttons[GLFW_GAMEPAD_BUTTON_START])
							m_mainController = (*it);
					}

					++it;
				}
			}*/

			//Get active controller data
			GLFWgamepadstate state;
			if (glfwGetGamepadState(m_mainController, &state))
			{
				for (size_t i = 0; i <= GLFW_GAMEPAD_BUTTON_LAST; i++)
				{
					if (state.buttons[i] && !m_controllerPressed[i])
					{
						m_controllerPressed[i] = true;
						pressKey((int)i, true);

						m_controllerUsed = true;
					}
					else if (!state.buttons[i] && m_controllerPressed[i])
					{
						m_controllerPressed[i] = false;
						releaseKey((int)i, true);

						m_controllerUsed = true;
					}
				}

				//Update axis as buttons
				for (size_t i = 0; i <= GLFW_GAMEPAD_AXIS_LAST; i++)
				{
					m_controllerAxis[i] = state.axes[i];
					size_t iOffset = i + JOYSTICKOSET;

					if (m_controllerAxis[i] > TRIGGERBUTTONPOINT && !m_controllerPressed[iOffset])
					{
						m_controllerPressed[iOffset] = true;
						pressKey((int)(iOffset), true);

						m_controllerUsed = true;
					}
					else if (!(m_controllerAxis[i] > TRIGGERBUTTONPOINT) && m_controllerPressed[iOffset])
					{
						m_controllerPressed[iOffset] = false;
						releaseKey((int)(iOffset), true);

						m_controllerUsed = true;
					}
				}
			}
		}
	}

	/******************************************************************************/
	/*!
	\brief
		activates a key
	\param key
		the key to activate
	\param bool
		set as joystick press
	*/
	/******************************************************************************/
	void GLFWInputManager::pressKey(int key, bool joystick)
	{
		if (joystick)
			m_activeControllerKeys[key] = Key{ key, true, false };
		else
			m_activeKeys[key] = Key{ key, true, false };
	}

	/******************************************************************************/
	/*!
	\brief
		releases a key
	\param key
		the key to activate
	\param bool
		set as joystick press
	*/
	/******************************************************************************/
	void GLFWInputManager::releaseKey(int key, bool joystick)
	{
		if (joystick)
		{
			auto it = m_activeControllerKeys.find(key);
			if (it != m_activeControllerKeys.end())
			{
				it->second.released = true;
			}
		}
		else
		{
			auto it = m_activeKeys.find(key);
			if (it != m_activeKeys.end())
			{
				it->second.released = true;
			}
		}
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the key is triggered
	\param key
		the key
	\returns
		returns true if key is triggered
	*/
	/******************************************************************************/
	bool GLFWInputManager::onKeyTriggered(int key)
	{
		auto it = Instance().m_activeKeys.find(key);
		if (it == Instance().m_activeKeys.cend())
			return false;

		return it->second.triggered;
	}

	// Mono function
	bool GLFWInputManager::onKeyTriggeredMono(int key)
	{
		return Instance().onKeyTriggered(key);
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the key is released
	\param key
		the key
	\returns
		returns true if key is released
	*/
	/******************************************************************************/
	bool GLFWInputManager::onKeyReleased(int key)
	{
		auto it = Instance().m_activeKeys.find(key);
		if (it == Instance().m_activeKeys.cend())
			return false;

		return it->second.released;
	}

	// Mono function
	bool GLFWInputManager::onKeyReleasedMono(int key)
	{
		return Instance().onKeyReleased(key);
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the key is held down
	\param key
		the key
	\returns
		returns true if key is held down
	*/
	/******************************************************************************/
	bool GLFWInputManager::onKeyHeld(int key)
	{
		return Instance().m_activeKeys.find(key) != Instance().m_activeKeys.cend();
	}

	// Mono function
	bool GLFWInputManager::onKeyHeldMono(int key)
	{
		return Instance().onKeyHeld(key);
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the controller button is triggered
	\param key
		the key
	\returns
		returns true if key is triggered
	*/
	/******************************************************************************/
	bool GLFWInputManager::onControllerTriggered(int key)
	{
		auto it = Instance().m_activeControllerKeys.find(key);
		if (it == Instance().m_activeControllerKeys.cend())
			return false;

		return it->second.triggered;
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the controller button is released
	\param key
		the key
	\returns
		returns true if key is released
	*/
	/******************************************************************************/
	bool GLFWInputManager::onControllerReleased(int key)
	{
		auto it = Instance().m_activeControllerKeys.find(key);
		if (it == Instance().m_activeControllerKeys.cend())
			return false;

		return it->second.released;
	}

	/******************************************************************************/
	/*!
	\brief
		checks if the controller button is held down
	\param key
		the key
	\returns
		returns true if key is held down
	*/
	/******************************************************************************/
	bool GLFWInputManager::onControllerHeld(int key)
	{
		return Instance().m_activeControllerKeys.find(key) != Instance().m_activeControllerKeys.cend();
	}

	/******************************************************************************/
	/*!
	\brief
		Gets the controller axis
	\param axisKey
		The key of the axis
	\returns
		float value controller axis
	*/
	/******************************************************************************/
	float GLFWInputManager::GetControllerAxis(int axisKey)
	{
		assert(axisKey < GLFW_GAMEPAD_AXIS_LAST + 1);
		return Instance().m_controllerAxis[axisKey];
	}

	bool GLFWInputManager::onAnyKey()
	{
		return Instance().m_activeKeys.size() > 0 || Instance().m_activeControllerKeys.size() > 0;
	}

	bool GLFWInputManager::IsUsingController()
	{
		return Instance().m_controllerUsed;
	}

	void GLFWInputManager::setActiveController(int controllerID)
	{
		m_mainController = controllerID;
	}

	/******************************************************************************/
	/*!
	\brief
		gets cursor position in the window and returns it by editing the referenced
		values
	\param x
		the X axis position
	\param y
		the Y axis position
	*/
	/******************************************************************************/
	void GLFWInputManager::getMouse(double& x, double& y)
	{
		glfwGetCursorPos(m_instance->m_glfwWindow, &x, &y);
	}

	/******************************************************************************/
	/*!
	\brief
		Sets the active window
	\param window
		The window to get input fromt
	*/
	/******************************************************************************/
	void GLFWInputManager::SetActiveWindow(GLFWwindow* window)
	{
		m_instance->m_glfwWindow = window;
	}

	/******************************************************************************/
	/*!
	\brief
		Gets the singleton instance of GLFWInputManager, creates one if doesn't
		exist
	\returns
		returns a GLFWInputManager instance
	*/
	/******************************************************************************/
	GLFWInputManager& GLFWInputManager::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new GLFWInputManager);
			});

		return *m_instance.get();
	}

	/******************************************************************************/
	/*!
	\brief
		GLFW Key callback
	*/
	/******************************************************************************/
	void GLFWInputManager::key_cb(GLFWwindow* pwin, int key, int scancode, int action, int mod)
	{
		ImGui_ImplGlfw_KeyCallback(pwin, key, scancode, action, mod);

		if (GLFW_PRESS == action)
		{
			m_instance->pressKey(key);
		}
		else if (GLFW_REPEAT == action)
		{
		}
		else if (GLFW_RELEASE == action)
		{
			m_instance->releaseKey(key);
		}

		m_instance->m_controllerUsed = false;
	}

	/******************************************************************************/
	/*!
	\brief
		GLFW mouse Key callback
	*/
	/******************************************************************************/
	void GLFWInputManager::mousebutton_cb(GLFWwindow* pwin, int button, int action, int mod)
	{
		ImGui_ImplGlfw_MouseButtonCallback(pwin, button, action, mod);

		if (GLFW_PRESS == action)
		{
			m_instance->pressKey(button);
		}
		else if (GLFW_REPEAT == action)
		{

		}
		else if (GLFW_RELEASE == action)
		{
			m_instance->releaseKey(button);
		}

		m_instance->m_controllerUsed = false;

		/*switch (button)
		{
		case GLFW_MOUSE_BUTTON_LEFT:
			break;
		case GLFW_MOUSE_BUTTON_RIGHT:
			break;
		}
		switch (action)
		{
		case GLFW_PRESS:
			break;
		case GLFW_RELEASE:
			break;
		}//*/
	}

	/******************************************************************************/
	/*!
	\brief
		GLFW joystick connection callback
	*/
	/******************************************************************************/
	void GLFWInputManager::joystick_callback(int jid, int event)
	{
		if (event == GLFW_CONNECTED)
		{
			if (glfwJoystickPresent(jid) && glfwJoystickIsGamepad(jid))
			{
				std::cout << "Controller Connected: " << glfwGetJoystickName(jid) << " ID(" << jid << ")" << std::endl;
				m_instance->m_connectedControllers.insert(jid);
				m_instance->m_mainController = jid;
			}
		}
		else if (event == GLFW_DISCONNECTED)
		{
			if (m_instance->m_connectedControllers.find(jid) != m_instance->m_connectedControllers.end())
			{
				std::cout << "Controller Disconnected: ID(" << jid << ")" << std::endl;
				m_instance->m_connectedControllers.erase(jid);

				if (!m_instance->m_connectedControllers.empty())
					m_instance->m_mainController = *m_instance->m_connectedControllers.crbegin();
				else
					m_instance->m_mainController = 0;
			}
		}
	}
}