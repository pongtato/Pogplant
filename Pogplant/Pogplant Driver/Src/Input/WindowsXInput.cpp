/******************************************************************************/
/*!
\file	WindowsXInput.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Uses xinput to implement basic functionality for controller vibration

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "WindowsXInput.h"

std::unique_ptr<PPI::xInput> PPI::xInput::m_instance = nullptr;
std::once_flag PPI::xInput::m_onceFlag;

namespace PPI
{
	/**************************************************************************/
	/*!
	\brief
		Gets the singleton instance of xInput, creates one if doesn't
		exist
	\returns
		returns a xInput instance
	*/
	/**************************************************************************/
	xInput& xInput::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new xInput);
			});

		return *m_instance.get();
	}

	/**************************************************************************/
	/*!
	\brief
		Default constructor
	*/
	/**************************************************************************/
	xInput::xInput()
	{
		ZeroMemory(&m_vibration, sizeof(XINPUT_VIBRATION));
	}

	/**************************************************************************/
	/*!
	\brief
		Updates the xinput vibration state
	*/
	/**************************************************************************/
	void xInput::UpdateControllerVibration()
	{
		XInputSetState(0, &m_vibration);
	}

	/**************************************************************************/
	/*!
	\brief
		Vibrates the light motor
	*/
	/**************************************************************************/
	void xInput::VibrateLightMotor(float amount)
	{
		m_vibration.wRightMotorSpeed = (WORD)(amount * 65535.f);
	}

	/**************************************************************************/
	/*!
	\brief
		Vibrates the heavy motor
	*/
	/**************************************************************************/
	void xInput::VibrateHeavyMotor(float amount)
	{
		m_vibration.wLeftMotorSpeed = (WORD)(amount * 65535.f);
	}
	
	/**************************************************************************/
	/*!
	\brief
		Sets all the vibration motors
	*/
	/**************************************************************************/
	void xInput::Vibrate(float lightAmount, float heavyAmount)
	{
		m_vibration.wLeftMotorSpeed = (WORD)(heavyAmount * 65535.f);
		m_vibration.wRightMotorSpeed = (WORD)(lightAmount * 65535.f);
	}
}
