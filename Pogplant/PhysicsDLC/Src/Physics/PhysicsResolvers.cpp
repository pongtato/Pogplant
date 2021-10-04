#include "PhysicsResolvers.h"

namespace PhysicsDLC
{
	namespace Physics
	{
		Rigidbody::Rigidbody(
			float c_mass,
			float c_drag,
			bool c_isKinematic,
			bool c_useGravity)
			:
			mass{ c_mass },
			drag{ c_drag },
			isKinematic{ c_isKinematic },
			useGravity{ c_useGravity },
			velocity{ 0.f, 0.f, 0.f },
			acceleration{ 0.f, 0.f, 0.f },
			impulseAcceleration{ 0.f, 0.f, 0.f },
			newPosition{ 0.f, 0.f, 0.f }
		{
		}

		void Rigidbody::AddForce(glm::vec3 direction)
		{
			//F = MA
			//A = F/M
			acceleration += direction * (1.f / mass);
		}

		void Rigidbody::AddImpulseForce(glm::vec3 direction)
		{
			impulseAcceleration += direction * (1.f / mass);
		}

		void ResolveAABBDynamic(
			vec3& _1pos,
			Rigidbody& _1rigidbody,
			Rigidbody* _2rigidbody,
			Collision::Shapes::AABB& _1aabb,
			Collision::Shapes::AABB& _2aabb,
			float collisionTime,
			float dt)
		{
			assert(dt > collisionTime);

			static vec3 m_hotSpot[6];
			static float extendCheck = 0.01f;
			static float edgeCheck = 0.05f;

			static Collision::Shapes::AABB hsBounds;

			if (_2rigidbody != nullptr)
			{
				_2aabb.m_min += _2rigidbody->velocity * collisionTime;
				_2aabb.m_max += _2rigidbody->velocity * collisionTime;
			}

			_1rigidbody.newPosition = _1pos + _1rigidbody.velocity * collisionTime;

			_1aabb.m_min += _1rigidbody.velocity * collisionTime;
			_1aabb.m_max += _1rigidbody.velocity * collisionTime;

			hsBounds.m_min = _1aabb.m_min + _1rigidbody.velocity * collisionTime;
			hsBounds.m_max = _1aabb.m_max + _1rigidbody.velocity * collisionTime;

			hsBounds.m_max.x -= edgeCheck;
			hsBounds.m_max.y -= edgeCheck;
			hsBounds.m_max.z -= edgeCheck;

			hsBounds.m_min.x += edgeCheck;
			hsBounds.m_min.y += edgeCheck;
			hsBounds.m_min.z += edgeCheck;

			//top +y
			m_hotSpot[0].x = glm::clamp((_2aabb.m_min.x + _2aabb.m_max.x) * 0.5f, hsBounds.m_min.x, hsBounds.m_max.x);
			m_hotSpot[0].y = _1aabb.m_max.y + extendCheck;
			m_hotSpot[0].z = glm::clamp((_2aabb.m_min.z + _2aabb.m_max.z) * 0.5f, hsBounds.m_min.z, hsBounds.m_max.z);

			//bottom -y
			m_hotSpot[1].x = m_hotSpot[0].x;
			m_hotSpot[1].y = _1aabb.m_min.y - extendCheck;
			m_hotSpot[1].z = m_hotSpot[0].z;

			//right +x
			m_hotSpot[2].x = _1aabb.m_max.x + extendCheck;
			m_hotSpot[2].y = glm::clamp((_2aabb.m_min.y + _2aabb.m_max.y) * 0.5f, hsBounds.m_min.y, hsBounds.m_max.y);
			m_hotSpot[2].z = m_hotSpot[0].z;

			//left -x
			m_hotSpot[3].x = _1aabb.m_min.x - extendCheck;
			m_hotSpot[3].y = m_hotSpot[2].y;
			m_hotSpot[3].z = m_hotSpot[0].z;

			//front +z
			m_hotSpot[4].x = m_hotSpot[0].x;
			m_hotSpot[4].y = m_hotSpot[2].y;
			m_hotSpot[4].z = _1aabb.m_max.z + extendCheck;

			//back -z
			m_hotSpot[5].x = m_hotSpot[0].x;
			m_hotSpot[5].y = m_hotSpot[2].y;
			m_hotSpot[5].z = _1aabb.m_min.z - extendCheck;

			for (size_t i = 0; i < 6; i++)
			{
				if (Collision::PointAABB(m_hotSpot[i], _2aabb))
				{
					switch (i)
					{
					case 0:
						if (_1rigidbody.velocity.y > 0)
						{
							_1rigidbody.velocity.y = 0.f;
							_1rigidbody.newPosition.y += _2aabb.m_min.y - _1aabb.m_max.y;
						}
						break;
					case 1:
						if (_1rigidbody.velocity.y < 0)
						{
							_1rigidbody.velocity.y = 0.f;
							_1rigidbody.newPosition.y += _2aabb.m_max.y - _1aabb.m_min.y;
						}
						break;
					case 2:
						if (_1rigidbody.velocity.x > 0)
						{
							_1rigidbody.velocity.x = 0.f;
							_1rigidbody.newPosition.x += _2aabb.m_min.x - _1aabb.m_max.x;
						}
						break;
					case 3:
						if (_1rigidbody.velocity.x < 0)
						{
							_1rigidbody.velocity.x = 0.f;
							_1rigidbody.newPosition.x += _2aabb.m_max.x - _1aabb.m_min.x;
						}
						break;
					case 4:
						if (_1rigidbody.velocity.z > 0)
						{
							_1rigidbody.velocity.z = 0.f;
							_1rigidbody.newPosition.z += _2aabb.m_min.z - _1aabb.m_max.z;
						}
						break;
					case 5:
						if (_1rigidbody.velocity.z < 0)
						{
							_1rigidbody.velocity.z = 0.f;
							_1rigidbody.newPosition.z += _2aabb.m_max.z - _1aabb.m_min.z;
						}
						break;
					default:
						throw std::exception("Wait what");
					}
				}
			}

			_1rigidbody.newPosition += _1rigidbody.velocity * (dt - collisionTime);
		}
	}
}