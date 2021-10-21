/******************************************************************************/
/*!
\file	ApplicationPlayState.cpp
\details
	
	This file contains the play state behaviour, as well as the behaviour
	when the game is set to build as a game instead of an editor.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "Application.h"

namespace PPD = PogplantDriver;
using namespace Components;
using namespace PogplantDriver;

/******************************************************************************/
/*!
\brief
	The function called when the engine enters play state
*/
/******************************************************************************/
void Application::EnterPlayState()
{
	std::cout << "Entering Play State" << std::endl;

	//This is where it'll be code when transitioning to play state
	m_sPhysicsSystem.InitPlayState();
}

/******************************************************************************/
/*!
\brief
	Updates play state
*/
/******************************************************************************/
void Application::UpdatePlayState(float c_dt)
{
	//Physics dynamic update until fps drops below 30fps
	m_accumulatedFixedTime += c_dt;

	if (m_accumulatedFixedTime < m_minFixedUpdateTime)
	{
		m_sPhysicsSystem.Update(m_accumulatedFixedTime);
		m_accumulatedFixedTime = 0.f;
	}
	else
	{
		while (m_accumulatedFixedTime > m_minFixedUpdateTime)
		{
			m_sPhysicsSystem.Update(m_minFixedUpdateTime);
			m_accumulatedFixedTime -= m_minFixedUpdateTime;
		}
	}

	m_sGeneralSystem.Update();
	m_sScriptSystem.Update();
	PPF::FileHandler& fh = fh.GetInstance();
	fh.UpdateModels();

	/// Most of this should be moved to other files when the engine is developed
	// Update the transform before drawing
	UpdateTransforms(ImGui::GetIO().DeltaTime);
}

/******************************************************************************/
/*!
\brief
	Renders play state
*/
/******************************************************************************/
void Application::RenderPlayState()
{
#ifdef PPD_EDITOR_BUILD
	// Things that appear in both editor & game
	DrawCommon();
	// Editor
	DrawEditor();
	// Game
	DrawGame();
	// Post process - Only used for run time, currently drawing to imgui
	//DrawScreen();
	// ImGUI
	DrawImGUI();
	///

#else
	//To do
	DrawGame() // only this?
#endif // PPD_EDITOR_BUILD
}

/******************************************************************************/
/*!
\brief
	The function called when the engine leaves play state
*/
/******************************************************************************/
void Application::LeavePlayState()
{
	std::cout << "Leaving Play State" << std::endl;
}