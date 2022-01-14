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
			CT_GJKMESH,
			CT_OBBBOX
		};

		COLLIDER_TYPE colliderType = COLLIDER_TYPE::CT_UNDEFINED;
		bool isTrigger = false;
		int collisionLayer = 0;

		void* broadPhaseKey = nullptr;
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

		std::string collisionLayer = "DEFAULT";
		PhysicsDLC::Collision::Shapes::AABB aabb;

		bool isStatic = false;
		bool isTrigger = false;

		RTTR_ENABLE();
	};

	struct BoxCollider : public Collider, public PhysicsDLC::Collision::GJK::GJKSupportShape
	{
		BoxCollider() = default;
		inline BoxCollider(glm::vec3 c_extends, glm::vec3 c_centre)
			: extends{ c_extends }, centre{ c_centre }
		{}

		glm::vec3 extends = glm::vec3{ 0.5f, 0.5f, 0.5f };
		glm::vec3 centre;

		virtual glm::vec3 GetCenter() const override;
		virtual glm::vec3 FindFurthestPoint(const glm::vec3& direction) const override;

		RTTR_ENABLE(Collider);
	};

	struct SphereCollider : public Collider, public PhysicsDLC::Collision::GJK::GJKSupportShape
	{
		SphereCollider() = default;
		inline SphereCollider(glm::vec3 c_centre, float c_radius)
			: centre{ c_centre }, radius{ c_radius }
		{}

		PhysicsDLC::Collision::Shapes::Sphere sphere;
		glm::vec3 centre;
		float radius = 1.f;
		
		virtual glm::vec3 GetCenter() const override;
		virtual glm::vec3 FindFurthestPoint(const glm::vec3& direction) const override;

		RTTR_ENABLE(Collider);
	};

	//Todo
	struct OBBBoxCollider : public Collider, public PhysicsDLC::Collision::GJK::GJKSupportShape
	{
		OBBBoxCollider() = default;
		inline OBBBoxCollider(glm::vec3 c_extends, glm::vec3 c_centre)
			: extends{ c_extends }, centre{ c_centre }
		{}

		entt::entity m_id;

		glm::vec3 extends = glm::vec3{ 1.f, 1.f, 1.f };
		glm::vec3 centre;

		static const glm::vec3 m_OBBverts[8];

		virtual glm::vec3 GetCenter() const override;
		virtual glm::vec3 FindFurthestPoint(const glm::vec3& direction) const override;

		RTTR_ENABLE(Collider);
	};

	struct MeshCollider : public Collider, public PhysicsDLC::Collision::GJK::GJKSupportShape
	{
		MeshCollider() = default;

		entt::entity m_id;

		virtual glm::vec3 GetCenter() const override;
		virtual glm::vec3 FindFurthestPoint(const glm::vec3& direction) const override;
		RTTR_ENABLE(Collider);
	};

	struct AudioSource
	{
		struct AudioObject
		{
			AudioObject() = default;

			inline AudioObject(const std::string& fileDir,
				float volume = 1.f,
				bool followTransformPosition = true)
				:
				m_fileDir{ fileDir },
				m_volume{ volume },
				m_update3DPosition{ followTransformPosition }
			{
				Init();
			}

			~AudioObject() = default;

			inline bool Init()
			{
				auto itr = PPA::AudioResource::AudioPool().find(m_fileDir);

				if (itr != PPA::AudioResource::AudioPool().end())
				{
					m_audioClip = &itr->second;
					return true;
				}

				return false;
			}


			/**> Relative file directory of the audio file*/
			std::string m_fileDir;
			float m_volume = 1.f;

			bool m_update3DPosition = true;

			/**> Runtime variables, do not serialise*/
			PPA::AudioResource::AudioClip* m_audioClip;
			int c_channelID;
			bool c_playing;
		};

		std::vector<AudioObject> m_audioSources;

		inline void UpdateAudioSettings(size_t id)
		{
			if (m_audioSources.size() > id)
			{
				if (!m_audioSources[id].m_audioClip)
					return;

				PPA::AudioEngine::StopPlayingChannel(m_audioSources[id].c_channelID);
				m_audioSources[id].c_playing = false;

				PPA::AudioEngine::UpdateAudio(
					m_audioSources[id].m_fileDir,
					m_audioSources[id].m_audioClip->m_is3D,
					m_audioSources[id].m_audioClip->m_isLooping,
					m_audioSources[id].m_audioClip->m_isStreamed);
			}
		}//*/

		inline void PlayAudio(size_t id, const std::string& channelGroupName = PPA_NULL_STRING, const glm::vec3& pos = PhysicsDLC::Vector::Zero)
		{
			if (m_audioSources.size() > id)
			{
				if (!m_audioSources[id].m_audioClip)
					return;

				if(m_audioSources[id].c_playing)
					PPA::AudioEngine::StopPlayingChannel(m_audioSources[id].c_channelID);

				m_audioSources[id].c_channelID = PPA::AudioEngine::PlaySound(
					m_audioSources[id].m_fileDir,
					m_audioSources[id].m_volume,
					pos,
					channelGroupName);

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
	};
}

#endif // !PHYSICSCOMPONENTS_H_
