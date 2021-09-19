/******************************************************************************/
/*!
\file	InputSystem.h
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
#ifndef INPUT_SYSTEM_H_
#define INPUT_SYSTEM_H_

#include <glfw3.h>
#include <unordered_map>

namespace Pogplant
{
	namespace Input
	{
		class InputSystem
		{
		public:
			void Init(GLFWwindow* window);
			void pollEvents();

			void setControllerLayout(int layout);

			bool onKeyHeld(std::string keyID);
			bool onKeyTriggered(std::string keyID);
			bool onKeyReleased(std::string keyID);

			//float getKeyAxis(std::string keyID);

			void appendKey(std::string keyID, int keyboardKey, int controllerKey = -1);

			bool controllerConnected();

			static InputSystem* Instance();
			static void Destroy();
			
		private:
			InputSystem() = default;
			~InputSystem() = default;

			struct keyCode
			{
				int keyboard;
				int controller;
			};

			/**> singleton instance*/
			static InputSystem* m_instance;

			std::unordered_map<std::string, keyCode> m_inputMap;
		};
	}
}

#endif // !INPUT_SYSTEM
