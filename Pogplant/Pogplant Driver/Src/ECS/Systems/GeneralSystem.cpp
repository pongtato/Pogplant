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
#include "../Entity.h"

#include "../Components/GameplayComponents.h"
#include "../Components/DependantComponents.h"
#include "../Systems/ScriptSystemHelper.h"
#include "../../GameScript.h"
#include "../../Input/GLFWInput.h"
#include "../../AudioEngine.h"

#include "GeneralSystem.h"

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

void GeneralSystem::UpdateGame(float c_dt)
{
	auto projectiles = m_registry->view<Components::Transform, Components::Projectile, Components::Rigidbody>();
	for (auto& projectileEntity : projectiles)
	{
		auto& projectile = projectiles.get<Components::Projectile>(projectileEntity);
		auto& transform = projectiles.get<Components::Transform>(projectileEntity);
		auto& rigidbody = projectiles.get<Components::Rigidbody>(projectileEntity);

		//transform.m_position += move;
		//glm::vec3 move = { 0.f,0.f,projectile.m_Speed * c_dt };
		
		if(projectile.m_Homing)
		{
			auto enemy_trans = m_registry->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(projectile.tracker));
			if(enemy_trans)
			{
				auto mag = glm::length(rigidbody.velocity);
				transform.LookAt(enemy_trans->GetGlobalPosition());
				auto forward_vector = transform.GetForwardVector();
				rigidbody.velocity = forward_vector * mag;

			}
		}

		projectile.m_CurentLifetime += c_dt;

		if (projectile.m_CurentLifetime > projectile.m_Lifetime)
			m_registry->DestroyEntity(projectileEntity);
	}

	auto lasers = m_registry->view<Components::Transform, Components::Laser>();
	for (auto& laserEntity : lasers)
	{
		auto& laser = lasers.get<Components::Laser>(laserEntity);
		//All lasers will be activated 
		if (laser.m_ActivateLaser)
		{

			//if (!laser.m_Onceflag)
			//{
			//	m_registry->EnableEntity(laserEntity);
			//	laser.m_Onceflag = true;
			//	std::cout << "Activate  LASSER" << std::endl;
			//}

			laser.m_AccumulatedTime += c_dt;
			if (laser.m_Activetime != 0 && laser.m_Inactivetime != 0)
			{
				laser.m_AccumulatedActivetime += c_dt;
				if (laser.m_AccumulatedActivetime >= laser.m_Activetime)
				{
					laser.m_AccumulatedActivetime = 0;
					laser.m_IsDeactivated = true;
					m_registry->DisableEntity(laserEntity);
				}
				if (laser.m_AccumulatedActivetime >= laser.m_Inactivetime)
				{
					laser.m_AccumulatedActivetime = 0;
					laser.m_IsDeactivated = false;
					m_registry->EnableEntity(laserEntity);
				}
			}

			//Final case for spawn time
			if (laser.m_AccumulatedTime > laser.m_Spawntime)
			{
				laser.m_ActivateLaser = false;
				laser.m_AccumulatedTime = 0;
				laser.m_LaserCompleted = true;
				if (!laser.m_IsDeactivated)
				{
					m_registry->DisableEntity(laserEntity);
				}

			}
		}

	}

	// Get all entities with particle systems
	auto particleSystemsView = m_registry->view<Components::Transform, Components::Tag, Components::ParticleSystem>();
	for (auto& particleSystem : particleSystemsView)
	{
		// get the parent entity of the particle system
		auto& ps_root = particleSystemsView.get<Components::Transform>(particleSystem).m_parent;

		// if parent exist and is valid entity
		if (ps_root != entt::null && m_registry->GetReg().valid(ps_root))
		{
			// check if the parent is tagged Particle
			if (m_registry->GetReg().get<Components::Tag>(ps_root).m_tag == "Particle")
			{
				auto& ps = particleSystemsView.get<Components::ParticleSystem>(particleSystem);

				// check life time of the particle system if more than max life then delete
				if (ps.m_CurrentLifetime >= ps.m_MaxLife)
					m_registry->DestroyEntity(ps_root);
			}
			else
			{
				if (particleSystemsView.get<Components::Tag>(particleSystem).m_tag == "Particle")
				{
					auto& ps = particleSystemsView.get<Components::ParticleSystem>(particleSystem);

					// check life time of the particle system if more than max life then delete
					if (ps.m_CurrentLifetime >= ps.m_MaxLife)
						m_registry->DestroyEntity(particleSystem);
				}
			}
		}
		else
		{
			// check if the this object is tagged Particle
			if (particleSystemsView.get<Components::Tag>(particleSystem).m_tag == "Particle")
			{
				auto& ps = particleSystemsView.get<Components::ParticleSystem>(particleSystem);

				// check life time of the particle system if more than max life then delete
				if (ps.m_CurrentLifetime >= ps.m_MaxLife)
					m_registry->DestroyEntity(particleSystem);
			}
		}
	}
}

