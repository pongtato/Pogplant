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
#include "Serialiser/Serializer.h"
#include "GameScript.h"

namespace PPD = PogplantDriver;
using namespace Components;
using namespace PogplantDriver;

/******************************************************************************/
/*!
\brief
	The function called when the engine enters play state
*/
/******************************************************************************/
void Application::EnterPlayState(const std::string& sceneToLoad)
{
	m_activeECS = &m_playECS;

	PPD::ImguiHelper::RelinkECS(&m_playECS);

	Serializer serialiser{ m_playECS };
	if (!serialiser.Load(sceneToLoad))
		assert(false);

	m_sGeneralSystem.Init(&m_playECS);
	m_sGeneralSystem.UpdateTransforms();

	m_sPhysicsSystem.Init(&m_playECS, m_eventBus);

	// Ensure that the scripts are reloaded.
	m_sScriptSystem.SetReload(true);
	m_sScriptSystem.InitPlayState(&m_playECS);

	//This is where code is when transitioning to play state
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
#ifdef PPD_EDITOR_BUILD

	if (m_playState == PLAYSTATE::PLAY || (m_playState == PLAYSTATE::STEPNEXT))
#endif // PPD_EDITOR_BUILD
	{
		//m_sPhysicsSystem.UpdateEditor();

		//Physics dynamic update until fps drops below 30fps
		m_accumulatedFixedTime += c_dt;

		if (m_accumulatedFixedTime < m_minFixedUpdateTime && m_accumulatedFixedTime > m_maxFixedUpdateTime)
		{
			m_sPhysicsSystem.Update(m_accumulatedFixedTime);

			m_sScriptSystem.Update(m_accumulatedFixedTime);
			m_sScriptSystem.LateUpdate(m_accumulatedFixedTime);
			m_sGeneralSystem.UpdateGame(m_accumulatedFixedTime);
			m_accumulatedFixedTime = 0.f;

			PPI::InputSystem::PollEvents();
		}
		else
		{
			while (m_accumulatedFixedTime > m_minFixedUpdateTime)
			{
				m_sPhysicsSystem.Update(m_minFixedUpdateTime);

				m_sScriptSystem.Update(m_minFixedUpdateTime);
				m_sScriptSystem.LateUpdate(m_minFixedUpdateTime);
				m_sGeneralSystem.UpdateGame(m_minFixedUpdateTime);
				m_accumulatedFixedTime -= m_minFixedUpdateTime;

				PPI::InputSystem::PollEvents();
			}
		}

		m_sGeneralSystem.Update(c_dt);
		//m_sScriptSystem.Update(c_dt);
		//m_sScriptSystem.LateUpdate(c_dt);
		PPF::FileHandler& fh = fh.GetInstance();
		if (fh.m_Modified)
		{
			UpdateModelRef(fh.m_UpdatedName);
			fh.m_Modified = false;
			fh.m_ShouldUpdate = true;
		}
		fh.UpdateModels();

		/// Most of this should be moved to other files when the engine is developed
		// Update the transform before drawing
		UpdateTransforms(c_dt);

#ifdef PPD_EDITOR_BUILD
		if (m_playState == PLAYSTATE::STEPNEXT)
			m_playState = PLAYSTATE::PAUSE;
#endif // PPD_EDITOR_BUILD
	}

#ifdef PPD_EDITOR_BUILD
	if (m_playState == PLAYSTATE::PAUSE)
		UpdateTransforms(0.f);
#endif
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
	// Debug draw colliders
	m_sPhysicsSystem.DrawColliders();

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
	
	DrawCommon();
	DrawGame();
	DrawScreen();

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
	m_sPhysicsSystem.Clear();

	PPA::AudioEngine::StopPlayingAll();
	m_sScriptSystem.Unload();
	m_playECS.Clear();
	m_playECS.m_prefab_map.clear();
}