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

#include "../Components/GameplayComponents.h"
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
}

void GeneralSystem::Update(float c_dt)
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
	auto projectiles = m_registry->GetReg().view<Components::Transform, Components::Projectile,Components::SphereCollider, Components::Rigidbody>();
	for (auto& projectileEntity : projectiles)
	{
		auto& projectile = projectiles.get<Components::Projectile>(projectileEntity);
		auto& transform = projectiles.get<Components::Transform>(projectileEntity);
		auto& rigidbody = projectiles.get<Components::Rigidbody>(projectileEntity);

		glm::vec3 move = { 0.f,0.f,projectile.m_Speed * c_dt };
		transform.m_position += move;
		projectile.m_CurentLifetime += c_dt;

		if (projectile.m_CurentLifetime > projectile.m_Lifetime)
			m_registry->DestroyEntity(projectileEntity);
	}



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
}