void GeneralSystem::Update(float c_dt)
{
	/*//entities will be a container of objects with it
	//get all entities with the imaginary_object component
	auto entities = m_registry->view<Imaginary_object, Transform, Tag>();

	//get all entities with more than 1 component
	//auto entities = m_registry->view<Imaginary_object, Transform, Tag>();

	//exclude entities with certain component
	//auto entities = m_registry->view<Transform>(entt::exclude<Imaginary_object>);


	for (auto entity : entities)
	{
		//do stuffs
		//get components
		auto& imaginary_component = entities.get<Imaginary_object>(entity);

		(void)imaginary_component;
	}//*/
	
	auto audios = m_registry->view<Components::Transform, Components::AudioSource>();

	for (auto& audioEntity : audios)
	{
		auto& audioSource = audios.get<Components::AudioSource>(audioEntity);
		auto& transform = audios.get<Components::Transform>(audioEntity);

		for (size_t i = 0; i < audioSource.m_audioSources.size(); i++)
		{
			if (audioSource.m_audioSources[i].m_update3DPosition && audioSource.m_audioSources[i].c_playing)
				audioSource.m_audioSources[i].c_playing = PPA::AudioEngine::UpdateChannel3DPosition(audioSource.m_audioSources[i].c_channelID, transform.GetGlobalPosition());
		}
	}
}

void GeneralSystem::UpdateTransforms()
{
	auto view = m_registry->view<Transform>();
	for (auto entity : view)
	{
		auto& transform = view.get<Transform>(entity);

		if (transform.m_parent == entt::null)
		{
			transform.updateModelMtx();

			for (auto& ent : transform.m_children)
			{
				transform.updateModelMtx();
				UpdateTransform(ent, transform);
			}
		}
	}
}

//Recursive update transform function
void GeneralSystem::UpdateTransform(entt::entity _id, Components::Transform& parent_transform)
{
	//update myself
	auto& transform = m_registry->GetReg().get<Transform>(_id);

	transform.updateModelMtx();
	transform.updateModelMtx(parent_transform);

	//update children
	if (!transform.m_children.empty())
	{
		for (auto& entity : transform.m_children)
			UpdateTransform(entity, transform);
	}
}

//Used to delete entities, should be the last system in the update loop
void GeneralSystem::DeleteEntities()
{
	const auto& set_of_entities = m_registry->m_EntitiesToDelete;

	for (const auto& ent : set_of_entities)
	{
		m_registry->TrulyDestroyEntity(ent);
	}

	m_registry->m_EntitiesToDelete.clear();
}

//Used to disable entities, should be the last system in the update loop
void GeneralSystem::DisableEntities()
{
	const auto& set_of_entities = m_registry->m_EntitiesToDisable;

	for (const auto& ent : set_of_entities)
	{
		m_registry->TrulyDisableEntity(ent);
	}

	m_registry->m_EntitiesToDisable.clear();
}

//Used to disable entities, should be the last system in the update loop
void GeneralSystem::EnableEntities()
{
	const auto& set_of_entities = m_registry->m_EntitiesToEnable;

	for (const auto& ent : set_of_entities)
	{
		m_registry->TrulyEnableEntity(ent);
	}

	m_registry->m_EntitiesToEnable.clear();
}