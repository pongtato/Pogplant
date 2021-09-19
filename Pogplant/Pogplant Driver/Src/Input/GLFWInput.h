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

namespace Pogplant
{
	namespace Input
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
			GLFWInputManager() = default;
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

			bool onKeyTriggered(int key) const;
			bool onKeyReleased(int key) const;
			bool onKeyHeld(int key) const;

			bool onControllerTriggered(int key) const;
			bool onControllerReleased(int key) const;
			bool onControllerHeld(int key) const;

			glm::vec2 getLeftThumbAxis();
			glm::vec2 getRightThumbAxis();
			float getLeftTrigger();
			float getRightTrigger();

			inline size_t controllersConnected()
			{
				return m_connectedControllers.size();
			}

			void setActiveController(int controllerID);

			void setThumbDeadzone(float value);

			void getMouse(double& x, double& y);

			static void key_cb(GLFWwindow* pwin, int key, int scancode, int action, int mod);
			static void mousebutton_cb(GLFWwindow* pwin, int button, int action, int mod);
			static void joystick_callback(int jid, int event);

			static void SetActiveWindow(GLFWwindow* window);
			static GLFWInputManager* Instance();
			static void Destroy();
		private:
			/**> Current GLFW window*/
			GLFWwindow* m_glfwWindow = nullptr;

			/**> Controller thumbstick deadzone*/
			float m_thumbStickDeadzone = 0.15f;

			int m_mainController = 0;

			/**> singleton instance*/
			static GLFWInputManager* m_instance;

			/**> active keys map*/
			std::unordered_map<int, Key> m_activeKeys;

			/**> active controller input map*/
			std::unordered_map<int, Key> m_activeControllerKeys;

			std::array<bool, GLFW_GAMEPAD_BUTTON_LAST + GLFW_GAMEPAD_AXIS_LAST + 2> m_controllerPressed;
			std::array<float, GLFW_GAMEPAD_AXIS_LAST + 1> m_controllerAxis;

			void pressKey(int key, bool joystick = false);
			void releaseKey(int key, bool joystick = false);
		};
	}
}

#endif // !INPUT_MANAGER_H
