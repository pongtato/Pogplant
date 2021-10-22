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
	/**> Use PhysicsDLC as rigidbody*/
	using Rigidbody = PhysicsDLC::Physics::Rigidbody;

	/**************************************************************************/
	/*!
	\brief
		A component automatically added on runtime to identify the collider type
		used for that particular object

		Notes: Do not serialise or make this object editable, it's for runtime
		use only
	*/
	/**************************************************************************/
	struct ColliderIdentifier
	{
		enum class COLLIDER_TYPE
		{
			CT_UNDEFINED,
			CT_BOX,
			CT_SPHERE,
			CT_HEIGHTMAP
		};

		COLLIDER_TYPE colliderType = COLLIDER_TYPE::CT_UNDEFINED;
		bool isTrigger = false;
		int collisionLayer = 0;
	};

	/**************************************************************************/
	/*!
	\brief
		Base collider class
	*/
	/**************************************************************************/
	struct Collider
	{
		enum COLLISION_RULE
		{
			CR_COLLIDE = 0,/**<Just perform collision response*/
			CR_EVENT,/**<Just send an event*/
			CR_COLLIDE_EVENT,/**<Perform both response and sending event*/
			CR_IGNORE/**<Just ignore the collision*/
		};

		bool isTrigger = false;
		int collisionLayer = 0;
	};

	struct BoxCollider : public Collider
	{
		BoxCollider() = default;
		inline BoxCollider(glm::vec3 c_extends, glm::vec3 c_centre)
			: extends{ c_extends }, centre{ c_centre }
		{}

		glm::vec3 extends = glm::vec3{ 1.f, 1.f, 1.f };
		glm::vec3 centre;

		PhysicsDLC::Collision::Shapes::AABB aabb;
	};

	struct SphereCollider : public Collider
	{
		SphereCollider() = default;
		inline SphereCollider(glm::vec3 c_centre, float c_radius)
			: centre{ c_centre }, radius{ c_radius }
		{}

		float radius = 1.f;
		glm::vec3 centre;

		PhysicsDLC::Collision::Shapes::Sphere sphere;
	};

	/*struct HeightMapCollider : public Collider
	{
		HeightMapCollider() = default;

		PhysicsDLC::Collision::Shapes::HeightMap heightMap;
	};//*/

	struct AudioSource
	{
		struct AudioClip
		{
			inline AudioClip(const std::string& fileDir,
				float volume = 1.f,
				bool is3D = true,
				bool isLooping = false,
				bool isStreamed = false,
				bool enableDoppler = false,
				bool followTransformPosition = true)
				:
				m_fileDir{ fileDir },
				m_volume{ volume },
				m_is3D{ is3D },
				m_isLooping{ isLooping },
				m_isStreamed{ isStreamed },
				m_enableDopplerEffect{ enableDoppler },
				m_update3DPosition{ followTransformPosition }
			{

			}

			~AudioClip() = default;

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

		inline bool LoadAudioToFMOD(size_t id)
		{
			if (m_audioSources.size() > id)
			{
				return PPA::AudioEngine::LoadAudio(
					m_audioSources[id].m_fileDir,
					m_audioSources[id].m_is3D,
					m_audioSources[id].m_isLooping,
					m_audioSources[id].m_isStreamed);
			}

			return false;
		}

		inline void UpdateAudioSettings(size_t id)
		{
			if (m_audioSources.size() > id)
			{
				PPA::AudioEngine::StopPlayingChannel(m_audioSources[id].c_channelID);
				m_audioSources[id].c_playing = false;

				PPA::AudioEngine::UpdateAudio(
					m_audioSources[id].m_fileDir,
					m_audioSources[id].m_is3D,
					m_audioSources[id].m_isLooping,
					m_audioSources[id].m_isStreamed);
			}
		}

		inline void PlayAudio(size_t id, const glm::vec3& pos = PhysicsDLC::Vector::Zero)
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
			if (m_audioSources.size() > id && m_audioSources[id].c_playing)
			{
				PPA::AudioEngine::StopPlayingChannel(m_audioSources[id].c_channelID);
				m_audioSources[id].c_playing = false;
			}
		}

		//Don't actually need to use it as the audio engine
		//will auto clear up if anything is not freed
		inline void UnloadResources()
		{
			for (size_t i = 0; i < m_audioSources.size(); i++)
			{
				if (m_audioSources[i].c_playing)
					PPA::AudioEngine::StopPlayingChannel(m_audioSources[i].c_channelID);

				PPA::AudioEngine::UnloadAudio(m_audioSources[i].m_fileDir);
			}
		}
	};
}

#endif // !PHYSICSCOMPONENTS_H_
