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
	if (!serialiser.Load(m_genericFilePath, true))
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
	entt::entity _id = entt::null;
	auto view = m_playECS.view<Transform>();
	for (auto& ent : view)
	{
		if (view.get<Transform>(ent).m_parent == entt::null)
		{
			_id = ent;
			m_playECS.GetReg().emplace_or_replace<Components::Prefab>(_id, m_genericFilePath);
			break;
		}
	}

	assert(_id != entt::null);

	Serializer serialiser{ m_playECS };
	serialiser.SavePrefab(m_genericFilePath, _id);

	//update all existing prefab using this GUID
	if (m_editorECS.m_prefab_map.contains(m_genericFilePath))
	{
		//delete current
		m_editorECS.DestroyEntity(m_editorECS.m_prefab_map[m_genericFilePath]);
		//load the new file
		
		m_editorECS.m_prefab_map.erase(m_genericFilePath);
		Serializer serialiser_2{ m_editorECS };
		serialiser_2.LoadPrefab(m_genericFilePath, true);

		assert(m_editorECS.m_prefab_map.contains(m_genericFilePath));
		entt::entity e_id = m_editorECS.m_prefab_map[m_genericFilePath];

		auto _view = m_editorECS.view<PrefabInstance>();

		for (auto ent : _view)
		{
			auto& prefab = m_editorECS.GetReg().get<Guid>(e_id);
			auto& prefab_instance = _view.get<PrefabInstance>(ent);

			if (prefab.m_guid == prefab_instance.prefab_GUID)
			{
				m_editorECS.DestroyEntity(ent);
				auto _ent = m_editorECS.CopyEntity(m_editorECS.m_prefab_map[m_genericFilePath]);
				m_editorECS.GetReg().emplace_or_replace<PrefabInstance>(_ent, prefab.m_guid, m_genericFilePath);
			}
		}
	}

	PPA::AudioEngine::StopPlayingAll();
	m_sScriptSystem.Unload();

	m_activeECS->Clear();

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
		m_genericFilePath = filePath;
		EnterPrefabState();
	}
	else
	{
		m_genericFilePath = filePath;
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