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
#include "Utils/TimeStone.h"

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
	PPA::AudioEngine::Instance().ResumeAllChannelGroups();
	PPA::AudioEngine::StopPlayingAll();

	m_activeECS = &m_playECS;

	PPD::ImguiHelper::RelinkECS(&m_playECS);

	Serializer serialiser{ m_playECS };
	if (!serialiser.Load(sceneToLoad))
		assert(false);
	
	Scripting::GameScript::Init();
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
	if (m_playState == PLAYSTATE::PLAY || (m_playState == PLAYSTATE::STEPNEXT))
	{
		c_dt = TimeStone::GetUpdatedDelta(c_dt);
		float timeMulti = TimeStone::GetCurrentTimeScale();
		//m_sPhysicsSystem.UpdateEditor();

		//Physics dynamic update until fps drops below 30fps
		m_accumulatedFixedTime += c_dt;

		if (m_accumulatedFixedTime < m_minFixedUpdateTime * timeMulti && m_accumulatedFixedTime > m_maxFixedUpdateTime * timeMulti)
		{
			m_sPhysicsSystem.Update(m_accumulatedFixedTime);

			m_sScriptSystem.Update(m_accumulatedFixedTime);
			m_sScriptSystem.LateUpdate(m_accumulatedFixedTime);
			m_sGeneralSystem.UpdateGame(m_accumulatedFixedTime);

			PPI::InputSystem::PollEvents(m_accumulatedFixedTime);

			m_accumulatedFixedTime = 0.f;
		}
		else if(m_accumulatedFixedTime > m_maxFixedUpdateTime * timeMulti)
		{
			float timeToUpdate = m_minFixedUpdateTime * timeMulti;

			while (m_accumulatedFixedTime > m_minFixedUpdateTime)
			{
				
				m_sPhysicsSystem.Update(timeToUpdate);

				m_sScriptSystem.Update(timeToUpdate);
				m_sScriptSystem.LateUpdate(timeToUpdate);
				m_sGeneralSystem.UpdateGame(timeToUpdate);

				PPI::InputSystem::PollEvents(timeToUpdate);

				m_accumulatedFixedTime -= m_minFixedUpdateTime;
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
	else if (m_playState == PLAYSTATE::PAUSE)
	{
		m_sScriptSystem.PauseUpdate(c_dt);
		m_sGeneralSystem.Update(c_dt);
		PPI::InputSystem::PollEvents(c_dt);

		UpdateTransforms(0.f);
	}
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

	PP::DebugDraw::m_DebugVertsConst.clear();
	PPA::AudioEngine::StopPlayingAll();
	m_sScriptSystem.Unload();
	m_playECS.Clear();
	m_playECS.m_prefab_map.clear();
}