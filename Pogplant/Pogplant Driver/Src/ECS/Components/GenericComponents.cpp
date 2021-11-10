#include "Pogplant.h"
#include "GenericComponents.h"

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
		glm::mat4 inverted = glm::inverse(m_ModelMtx);
		return normalize(glm::vec3(-inverted[2].x, -inverted[2].y, inverted[2].z));
	}

	void Transform::ComputeLocalMtxes()
	{
		computedLocal = true;
		m_midddleMatrix = m_localToWorld * m_ModelMtxLocal;
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
		bool _RandomRotate
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

		glm::vec3 randPos = _Position;
		if (m_SpawnRadius > 0)
		{
			randPos += glm::sphericalRand(m_SpawnRadius);
		}

		// Update at end of pool
		m_ParticlePool[m_ActiveCount] =
		Particle
		{
			&m_Speed.m_CurveData,
			&m_Scale.m_CurveData,
			m_Color,
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

	void ParticleSystem::UpdateInstance(Particle& _Particle, float _Dt, const glm::vec3& _CamPos)
	{
		constexpr glm::vec3 particleDir = { 0,0,1 };
		float t = 1.0f - _Particle.m_Life / _Particle.m_BaseLife;
		size_t index = static_cast<size_t>(t / _Particle.m_IndexCalc);
		// For some reason it exceeds the index size
		index = index >= _Particle.m_SpeedCurve->size() ? _Particle.m_SpeedCurve->size() - 1 : index;
		const float curveSpeed = (*_Particle.m_SpeedCurve)[index];
		const float curveScale = (*_Particle.m_ScaleCurve)[index];

		// Lerp
		const float speedCalc = _Particle.m_Speed.m_Max * (1 - curveSpeed) + _Particle.m_Speed.m_Min * curveSpeed;
		const float scaleCalc = _Particle.m_Scale.m_Max * (1 - curveScale) + _Particle.m_Scale.m_Min * curveScale;
		const glm::vec3 scale = glm::vec3{ scaleCalc,scaleCalc,scaleCalc } * _Particle.m_Scale.m_Multiplier;

		// Update position
		_Particle.m_Velocity += _Dt * _Particle.m_Force;
		glm::vec3 currVel = _Particle.m_Velocity;

		_Particle.m_Position += currVel * _Dt * speedCalc * _Particle.m_Speed.m_Multiplier; // Speed here is the multiplier randomned 

		glm::mat4 model = glm::mat4{ 1 };
		model = glm::translate(model, _Particle.m_Position);

		// Particle local rot
		model = glm::rotate(model, _Particle.m_Rotation, glm::vec3{ 0,0,1 });

		/// This depends on the camera transform update above to save access calculation for billboarding
		glm::vec3 targetDir = _CamPos - _Particle.m_Position;
		if (targetDir.length() > 0)
		{
			// 2D billboarding only xz plane
			glm::vec3 targetDirXZ = { targetDir.x,0,targetDir.z };
			targetDirXZ = glm::normalize(targetDirXZ);
			glm::vec3 up = glm::cross(particleDir, targetDirXZ);
			float dot = glm::dot(particleDir, targetDirXZ);
			// Dont rotate if parallel
			if (dot < 1.0f || dot > 1.0f)
			{
				float xz_rad = glm::acos(dot);
				model = glm::rotate(model, xz_rad, up);
			}

			// Spherical portion
			targetDir = glm::normalize(targetDir);
			dot = glm::dot(targetDir, targetDirXZ);
			if (dot < 1.0f || dot > 1.0f)
			{
				float rad = glm::acos(dot);
				if (targetDir.y < 0)
				{
					model = glm::rotate(model, rad, { 1,0,0 });
				}
				else
				{
					model = glm::rotate(model, rad, { -1,0,0 });
				}
			}
		};
		model = glm::scale(model, scale);

		Pogplant::MeshInstance::SetInstance(Pogplant::InstanceData{ model, _Particle.m_Color, static_cast<int>(_Particle.m_TexID), false });
	}

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