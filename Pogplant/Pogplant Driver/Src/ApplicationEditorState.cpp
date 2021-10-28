/******************************************************************************/
/*!
\file	ApplicationPlayState.cpp
\details

	This file contains the editor state behaviour

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "Application.h"
#include "Serialiser/Serializer.h"

namespace PPD = PogplantDriver;
using namespace Components;
using namespace PogplantDriver;

/******************************************************************************/
/*!
\brief
	The function called when the engine enters editor state
*/
/******************************************************************************/
void Application::EnterEditorState()
{
	//std::cout << "Entering Editor State" << std::endl;
	m_activeECS = &m_editorECS;
	PPD::ImguiHelper::RelinkECS(&m_editorECS);
	m_sGeneralSystem.Init(&m_editorECS);
	m_sPhysicsSystem.Init(&m_editorECS, m_eventBus);
	m_sScriptSystem.Init(&m_editorECS);
}

/******************************************************************************/
/*!
\brief
	Updates editor state
*/
/******************************************************************************/
void Application::UpdateEditorState(float c_dt)
{
#ifdef PPD_UPDATE_EDITOR_AS_GAME
	m_sPhysicsSystem.UpdateEditor();

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
	m_sScriptSystem.Update(c_dt);
	m_sScriptSystem.LateUpdate();
#else

	m_sPhysicsSystem.UpdateEditor();

#endif // PPD_UPDATE_EDITOR_AS_GAME


	m_sGeneralSystem.Update(c_dt);
	PPF::FileHandler& fh = fh.GetInstance();
	fh.UpdateModels();

	/// Most of this should be moved to other files when the engine is developed
	// Update the transform before drawing
	UpdateTransforms(ImGui::GetIO().DeltaTime);
}

/******************************************************************************/
/*!
\brief
	Renders editor state
*/
/******************************************************************************/
void Application::RenderEditorState()
{
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
}

/******************************************************************************/
/*!
\brief
	The function called when the engine leaves editor state
*/
/******************************************************************************/
void Application::LeaveEditorState()
{
	//std::cout << "Leaving Editor State" << std::endl;

	PPA::AudioEngine::StopPlayingAll();

	Serializer serialiser{ m_editorECS };
	serialiser.Save("Resources/tmp");
}