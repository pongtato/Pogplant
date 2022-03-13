#ifndef REFLECTION_COMPONENTS
#define REFLECTION_COMPONENTS

#include "GenericComponents.h"
#include "DependantComponents.h"


namespace Components
{
	RTTR_REGISTRATION
	{
		rttr::registration::class_<Transform>("Transform")
		.method("init", &Transform::init)
		.property("m_position", &Transform::m_position)
		.property("m_rotation", &Transform::m_rotation)
		.property("m_scale", &Transform::m_scale);

		rttr::registration::class_<Light>("Light")
			.property("m_Color", &Light::m_Color)
			.property("m_Intensity", &Light::m_Intensity);

		rttr::registration::class_<Point_Light>("Point_Light")
			.property("m_Linear", &Point_Light::m_Linear)
			.property("m_Quadratic", &Point_Light::m_Quadratic);

		rttr::registration::class_<Directional_Light>("Directional_Light")
			.property("m_Direction", &Directional_Light::m_Direction)
			.property("m_Diffuse", &Directional_Light::m_Diffuse)
			.property("m_Specular", &Directional_Light::m_Specular);

		rttr::registration::class_<Name>("Name")
			.property("m_Name", &Name::m_name)
			.property("status", &Name::status);

		rttr::registration::class_<Tag>("Tag")
			.property("m_tag", &Tag::m_tag);

		rttr::registration::class_<Text>("Text")
			.property("m_Color", &Text::m_Color)
			.property("m_FontID", &Text::m_FontID)
			.property("m_Text", &Text::m_Text)
			.property("m_Ortho", &Text::m_Ortho);

		rttr::registration::class_<Rigidbody>("Rigidbody")
			.property("mass", &Rigidbody::mass)
			.property("isKinematic", &Rigidbody::isKinematic)
			.property("useGravity", &Rigidbody::useGravity);

		rttr::registration::class_<Collider>("Collider")
			.property("isTrigger", &Collider::isTrigger)
			.property("isStatic", &Collider::isStatic)
			.property("collisionLayer", &Collider::collisionLayer);

		rttr::registration::class_<BoxCollider>("BoxCollider")
			.property("extends", &BoxCollider::extends)
			.property("centre", &BoxCollider::centre);

		rttr::registration::class_<SphereCollider>("SphereCollider")
			.property("radius", &SphereCollider::radius)
			.property("centre", &SphereCollider::centre);

		rttr::registration::class_<OBBBoxCollider>("OBBBoxCollider")
			.property("extends", &OBBBoxCollider::extends)
			.property("centre", &OBBBoxCollider::centre);

		rttr::registration::class_<MeshCollider>("MeshCollider");

		rttr::registration::class_<Camera>("Camera")
			.property("m_Active", &Camera::m_Active)
			.property("m_Yaw", &Camera::m_Yaw)
			.property("m_Pitch", &Camera::m_Pitch)
			.property("m_Near", &Camera::m_Near)
			.property("m_Far", &Camera::m_Far)
			.property("m_Fov", &Camera::m_Fov);

		rttr::registration::class_<ParticleSystem::CurveVariable>("CurveVariable")
			.method("init", &ParticleSystem::CurveVariable::init)
			.property("m_CurvePoints", &ParticleSystem::CurveVariable::m_CurvePoints)
			.property("m_MultiplierMin", &ParticleSystem::CurveVariable::m_MultiplierMin)
			.property("m_MultiplierMax", &ParticleSystem::CurveVariable::m_MultiplierMax)
			.property("m_CurveMin", &ParticleSystem::CurveVariable::m_CurveMin)
			.property("m_CurveMax", &ParticleSystem::CurveVariable::m_CurveMax);

		rttr::registration::class_<ParticleSystem>("ParticleSystem")
			.method("init", &ParticleSystem::init)
			.property("m_Color", &ParticleSystem::m_Color)
			.property("m_SpawnDirection", &ParticleSystem::m_SpawnDirection)
			.property("m_TargetPos", &ParticleSystem::m_TargetPos)
			.property("m_Force", &ParticleSystem::m_Force)
			.property("m_BillboardAxis", &ParticleSystem::m_BillboardAxis)
			.property("m_Speed", &ParticleSystem::m_Speed)
			.property("m_Scale", &ParticleSystem::m_Scale)
			.property("m_SpriteAnimation", &ParticleSystem::m_SpriteAnimation)
			.property("m_SpawnRadius", &ParticleSystem::m_SpawnRadius)
			.property("m_ConeRadiusMin", &ParticleSystem::m_ConeRadiusMin)
			.property("m_ConeRadiusMax", &ParticleSystem::m_ConeRadiusMax)
			.property("m_TargetConeScale", &ParticleSystem::m_TargetConeScale)
			.property("m_Delay", &ParticleSystem::m_Delay)
			.property("m_SubDelay", &ParticleSystem::m_SubDelay)
			.property("m_MinLife", &ParticleSystem::m_MinLife)
			.property("m_MaxLife", &ParticleSystem::m_MaxLife)
			.property("m_TexName", &ParticleSystem::m_TexName)
			.property("m_SpawnCount", &ParticleSystem::m_SpawnCount)
			.property("m_SubSpawnCount", &ParticleSystem::m_SubSpawnCount)
			.property("_EmitterType", &ParticleSystem::m_EmitterType)
			.property("m_Loop", &ParticleSystem::m_Loop)
			.property("m_RandomRotate", &ParticleSystem::m_RandomRotate)
			.property("m_Play", &ParticleSystem::m_Play)
			.property("m_FollowParent", &ParticleSystem::m_FollowParent)
			.property("m_Trail", &ParticleSystem::m_Trail)
			.property("m_MoveToTarget", &ParticleSystem::m_MoveToTarget);

		rttr::registration::class_<SpriteAnimation>("SpriteAnimation")
			.method("init", &SpriteAnimation::init)
			.property("m_Tiling", &SpriteAnimation::m_Tiling)
			.property("m_UV_Offset", &SpriteAnimation::m_UV_Offset)
			.property("m_FrameCounter", &SpriteAnimation::m_FrameCounter)
			.property("m_PlaySpeed", &SpriteAnimation::m_PlaySpeed)
			.property("m_MaxFrames", &SpriteAnimation::m_MaxFrames)
			.property("m_Rows", &SpriteAnimation::m_Rows)
			.property("m_Columns", &SpriteAnimation::m_Columns)
			.property("m_Repeat", &SpriteAnimation::m_Repeat)
			.property("m_Play", &SpriteAnimation::m_Play);

		rttr::registration::class_<Canvas>("Canvas")
			.method("init", &Canvas::init)
			.property("m_Color", &Canvas::m_Color)
			.property("m_TexName", &Canvas::m_TexName)
			.property("m_Ortho", &Canvas::m_Ortho)
			.property("m_ForceAlpha", &Canvas::m_ForceAlpha)
			.property("m_SpriteAnimation", &Canvas::m_SpriteAnimation);

		rttr::registration::class_<Laser>("Laser")
			.property("m_ActivateLaser", &Laser::m_ActivateLaser)
			.property("m_Spawntime", &Laser::m_Spawntime)
			.property("m_Activetime", &Laser::m_Activetime)
			.property("m_Inactivetime", &Laser::m_Inactivetime)
			.property("m_ChargeParticletime", &Laser::m_ChargeParticletime)
			.property("m_LaserLerptime", &Laser::m_LaserLerptime);


		rttr::registration::class_<ImVec2>("ImVec2")
			.property("m_x", &ImVec2::x)
			.property("m_y", &ImVec2::y);

		rttr::registration::class_<Guid>("Guid")
			.property("m_guid", &Guid::m_guid);

		rttr::registration::class_<Prefab>("Prefab")
			.property("file_path", &Prefab::file_path);

		rttr::registration::class_<PrefabInstance>("PrefabInstance")
			.property("prefab_GUID", &PrefabInstance::prefab_GUID)
			.property("prefab_path", &PrefabInstance::prefab_path);

		rttr::registration::enumeration<ParticleSystem::EMITTER_TYPE>("EMITTER_TYPE")
			(
				rttr::value("GENERAL", ParticleSystem::EMITTER_TYPE::GENERAL),
				rttr::value("BURST", ParticleSystem::EMITTER_TYPE::BURST),
				rttr::value("CONE", ParticleSystem::EMITTER_TYPE::CONE)
			);

		rttr::registration::class_<MovementBounds>("MovementBounds")
			.property("maxX", &MovementBounds::maxX)
			.property("minX", &MovementBounds::minX)
			.property("maxY", &MovementBounds::maxY)
			.property("minY", &MovementBounds::minY);
	}
}

#endif // !REFLECTION_COMPONENTS
