/******************************************************************************/
/*!
\file	GeneralSystem.cpp
\author
\par	email:
\details

	This file contains implementations for a system handling generic components
	that are basic enough that do not require a whole file or class
	delicated to it

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "GeneralSystem.h"
#include "../Entity.h"

#include "../Components/DependantComponents.h"
#include "../../Input/GLFWInput.h"
#include "../../AudioEngine.h"

using namespace Components;

GeneralSystem::GeneralSystem()
{
}

GeneralSystem::~GeneralSystem()
{
}

void GeneralSystem::Init(ECS* ecs)
{
	m_registry = ecs;

	auto audios = m_registry->GetReg().view<Components::AudioSource>();

	for (auto& audioEntity : audios)
	{
		auto& audioSource = audios.get<Components::AudioSource>(audioEntity);

		for (size_t i = 0; i < audioSource.m_audioSources.size(); i++)
		{
			PPA::AudioEngine::LoadAudio(
				audioSource.m_audioSources[i].m_fileDir,
				audioSource.m_audioSources[i].m_is3D,
				audioSource.m_audioSources[i].m_isLooping,
				audioSource.m_audioSources[i].m_isStreamed);
		}
	}
}

void GeneralSystem::Update()
{
	/*//entities will be a container of objects with it
	//get all entities with the imaginary_object component
	auto entities = m_registry->GetReg().view<Imaginary_object, Transform, Tag>();

	//get all entities with more than 1 component
	//auto entities = m_registry->GetReg().view<Imaginary_object, Transform, Tag>();

	//exclude entities with certain component
	//auto entities = m_registry->GetReg().view<Transform>(entt::exclude<Imaginary_object>);


	for (auto entity : entities)
	{
		//do stuffs
		//get components
		auto& imaginary_component = entities.get<Imaginary_object>(entity);

		(void)imaginary_component;
	}//*/

	auto audios = m_registry->GetReg().view<Components::Transform, Components::AudioSource>();

	for (auto& audioEntity : audios)
	{
		auto& audioSource = audios.get<Components::AudioSource>(audioEntity);
		auto& transform = audios.get<Components::Transform>(audioEntity);

		for (size_t i = 0; i < audioSource.m_audioSources.size(); i++)
		{
			if (audioSource.m_audioSources[i].m_update3DPosition && audioSource.m_audioSources[i].c_playing)
				audioSource.m_audioSources[i].c_playing = PPA::AudioEngine::UpdateChannel3DPosition(audioSource.m_audioSources[i].c_channelID, transform.m_position);
		}
	}

	auto characters = m_registry->GetReg().view<Transform, Rigidbody, Components::CharacterController>();

	for (auto entity : characters)
	{
		auto& character = characters.get<Components::CharacterController>(entity);
		auto& rigidBody = characters.get<Components::Rigidbody>(entity);

		if (PPI::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_UP))
		{
			rigidBody.AddForce({ 0.f, 0.f, -character.force });
		}

		if (PPI::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_DOWN))
		{
			rigidBody.AddForce({ 0.f, 0.f, character.force });
		}

		if (PPI::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_LEFT))
		{
			rigidBody.AddForce({ -character.force, 0.f, 0.f });
		}

		if (PPI::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_RIGHT))
		{
			rigidBody.AddForce({ character.force, 0.f, 0.f });
		}

		if (PPI::GLFWInputManager::Instance()->onKeyTriggered(GLFW_KEY_SPACE))
		{
			rigidBody.AddImpulseForce({ 0.f, 10.f, 0.f });
		}

		if (PPI::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_PAGE_UP))
			rigidBody.mass = 3.f;

		if (PPI::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_PAGE_DOWN))
			rigidBody.mass = 0.5f;
	}
}