/******************************************************************************/
/*!
\file	WindowsXInput.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Uses xinput to implement basic functionality for controller vibration

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef WINDOWS_X_INPUT_H
#define WINDOWS_X_INPUT_H

#pragma warning(disable : 4005)

#define NOMINMAX
#include <Windows.h>
#include <Xinput.h>

#include <memory>
#include <mutex>

namespace PPI
{
	class xInput
	{
	public:
		~xInput() = default;

		static xInput& Instance();

		void UpdateControllerVibration();
		void VibrateHeavyMotor(float amount);
		void VibrateLightMotor(float amount);
		void Vibrate(float lightAmount, float heavyAmount);

	private:
		xInput();

		XINPUT_VIBRATION m_vibration;

		/**> singleton instance*/
		static std::unique_ptr<xInput> m_instance;
		static std::once_flag m_onceFlag;
	};
}

#endif