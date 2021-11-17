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
		, m_Speed{ }
		, m_Scale{ }
		, m_SpawnRadius{ 1.0f }
		, m_Delay{ 1.0f }
		, m_Timer{ 0 }
		, m_MinLife{ 1.0f }
		, m_MaxLife{ 1.0f }
		, m_TexName{ "" }
		, m_ActiveCount{ 0 }
		, m_SpawnCount{ 100 }
		, m_Loop{ false }
		, m_Done{ false }
		, m_Burst{ false }
		, m_RandomRotate{ false }
		, m_Play{ false }
		, m_Pause{ false }
		, m_FollowParent{ false }
	{
		m_TexID = -1;
	}

	ParticleSystem::ParticleSystem
	(
		glm::vec4 _Color,
		glm::vec3 _SpawnDir,
		glm::vec3 _Force,
		float _SpawnRadius,
		float _Delay,
		float _MinLife,
		float _MaxLife,
		CurveVariable _Speed,
		CurveVariable _Scale,
		std::string _TexName,
		int _SpawnCount,
		bool _Loop,
		bool _Burst,
		bool _RandomRotate,
		bool _FollowParent
	)
		: m_Color{ _Color }
		, m_SpawnDirection{ _SpawnDir }
		, m_Force{ _Force }
		, m_Speed{ _Speed }
		, m_Scale{ _Scale }
		, m_SpawnRadius{ _SpawnRadius }
		, m_Delay{ _Delay }
		, m_Timer{ 0 }
		, m_MinLife{ _MinLife }
		, m_MaxLife{ _MaxLife }
		, m_TexName{ _TexName }
		, m_ActiveCount{ 0 }
		, m_SpawnCount{ _SpawnCount }
		, m_Loop{ _Loop }
		, m_Done{ false }
		, m_Burst{ _Burst }
		, m_RandomRotate{ _RandomRotate }
		, m_Play{ false }
		, m_Pause{ false }
		, m_FollowParent{ _FollowParent }
	{
		//m_TexID = static_cast<int>(PP::TextureResource::m_TexturePool[_TexName]);
		auto rawID = PP::TextureResource::m_TexturePool[m_TexName];
		m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[rawID]);
	}

	void ParticleSystem::init()
	{
		Clear();
		m_Pause = false;
		//m_TexID = static_cast<int>(PP::TextureResource::m_TexturePool[m_TexName]);
		auto rawID = PP::TextureResource::m_TexturePool[m_TexName];
		m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[rawID]);
	}

	void ParticleSystem::Spawn(glm::vec3 _Position, glm::vec3 _Direction)
	{
		// Scale up if need more
		if (m_ActiveCount >= m_ParticlePool.size())
		{
			m_ParticlePool.resize(m_ParticlePool.size() + 1 * 2);
		}

		float speedMult = glm::linearRand(m_Speed.m_MultiplierMin, m_Speed.m_MultiplierMax);
		float life = glm::linearRand(m_MinLife, m_MaxLife);
		float scaleMult = glm::linearRand(m_Scale.m_MultiplierMin, m_Scale.m_MultiplierMax);
		float randRotate = glm::radians(m_RandomRotate ? glm::linearRand(0.0f, 360.0f) : 0.0f);

		glm::vec3 randPos = glm::vec3{ 0 };
		if (m_SpawnRadius > 0)
		{
			randPos = glm::sphericalRand(m_SpawnRadius);
		}

		// Update at end of pool
		m_ParticlePool[m_ActiveCount] =
		Particle
		{
			&m_Speed.m_CurveData,
			&m_Scale.m_CurveData,
			m_Color,
			_Position,
			randPos,
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
			randRotate
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

	void ParticleSystem::UpdateInstance(Particle& _Particle, float _Dt, const glm::vec3& _CamPos, const glm::vec3& _ParentPos, bool _Parented)
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

		const glm::vec3 posCalc = _Parented ? _ParentPos + _Particle.m_LocalPosition : _Particle.m_BasePosition + _Particle.m_LocalPosition;
		/// This depends on the camera transform update above to save access calculation for billboarding
		glm::mat4 model = glm::translate(glm::mat4{ 1 }, posCalc);
		glm::vec3 targetDir = _CamPos - posCalc;
		constexpr glm::vec3 particleDir = { 0,0,1 };
		if (targetDir.length() > 0)
		{
			targetDir = glm::normalize(targetDir);
			float angle = atan2f(targetDir.x, targetDir.z);
			glm::quat yRotate = glm::angleAxis(angle, glm::vec3{ 0,1,0 });

			int flip = targetDir.y < 0 ? 1 : -1;
			glm::quat xRotate = glm::angleAxis(glm::angle(targetDir, { targetDir.x,0,targetDir.z }) * flip, glm::vec3{ 1,0,0 });

			// Local
			glm::quat zRotate = glm::angleAxis(_Particle.m_Rotation, glm::vec3{ 0,0,1 });
			model *= glm::mat4_cast(yRotate * xRotate * zRotate);
		}

		model *= glm::scale(glm::mat4{ 1 }, scale);

		Pogplant::MeshInstance::SetInstance(Pogplant::InstanceData{ model, _Particle.m_Color, static_cast<int>(_Particle.m_TexID), false });
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
	};

	Canvas::Canvas(const glm::vec4& _Color, std::string _TexName)
		: m_Color {_Color}
		, m_TexName {_TexName}
		, m_TexID {0}
	{
		init();
	}

	void Canvas::init()
	{
		auto rawID = PP::TextureResource::m_TexturePool[m_TexName];
		m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[rawID]);
	}
}