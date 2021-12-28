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

		struct ControllerAxis
		{
			int axisID;

			bool UseMiddleDeadzone = false;
			bool UseBoundedDeadzone = true;

			float lowerDeadzone = 0.1f;
			float upperDeadzone = 0.1f;
		};

		~InputSystem() = default;

		static void Init(GLFWwindow* window);
		static void PollEvents(float c_dt);

		static void VibrateControllerLightMotor(float amount, float duration, bool compounded = true);
		static void VibrateControllerHeavyMotor(float amount, float duration, bool compounded = true);

		static bool onKeyHeld(std::string keyID);
		static bool onKeyHeldMono(MonoString* keyID);
		static bool onKeyTriggered(std::string keyID);
		static bool onKeyTriggeredMono(MonoString* keyID);
		static bool onKeyReleased(std::string keyID);
		static bool onKeyReleasedMono(MonoString* keyID);

		static bool onKeyTriggered(int key);
		static bool onKeyReleased(int key);
		static bool onKeyHeld(int key);

		static float GetAxis(const std::string& axisID);
		static float GetAxisMono(MonoString* axisID);

		//float getKeyAxis(std::string keyID);

		static void AppendKey(const std::string& keyID, int keyboardKey, int controllerKey = -1);
		static void AppendAxis(const std::string& keyID, int axisID, bool enableBoundDeadzone = false, bool enableMiddleDeadzone = false, float lowerBound = 0.1f, float upperBound = 0.1f);
		static void ClearBindings();

		inline static std::unordered_map<std::string, keyCode>& GetInputMap()
		{
			return Instance().m_inputMap;
		}

		inline static std::unordered_map<std::string, ControllerAxis>& GetAxisMap()
		{
			return Instance().m_axisMap;
		}

		static bool ControllerConnected();

		static InputSystem& Instance();

	private:
		InputSystem() = default;
		
		struct VibrateInfo
		{
			enum class Motor
			{
				HEAVY,
				LIGHT,
				TRIGGERLEFT,
				TRIGGERRIGHT
			};

			Motor m_motor = Motor::LIGHT;
			float m_vibrationStrength = 0.f;
			float m_timer = 0.f;
		};

		/**> singleton instance*/
		static std::unique_ptr<InputSystem> m_instance;
		static std::once_flag m_onceFlag;

		std::unordered_map<std::string, keyCode> m_inputMap;
		std::unordered_map<std::string, ControllerAxis> m_axisMap;

		std::vector<VibrateInfo> m_vibrationStack;
	};
}

#endif // !INPUT_SYSTEM
