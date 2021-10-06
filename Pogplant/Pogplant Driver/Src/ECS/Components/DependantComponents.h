/******************************************************************************/
/*!
\file	DependantComponents.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	
	This file contains the components that may require another library
	or include. It's separated from the main GenericComponents.h to
	prevent the Pogplant graphics library requiring to include
	FMOD, PhysicsDLC etc, due to a slight design oversight.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef DEPENDANTCOMPONENTS_H_
#define DEPENDANTCOMPONENTS_H_

#include <PhysicsDLC.h>
#include "../../AudioEngine.h"
#include <rttr/registration>

namespace Components
{
	using Rigidbody = PhysicsDLC::Physics::Rigidbody;

	struct Collider
	{
		enum COLLISION_RULE
		{
			CR_COLLIDE = 0,/**<Just perform collision response*/
			CR_EVENT,/**<Just send an event*/
			CR_COLLIDE_EVENT,/**<Perform both response and sending event*/
			CR_IGNORE/**<Just ignore the collision*/
		};

		enum COLLIDER_TYPE
		{
			CT_BOX,
			CT_SPHERE,
			CT_GJK
		};

		bool isTrigger = false;

		int collisionLayer = 0;
		int colliderType = 0;
	};

	struct BoxCollider : public Collider
	{
		BoxCollider() = default;
		inline BoxCollider(glm::vec3 c_extends, glm::vec3 c_centre)
			: extends{ c_extends }, centre{ c_centre }
		{}

		glm::vec3 extends;
		glm::vec3 centre;

		PhysicsDLC::Collision::Shapes::AABB aabb;
	};

	struct SphereCollider : public Collider
	{
		SphereCollider() = default;
		inline SphereCollider(glm::vec3 c_centre, float c_radius)
			: centre{ c_centre }, radius{ c_radius }
		{}

		float radius;
		glm::vec3 centre;

		PhysicsDLC::Collision::Shapes::Sphere sphere;
	};

	struct AudioSource
	{
		struct AudioClip
		{
			/**> Relative file directory of the audio file*/
			std::string m_fileDir;
			float m_volume = 1.f;
			bool m_is3D = true;
			bool m_isLooping = false;
			bool m_isStreamed = false;

			bool m_enableDopplerEffect = false;

			bool m_update3DPosition = true;

			/**> Runtime variables, do not serialise*/
			int c_channelID;
			bool c_playing;
		};

		std::vector<AudioClip> m_audioSources;

		inline void PlayAudio(size_t id, const glm::vec3& pos)
		{
			if (m_audioSources.size() > id)
			{
				m_audioSources[id].c_channelID = PPA::AudioEngine::PlaySound(
					m_audioSources[id].m_fileDir,
					m_audioSources[id].m_volume,
					pos);

				m_audioSources[id].c_playing = true;
			}
		}

		inline void StopAudio(size_t id)
		{

		}
	};
}

#endif // !PHYSICSCOMPONENTS_H_
