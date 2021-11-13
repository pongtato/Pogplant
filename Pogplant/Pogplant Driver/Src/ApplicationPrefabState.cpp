/******************************************************************************/
/*!
\file	ApplicationPrefabState.cpp
\details

	This file contains the prefab editing state behaviour

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
	The function called when the engine enters prefab editor state
*/
/******************************************************************************/
void Application::EnterPrefabState()
{
	//std::cout << "Entering Editor State" << std::endl;
	m_activeECS = &m_playECS;

	Serializer serialiser{ m_playECS };
	if (!serialiser.Load(m_prefabFilePath))
		assert(false);

	PPD::ImguiHelper::RelinkECS(&m_playECS);
	m_sGeneralSystem.Init(&m_playECS);
	m_sGeneralSystem.UpdateTransforms();

	m_sPhysicsSystem.Init(&m_playECS, m_eventBus);
	m_sScriptSystem.InitEditor(&m_playECS);
}

/******************************************************************************/
/*!
\brief
	Updates editor state
*/
/******************************************************************************/
void Application::UpdatePrefabState(float c_dt)
{
	m_sPhysicsSystem.UpdateEditor();

	m_sGeneralSystem.Update(c_dt);

	PPF::FileHandler& fh = fh.GetInstance();
	if (fh.m_Modified)
	{
		UpdateModelRef(fh.m_UpdatedName);
		fh.m_Modified = false;
		fh.m_ShouldUpdate = true;
	}
	fh.UpdateModels();

	//Update the transform before drawing
	UpdateTransforms(c_dt);
}

/******************************************************************************/
/*!
\brief
	Renders prefab state
*/
/******************************************************************************/
void Application::RenderPrefabState()
{
	// Debug draw colliders
	m_sPhysicsSystem.DrawColliders();

	// Things that appear in both editor & game
	DrawCommon();
	// Editor
	DrawEditor();
	// Game
	DrawGame();
	// ImGUI
	DrawImGUI();
}

/******************************************************************************/
/*!
\brief
	The function called when the engine leaves prefab state
*/
/******************************************************************************/
void Application::LeavePrefabState()
{
	//adding guid to existing prefab files
	auto view =	m_playECS.view<Transform>();
	for (auto& ent : view)
	{
		auto& transform = view.get<Transform>(ent);
		//add GUID to parent only
		if (transform.m_parent == entt::null)
			m_playECS.GetReg().emplace<Components::Guid>(ent, m_playECS.GenerateGUID());
	}

	Serializer serialiser{ m_playECS };
	serialiser.Save(m_prefabFilePath);

	//update all existing prefab using this GUID




	PPA::AudioEngine::StopPlayingAll();
	m_sScriptSystem.Unload();
	m_activeECS->GetReg().clear();

}

/******************************************************************************/
/*!
\brief
	Function to trigger application to edit a particular prefab
\param filePath
	The file path to the prefab
*/
/******************************************************************************/
void Application::StartPrefabEditing(const std::string& filePath)
{
	if (m_appState == Application::APPLICATIONSTATE::PREFAB_EDITOR)
	{
		LeavePrefabState();
		m_prefabFilePath = filePath;
		EnterPrefabState();
	}
	else
	{
		m_prefabFilePath = filePath;
		TransitionApplicationState(Application::APPLICATIONSTATE::PREFAB_EDITOR);
	}
}

/******************************************************************************/
/*!
\brief
	Function to trigger application to exit the prefab editor
*/
/******************************************************************************/
void Application::ExitPrefabEditing()
{
	TransitionApplicationState(Application::APPLICATIONSTATE::EDITOR);
}