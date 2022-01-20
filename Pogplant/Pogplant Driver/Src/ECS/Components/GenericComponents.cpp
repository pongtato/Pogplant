#include "Pogplant.h"
#include "GenericComponents.h"
#include <gtx/vector_angle.hpp>

namespace Components
{
	size_t ParticleSystem::CurveVariable::m_MaxPoints = 9;
	size_t ParticleSystem::CurveVariable::m_DataPoints = 128;

	void Transform::SetGlobalPosition(const glm::vec3& globalPos)
	{
		if (m_parent == entt::null)
		{
			m_position = globalPos;
			return;
		}

		if (!computedLocal)
			ComputeLocalMtxes();

		glm::vec3 pos;
		glm::vec3 rot;
		glm::vec3 scale;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_midddleMatrix),
			glm::value_ptr(pos),
			glm::value_ptr(rot),
			glm::value_ptr(scale)
		);

		pos = globalPos;

		ImGuizmo::RecomposeMatrixFromComponents
		(
			glm::value_ptr(pos),
			glm::value_ptr(rot),
			glm::value_ptr(scale),
			glm::value_ptr(m_midddleMatrix)
		);

		m_ModelMtxLocal = glm::inverse(m_localToWorld) * m_midddleMatrix;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_ModelMtxLocal),
			glm::value_ptr(m_position),
			glm::value_ptr(m_rotation),
			glm::value_ptr(m_scale)
		);
	}

	void Transform::SetGlobalRotation(const glm::vec3& globalRot)
	{
		if (m_parent == entt::null)
		{
			m_rotation = globalRot;
			return;
		}

		if (!computedLocal)
			ComputeLocalMtxes();

		glm::vec3 pos;
		glm::vec3 rot;
		glm::vec3 scale;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_midddleMatrix),
			glm::value_ptr(pos),
			glm::value_ptr(rot),
			glm::value_ptr(scale)
		);

		rot = globalRot;

		ImGuizmo::RecomposeMatrixFromComponents
		(
			glm::value_ptr(pos),
			glm::value_ptr(rot),
			glm::value_ptr(scale),
			glm::value_ptr(m_midddleMatrix)
		);

		m_ModelMtxLocal = glm::inverse(m_localToWorld) * m_midddleMatrix;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_ModelMtxLocal),
			glm::value_ptr(m_position),
			glm::value_ptr(m_rotation),
			glm::value_ptr(m_scale)
		);
	}

	void Transform::SetGlobalScale(const glm::vec3& globalScale)
	{
		if (m_parent == entt::null)
		{
			m_scale = globalScale;
			return;
		}

		if (!computedLocal)
			ComputeLocalMtxes();

		glm::vec3 pos;
		glm::vec3 rot;
		glm::vec3 scale;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_midddleMatrix),
			glm::value_ptr(pos),
			glm::value_ptr(rot),
			glm::value_ptr(scale)
		);

		scale = globalScale;

		ImGuizmo::RecomposeMatrixFromComponents
		(
			glm::value_ptr(pos),
			glm::value_ptr(rot),
			glm::value_ptr(scale),
			glm::value_ptr(m_midddleMatrix)
		);

		m_ModelMtxLocal = glm::inverse(m_localToWorld) * m_midddleMatrix;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_ModelMtxLocal),
			glm::value_ptr(m_position),
			glm::value_ptr(m_rotation),
			glm::value_ptr(m_scale)
		);
	}

	glm::vec3 Transform::GetGlobalPosition()
	{
		if (m_parent == entt::null)
			return m_position;

		if (!computedLocal)
			ComputeLocalMtxes();

		glm::vec3 pos;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_midddleMatrix),
			glm::value_ptr(pos),
			nullptr,
			nullptr
		);

		return pos;
	}

	glm::vec3 Transform::GetGlobalRotation()
	{
		if (m_parent == entt::null)
			return m_rotation;

		if (!computedLocal)
			ComputeLocalMtxes();

		glm::vec3 rot;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_midddleMatrix),
			nullptr,
			glm::value_ptr(rot),
			nullptr
		);

		return rot;
	}
	
	glm::vec3 Transform::GetGlobalScale()
	{
		if (m_parent == entt::null)
			 return m_scale;

		if (!computedLocal)
			ComputeLocalMtxes();

		glm::vec3 scale;

		ImGuizmo::DecomposeMatrixToComponents
		(
			glm::value_ptr(m_midddleMatrix),
			nullptr,
			nullptr,
			glm::value_ptr(scale)
		);

		return scale;
	}

	glm::vec3 Transform::GetForwardVector()
	{
		glm::vec3 rot = GetGlobalRotation();
		glm::mat4 rotMtx;
		ImGuizmo::RecomposeRotationMatrix(glm::value_ptr(rot), glm::value_ptr(rotMtx));

		return rotMtx * glm::vec4{0.f, 0.f, 1.f, 0.f};

		/*glm::mat4 inverted = glm::inverse(m_ModelMtx);
		return normalize(glm::vec3(-inverted[2].x, -inverted[2].y, inverted[2].z));//*/
	}

	glm::vec3 Transform::GetUpVector()
	{
		glm::vec3 rot = GetGlobalRotation();
		glm::mat4 rotMtx;
		ImGuizmo::RecomposeRotationMatrix(glm::value_ptr(rot), glm::value_ptr(rotMtx));

		return rotMtx * glm::vec4{ 0.f, 1.f, 0.f, 0.f };
	}

	void Transform::ComputeLocalMtxes()
	{
		computedLocal = true;
		m_midddleMatrix = m_localToWorld * m_ModelMtxLocal;
	}

	Camera::Camera()
		: m_Orientation{ glm::quat{0,0,0,-1} }
		, m_Projection{ glm::mat4{1} }
		, m_View{ glm::mat4{1} }
		, m_Front{ glm::vec3{0,0,-1} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Pitch{ 0.0f }
		, m_Yaw{ 0.0f }
		, m_Roll{ 0.0f }
		, m_Fov{ 45.0f }
		, m_Near{ 0.1f }
		, m_Far{ 420.0f }
		, m_Active{ false }
	{
	}

	Camera::Camera(bool _Active)
		: m_Orientation{ glm::quat{0,0,0,-1} }
		, m_Projection{ glm::mat4{1} }
		, m_View{ glm::mat4{1} }
		, m_Front{ glm::vec3{0,0,-1} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Pitch{ 0.0f }
		, m_Yaw{ 0.0f }
		, m_Roll{ 0.0f }
		, m_Fov{ 45.0f }
		, m_Near{ 0.1f }
		, m_Far{ 420.0f }
		, m_Active{ false }
	{
		m_Active = _Active;
	}

	ParticleSystem::ParticleSystem()
		: m_Color{ glm::vec4{1} }
		, m_SpawnDirection{ glm::vec3{0,1,0} }
		, m_Force{ 1.0f }
		, m_BillboardAxis{ glm::vec3{1,1,1} }
		, m_Speed{ }
		, m_Scale{ }
		, m_SpawnRadius{ 1.0f }
		, m_ConeRadiusMin{ 1.0f }
		, m_ConeRadiusMax{ 0.0f }
		, m_TargetConeScale{ 45.0f }
		, m_Delay{ 1.0f }
		, m_SubDelay{1.0f}
		, m_Timer{ 0 }
		, m_MinLife{ 1.0f }
		, m_MaxLife{ 1.0f }
		, m_TexName{ "" }
		, m_ActiveCount{ 0 }
		, m_SpawnCount{ 1 }
		, m_SubSpawnCount{ 1 }
		, m_SubActiveCount{ 0 }
		, m_CurrentLifetime{ 0.0f }
		, m_EmitterType{ EMITTER_TYPE::GENERAL }
		, m_Loop{ false }
		, m_Done{ false }
		, m_RandomRotate{ false }
		, m_Play{ false }
		, m_Pause{ false }
		, m_FollowParent{ false }
		, m_Trail{ false }
		, m_TexID{-1}
	{
	}

	ParticleSystem::ParticleSystem
	(
		glm::vec4 _Color,
		glm::vec3 _SpawnDir,
		glm::vec3 _Force,
		glm::vec3 _BillboardAxis,
		float _SpawnRadius,
		float _ConeRadiusMin,
		float _ConeRadiusMax,
		float _TargetConeScale,
		float _Delay,
		float _SubDelay, // Delay for each point to form a trail
		float _MinLife,
		float _MaxLife,
		CurveVariable _Speed,
		CurveVariable _Scale,
		SpriteAnimation _SpriteAnimation,
		std::string _TexName,
		int _SpawnCount,
		int _SubSpawnCount,
		int _EmitterType,
		bool _Loop,
		bool _RandomRotate,
		bool _FollowParent,
		bool _Trail
	)
		: m_Color{ _Color }
		, m_SpawnDirection{ _SpawnDir }
		, m_Force{ _Force }
		, m_BillboardAxis { _BillboardAxis }
		, m_Speed{ _Speed }
		, m_Scale{ _Scale }
		, m_SpriteAnimation { _SpriteAnimation }
		, m_SpawnRadius{ _SpawnRadius }
		, m_ConeRadiusMin{ _ConeRadiusMin }
		, m_ConeRadiusMax{ _ConeRadiusMax }
		, m_TargetConeScale{ _TargetConeScale }
		, m_Delay{ _Delay }
		, m_SubDelay{ _SubDelay }
		, m_Timer{ 0 }
		, m_MinLife{ _MinLife }
		, m_MaxLife{ _MaxLife }
		, m_TexName{ _TexName }
		, m_ActiveCount{ 0 }
		, m_SpawnCount{ _SpawnCount }
		, m_SubSpawnCount{ _SubSpawnCount }
		, m_SubActiveCount{ 0 }
		, m_CurrentLifetime{ 0.0f }
		, m_EmitterType{ static_cast<EMITTER_TYPE>(_EmitterType) }
		, m_Loop{ _Loop }
		, m_Done{ false }
		, m_RandomRotate{ _RandomRotate }
		, m_Play{ false }
		, m_Pause{ false }
		, m_FollowParent{ _FollowParent }
		, m_Trail{ _Trail }
		, m_TexID{ -1 }
	{
		//m_TexID = static_cast<int>(PP::TextureResource::m_TexturePool[_TexName]);
		//auto rawID = PP::TextureResource::m_TexturePool[m_TexName];
		//m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[m_TexName].m_MappedID);
	}

	void ParticleSystem::init()
	{
		Clear();
		m_Pause = false;
		//m_TexID = static_cast<int>(PP::TextureResource::m_TexturePool[m_TexName]);
		//auto rawID = PP::TextureResource::m_TexturePool[m_TexName];
		//m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[rawID]);
		m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[m_TexName].m_MappedID);

		m_CurrentLifetime = 0.0f;
	}

	void ParticleSystem::Update(float _Dt, const Transform& _Transform, const glm::vec3& _CamPos)
	{
		if (!m_Play || m_Pause)
		{
			// Just update render
			for (int i = 0; i < m_ActiveCount; i++)
			{
				UpdateInstance(m_ParticlePool[i], 0.0, _CamPos, _Transform.m_ModelMtx, m_FollowParent);
			}
			return;
		}

		switch (m_EmitterType)
		{
		case ParticleSystem::EMITTER_TYPE::GENERAL:
			// Spawn delay
			m_Timer += _Dt;
			if (m_Timer >= m_Delay)
			{
				m_Timer = 0.0f;
				Spawn(_Transform.m_position, glm::sphericalRand(m_SpawnRadius), m_SpawnDirection);
			}
			break;
		case ParticleSystem::EMITTER_TYPE::BURST:
			if (!m_Done)
			{
				// Only spawn when everything has despawned
				if (m_ActiveCount == 0)
				{
					// To loop or not
					if (!m_Loop)
					{
						m_Done = true;
						m_Play = false;
						m_Pause = false;
					}
					else
					{
						for (int i = 0; i < m_SpawnCount; i++)
						{
							Spawn(_Transform.m_position, glm::vec3{ 0 }, glm::sphericalRand(1.0f));
						}
					}
				}
			}
			break;
		case ParticleSystem::EMITTER_TYPE::CONE:
			// Spawn delay
			m_Timer += _Dt;
			if (m_Timer >= m_Delay)
			{
				const float angle = glm::dot(m_SpawnDirection, glm::vec3{ 0,1,0 });
				glm::vec3 right = glm::vec3{ 1 };
				glm::vec3 up = glm::vec3{ 1 };
				// XZ plane 
				if (angle < -1.0f + 0.001f || angle > 1.0f - 0.001f)
				{
					right = glm::vec3{ 1,0,0 };
					up = glm::vec3{ 0,0,1 };
				}
				else
				{
					right = glm::cross(m_SpawnDirection, glm::vec3{ 0,1,0 });
					up = glm::cross(m_SpawnDirection, right);
				}

				m_Timer = 0.0f;
				float length = 0.0f;
				glm::vec2 offset = glm::vec2{ 0.0f };
				if (m_ConeRadiusMin < m_ConeRadiusMax)
				{
					while (m_ConeRadiusMin > 0.0f && length < m_ConeRadiusMin)
					{
						offset = glm::diskRand(m_ConeRadiusMax);
						length = glm::length(offset);
					}
				}

				const glm::vec3 resultantPos = right * offset.x + up * offset.y;
				// "Outer ring" along direction to move outwards
				const glm::vec3 targetPos = m_SpawnDirection * m_SpawnRadius + resultantPos * m_TargetConeScale;
				const glm::vec3 resultantDir = glm::normalize(targetPos - resultantPos);

				if (m_Trail)
				{
					// Scale up if need more
					if (m_SubActiveCount >= m_SubEmitters.size())
					{
						m_SubEmitters.resize((m_SubEmitters.size() + 1) * 2);
					}
					m_SubEmitters[m_SubActiveCount] = SubEmitter(resultantPos, resultantDir, m_SubSpawnCount);
					m_SubActiveCount++;
				}
				else
				{
					Spawn(_Transform.m_position, resultantPos, resultantDir);
				}
			}
			break;
		}

		// Update sub emitters
		for (int i = 0; i < m_SubActiveCount; i++)
		{
			auto& subEmi = m_SubEmitters[i];
			if (subEmi.m_Count > 0)
			{
				if(subEmi.Update(_Dt, m_SubDelay))
				{
					Spawn(_Transform.m_position, subEmi.m_Position, subEmi.m_Direction);
				}
			}
			else
			{
				m_SubActiveCount--;
				std::swap(subEmi, m_SubEmitters[m_SubActiveCount]);
				i--;
				continue;
			}
		}

		// Update particles
		for (int i = 0; i < m_ActiveCount; i++)
		{
			Particle& it = m_ParticlePool[i];
			if (it.m_Life > 0.0f)
			{
				// Decrease life
				it.m_Life -= _Dt;

				if (it.m_Life <= 0.0f)
				{
					m_ActiveCount--;
					std::swap(it, m_ParticlePool[m_ActiveCount]);
					// Since we swapped got to update at this index again, else the particle will "flicker"
					i--;
					continue;
				}
				it.m_SpriteAnimation.Update(_Dt);
				UpdateInstance(m_ParticlePool[i], _Dt, _CamPos, _Transform.m_ModelMtx, m_FollowParent);
			}
		}

		m_CurrentLifetime += _Dt;
	}

	void ParticleSystem::Spawn(glm::vec3 _BasePos, glm::vec3 _RandPos, glm::vec3 _Direction)
	{
		// Scale up if need more
		if (m_ActiveCount >= m_ParticlePool.size())
		{
			m_ParticlePool.resize((m_ParticlePool.size() + 1) * 2);
		}

		float speedMult = glm::linearRand(m_Speed.m_MultiplierMin, m_Speed.m_MultiplierMax);
		float life = glm::linearRand(m_MinLife, m_MaxLife);
		float scaleMult = glm::linearRand(m_Scale.m_MultiplierMin, m_Scale.m_MultiplierMax);
		float randRotate = glm::radians(m_RandomRotate ? glm::linearRand(0.0f, 360.0f) : 0.0f);

		// Update at end of pool
		m_ParticlePool[m_ActiveCount] =
		Particle
		{
			&m_Speed.m_CurveData,
			&m_Scale.m_CurveData,
			m_Color,
			_BasePos,
			_RandPos,
			_Direction,
			glm::vec3{0},
			m_Force,
			{
				m_Speed.m_CurveMin,
				m_Speed.m_CurveMax,
				speedMult
			},
			{
				m_Scale.m_CurveMin,
				m_Scale.m_CurveMax,
				scaleMult
			},
			m_TexID,
			life,
			life,
			1.0f / m_Speed.m_CurveData.size(),
			randRotate,
			m_SpriteAnimation
		};

		m_ActiveCount++;
	}

	void ParticleSystem::Clear()
	{
		m_ParticlePool.clear();
		m_ActiveCount = 0;
	}

	glm::quat RotateBetweenVector(const glm::vec3& _Start, const glm::vec3& _Dest)
	{
		float angle = glm::dot(_Start, _Dest);

		// Parallel
		if (angle < -1.0f + 0.001f)
		{
			glm::vec3 axis = glm::cross(glm::vec3{ 0,0,1 }, _Start);
			if (axis.length() < 0.01f)
			{
				axis = glm::cross(glm::vec3{ 1,0,0 }, _Start);
			}
			axis = glm::normalize(axis);

			return glm::angleAxis(glm::radians(180.0f), axis);
		}

		glm::vec3 axis = glm::cross(_Start, _Dest);
		float s = sqrtf((1 + angle) * 2.0f);
		float inverse = 1.0f / s;
		return glm::quat{ s * 0.5f, axis.x * inverse, axis.y * inverse, axis.z * inverse };
	}

	void ParticleSystem::UpdateInstance(Particle& _Particle, float _Dt, const glm::vec3& _CamPos, const glm::mat4& _ParentMtx, bool _Parented)
	{
		float t = 1.0f - _Particle.m_Life / _Particle.m_BaseLife;
		size_t index = static_cast<size_t>(t / _Particle.m_IndexCalc);
		// For some reason it exceeds the index size

		//Change if needed, I defaulted it to 1.f for both first
		float curveSpeed;
		float curveScale;
		if (!_Particle.m_SpeedCurve->empty())
		{
			index = index >= _Particle.m_SpeedCurve->size() ? _Particle.m_SpeedCurve->size() - 1 : index;
			curveSpeed = (*_Particle.m_SpeedCurve)[index];
			curveScale = (*_Particle.m_ScaleCurve)[index];
		}
		else
		{
			curveSpeed = 1.f;
			curveScale = 1.f;
		}

		// Lerp
		const float speedCalc = _Particle.m_Speed.m_Max * (1 - curveSpeed) + _Particle.m_Speed.m_Min * curveSpeed;
		const float scaleCalc = _Particle.m_Scale.m_Max * (1 - curveScale) + _Particle.m_Scale.m_Min * curveScale;
		const auto scaleMult = scaleCalc * _Particle.m_Scale.m_Multiplier;
		const glm::vec3 scale = glm::vec3{ scaleMult,scaleMult,scaleMult };

		// Update position
		_Particle.m_Velocity += _Dt * _Particle.m_Force;
		glm::vec3 currVel = _Particle.m_Velocity;

		_Particle.m_LocalPosition += currVel * _Dt * speedCalc * _Particle.m_Speed.m_Multiplier; // Speed here is the multiplier randomned 

		glm::vec3 posCalc = glm::vec3{ 1 };

		// Billboard with rotation relative to parent
		if (_Parented)
		{
			// Local
			glm::mat4 localMtx = glm::translate(glm::mat4{ 1 }, _Particle.m_LocalPosition);
			localMtx = glm::scale(localMtx, scale);

			// Get global pos
			glm::mat4 concat = _ParentMtx * localMtx;
			ImGuizmo::DecomposeMatrixToComponents(glm::value_ptr(concat), glm::value_ptr(posCalc), nullptr, nullptr);
		}
		// Spawn and forget
		else
		{
			posCalc = (_Particle.m_BasePosition + _Particle.m_LocalPosition);
		}

		// Billboard with new global position
		glm::vec3 targetDir = _CamPos - posCalc;
		constexpr glm::vec3 particleDir = { 0,0,1 };
		glm::mat4 rotation = glm::mat4{ 1 };
		if (targetDir.length() > 0)
		{
			targetDir = glm::normalize(targetDir);
			float angle = atan2f(targetDir.x, targetDir.z);
			glm::quat yRotate = glm::angleAxis(angle, glm::vec3{ 0,m_BillboardAxis.y,0 });

			int flip = targetDir.y < 0 ? 1 : -1;
			glm::quat xRotate = glm::angleAxis(glm::angle(targetDir, { targetDir.x,0,targetDir.z }) * flip, glm::vec3{ m_BillboardAxis.x,0,0 });

			// Local
			glm::quat zRotate = glm::angleAxis(_Particle.m_Rotation, glm::vec3{ 0,0,m_BillboardAxis.z });
			rotation = glm::mat4_cast(yRotate * xRotate * zRotate);
		}

		// Construct new matrix
		glm::mat4 model = glm::translate(glm::mat4{ 1 }, posCalc);
		model *= rotation;
		model = glm::scale(model, scale);

		Pogplant::MeshInstance::SetInstance
		(
			Pogplant::InstanceData
			{ 
				model,
				_Particle.m_Color,
				_Particle.m_SpriteAnimation.m_Tiling,
				_Particle.m_SpriteAnimation.m_UV_Offset,
				static_cast<int>(_Particle.m_TexID),
				false,
				false 
			}
		);
	}

	ParticleSystem::CurveVariable::CurveVariable()
		: m_CurveData{}
		, m_CurvePoints{ }
		, m_CurveMin{ 0.0f }
		, m_CurveMax{ 1.0f }
		, m_MultiplierMin{ 1.0f }
		, m_MultiplierMax{ 1.0f }
	{
		//m_CurvePoints.resize(m_MaxPoints);

		const size_t second_last = m_MaxPoints - 2;
		const float increment = 1.0f / second_last;
		// Init curve vertices
		for (size_t i = 0; i < second_last; i++)
		{
			float currX = i * increment;
			m_CurvePoints.push_back({ currX, 1.0f });
		}
		m_CurvePoints.push_back({ 1.0f, 1.0f });

		m_CurveData.resize(m_DataPoints);

		int smoothness = static_cast<int>(m_CurveData.size());
		for (int i = 0; i <= (smoothness - 1); ++i) 
		{
			float qx = (i + 1) / float(smoothness);
			//float py = 1 - CurveValueSmooth(px, static_cast<int>(m_CurvePoints.size() + 1), m_CurvePoints.data());
			float qy = 1 - CurveValueSmooth(qx, static_cast<int>(m_CurvePoints.size() + 1), m_CurvePoints.data());
			m_CurveData[i] = qy;
		}
	};

	ParticleSystem::CurveVariable::CurveVariable(const std::vector<ImVec2>& _CurvePoints, float _CurveMin, float _CurveMax, float _MultiMin, float _MultiMax)
		: m_CurveData{}
		, m_CurvePoints{ _CurvePoints }
		, m_CurveMin{ _CurveMin }
		, m_CurveMax{ _CurveMax }
		, m_MultiplierMin{ _MultiMin }
		, m_MultiplierMax{ _MultiMax }
	{
		m_CurveData.resize(m_DataPoints);

		int smoothness = static_cast<int>(m_CurveData.size());
		for (int i = 0; i <= (smoothness - 1); ++i)
		{
			float qx = (i + 1) / float(smoothness);
			//float py = 1 - CurveValueSmooth(px, static_cast<int>(m_CurvePoints.size() + 1), m_CurvePoints.data());
			float qy = 1 - CurveValueSmooth(qx, static_cast<int>(m_CurvePoints.size() + 1), m_CurvePoints.data());
			m_CurveData[i] = qy;
		}
	}

	float ParticleSystem::CurveVariable::CurveValueSmooth(float _P, int _Maxpoints, const ImVec2* _Points)
	{
		if (_Maxpoints < 2 || _Points == 0)
		{
			return 0;
		}

		if (_P < 0)
		{
			return _Points[0].y;
		}

		float* input = new float[_Maxpoints * 2];
		float output[4];

		for (int i = 0; i < _Maxpoints; ++i) {
			input[i * 2 + 0] = _Points[i].x;
			input[i * 2 + 1] = _Points[i].y;
		}

		Spline(input, _Maxpoints, 1, _P, output);

		delete[] input;
		return output[0];
	}

	void ParticleSystem::CurveVariable::Spline(const float* _Key, int _Num, int _Dim, float _T, float* _V)
	{
		static signed char coefs[16] = {
			-1, 2,-1, 0,
			 3,-5, 0, 2,
			-3, 4, 1, 0,
			 1,-1, 0, 0 };

		const int size = _Dim + 1;

		// find key
		int k = 0; while (_Key[k * size] < _T) k++;

		// interpolant
		const float h = (_T - _Key[(k - 1) * size]) / (_Key[k * size] - _Key[(k - 1) * size]);

		// init result
		for (int i = 0; i < _Dim; i++) _V[i] = 0.0f;

		// add basis functions
		for (int i = 0; i < 4; i++)
		{
			int kn = k + i - 2; if (kn < 0) kn = 0; else if (kn > (_Num - 1)) kn = _Num - 1;

			const signed char* co = coefs + 4 * i;

			const float b = 0.5f * (((co[0] * h + co[1]) * h + co[2]) * h + co[3]);

			for (int j = 0; j < _Dim; j++) _V[j] += b * _Key[kn * size + j + 1];
		}
	}

	Canvas::Canvas(const glm::vec4& _Color, std::string _TexName, bool _Ortho)
		: m_Color {_Color}
		, m_TexName {_TexName}
		, m_TexID {0}
		, m_Ortho {_Ortho}
	{
		init();
	}

	void Canvas::init()
	{
		//auto rawID = PP::TextureResource::m_TexturePool[m_TexName];
		//m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[rawID]);
		m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[m_TexName].m_MappedID);
	}

	ParticleSystem::SubEmitter::SubEmitter(const glm::vec3& _Position, const glm::vec3& _Direction, int _Count)
		: m_Position { _Position }
		, m_Direction { _Direction }
		, m_Count { _Count }
		, m_Timer {0.0f}
	{
	}

	bool ParticleSystem::SubEmitter::Update(float _Dt, float _Delay)
	{
		m_Timer += _Dt;
		if (m_Timer >= _Delay)
		{
			m_Timer = 0.0f;
			m_Count--;
			return true;
		}

		return false;
	}

	PrefabInstance::PrefabInstance(std::string _str, std::string _path) :
		prefab_GUID(_str), prefab_path(_path)
	{
	}

	SpriteAnimation::SpriteAnimation()
		: m_Tiling{ 1,1 }
		, m_UV_Offset{ 0,0 }
		, m_FrameCounter{ 0 }
		, m_PlaySpeed{ 1 }
		, m_MaxFrames{ 1 }
		, m_Rows{ 1 }
		, m_Columns{ 1 }
		, m_Repeat{ false }
		, m_Play{ false }
		, m_Pause{ false }
	{
	}

	SpriteAnimation::SpriteAnimation(int _MaxFrames, float _CurrentFrame, int _Rows, int _Columns, bool _Repeat, bool _Playing, float _PlaySpeed)
		: m_Tiling{ 1.0f / _Columns, 1.0f / _Rows }
		, m_UV_Offset{ 0,0 }
		, m_FrameCounter{ _CurrentFrame }
		, m_PlaySpeed{ _PlaySpeed }
		, m_MaxFrames{ _MaxFrames }
		, m_Rows{ _Rows }
		, m_Columns{ _Columns }
		, m_Repeat{ _Repeat }
		, m_Play{ _Playing }
		, m_Pause{ false }
	{
	}

	void SpriteAnimation::Update(float _Dt)
	{
		// If paused or stop just do not update
		if (!m_Play || m_Pause)
		{
			return;
		}

		// Current frame
		m_FrameCounter += _Dt * m_PlaySpeed;
		// Cap at maximum frame/restart, -1 since start from 0
		if (m_FrameCounter >= m_MaxFrames)
		{
			if (m_Repeat)
			{
				m_FrameCounter = 0.0f;
			}
			else
			{
				m_FrameCounter = static_cast<float>(m_MaxFrames - 1);
			}
		}

		CalcUV();
	}

	void SpriteAnimation::UpdateTiling()
	{
		m_FrameCounter = 0.0f;
		m_Tiling = { 1.0f / m_Columns, 1.0f / m_Rows };
		m_UV_Offset = { 0.0f, (m_Rows - 1) * m_Tiling.y };
	}

	void SpriteAnimation::NextFrame()
	{
		m_FrameCounter = m_FrameCounter + 1 < m_MaxFrames ? m_FrameCounter + 1 : m_FrameCounter;
		CalcUV();
	}

	void SpriteAnimation::PrevFrame()
	{
		m_FrameCounter = m_FrameCounter - 1 > 0 ? m_FrameCounter - 1 : 0;
		CalcUV();
	}

	void SpriteAnimation::SetFrame(int _NewFrame)
	{
		if (_NewFrame < 0 || _NewFrame >= m_MaxFrames)
		{
			return;
		}
		m_FrameCounter = static_cast<float>(_NewFrame);
		CalcUV();
	}

	void SpriteAnimation::init()
	{
		//m_FrameCounter = 0.0f;
	}

	void SpriteAnimation::CalcUV()
	{
		// Calculate the UV offset
		const int currFrame = static_cast<int>(m_FrameCounter);
		// Rows = y, Columns = x
		// Assuming we have 4 rows and 4 columns, tiling is {0.25,0.25}  
		// Frame 6 will result in an offset of {0.5,0.25}
		int yOffset = currFrame / m_Columns;
		int xOffset = currFrame % m_Columns;
		m_UV_Offset = { xOffset * m_Tiling.x, (m_Rows - 1) * m_Tiling.y - yOffset * m_Tiling.y };
	}
}