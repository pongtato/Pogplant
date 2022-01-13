/******************************************************************************/
/*!
\file	GLFWInput.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	A input handler to handle GLFW keycallbacks

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef GLFWINPUT_H_
#define GLFWINPUT_H_

#include <glfw3.h>
#include <unordered_map>
#include <set>
#include <array>
#include <glm.hpp>
#include <memory>
#include <mutex>

namespace PPI
{
	/******************************************************************************/
	/*!
	\class GLFWInputManager
	\brief
		GLFW input manager wrapper
	*/
	/******************************************************************************/
	class GLFWInputManager
	{
	public:
		~GLFWInputManager() = default;

		struct Key
		{
			int ID = 0;
			bool triggered = true;
			bool released = false;
		};

		std::set<int> m_connectedControllers;

		//member functions
		void pollEvents();

		static bool onKeyTriggered(int key);
		static bool onKeyTriggeredMono(int key);
		static bool onKeyReleased(int key);
		static bool onKeyReleasedMono(int key);
		static bool onKeyHeld(int key);
		static bool onKeyHeldMono(int key);

		static bool onControllerTriggered(int key);
		static bool onControllerReleased(int key);
		static bool onControllerHeld(int key);

		static float GetControllerAxis(int axisKey);

		static bool onAnyKey();

		static bool IsUsingController();

		inline size_t controllersConnected()
		{
			return m_connectedControllers.size();
		}

		void setActiveController(int controllerID);

		void getMouse(double& x, double& y);

		static void key_cb(GLFWwindow* pwin, int key, int scancode, int action, int mod);
		static void mousebutton_cb(GLFWwindow* pwin, int button, int action, int mod);
		static void joystick_callback(int jid, int event);

		static void SetActiveWindow(GLFWwindow* window);
		static GLFWInputManager& Instance();

	private:
		GLFWInputManager() = default;

		/**> Current GLFW window*/
		GLFWwindow* m_glfwWindow = nullptr;

		int m_mainController = 0;

		/**> singleton instance*/
		static std::unique_ptr<GLFWInputManager> m_instance;
		static std::once_flag m_onceFlag;

		/**> active keys map*/
		std::unordered_map<int, Key> m_activeKeys;

		/**> active controller input map*/
		std::unordered_map<int, Key> m_activeControllerKeys;

		std::array<bool, GLFW_GAMEPAD_BUTTON_LAST + GLFW_GAMEPAD_AXIS_LAST + 2> m_controllerPressed;
		std::array<float, GLFW_GAMEPAD_AXIS_LAST + 1> m_controllerAxis;

		void pressKey(int key, bool joystick = false);
		void releaseKey(int key, bool joystick = false);

		bool m_controllerUsed = false;
	};
}

#endif // !INPUT_MANAGER_H
