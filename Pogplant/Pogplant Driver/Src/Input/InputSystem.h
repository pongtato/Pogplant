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

#include "../Panels.h"

#include <glfw3.h>
#include <unordered_map>
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>

#include <memory>
#include <mutex>

namespace PPI
{
	class InputSystem
	{
	public:
		struct keyCode
		{
			int keyboard;
			int controller;
		};

		~InputSystem() = default;

		static void Init(GLFWwindow* window);
		static void PollEvents();

		void setControllerLayout(int layout);

		bool onKeyHeld(std::string keyID);
		static bool onKeyHeldMono(MonoString* keyID);
		bool onKeyTriggered(std::string keyID);
		static bool onKeyTriggeredMono(MonoString* keyID);
		bool onKeyReleased(std::string keyID);
		static bool onKeyReleasedMono(MonoString* keyID);

		//float getKeyAxis(std::string keyID);

		static void AppendKey(std::string keyID, int keyboardKey, int controllerKey = -1);
		static void ClearBindings();

		inline static std::unordered_map<std::string, keyCode>& GetInputMap()
		{
			return Instance().m_inputMap;
		}

		static bool ControllerConnected();

		static InputSystem& Instance();

	private:
		InputSystem() = default;
		
		/**> singleton instance*/
		static std::unique_ptr<InputSystem> m_instance;
		static std::once_flag m_onceFlag;

		std::unordered_map<std::string, keyCode> m_inputMap;
	};
}

#endif // !INPUT_SYSTEM
