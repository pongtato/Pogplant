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
			.property("m_Name", &Name::m_name);

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
			.property("collisionLayer", &Collider::collisionLayer);

		rttr::registration::class_<BoxCollider>("BoxCollider")
			.property("extends", &BoxCollider::extends)
			.property("centre", &BoxCollider::centre);

		rttr::registration::class_<SphereCollider>("SphereCollider")
			.property("radius", &SphereCollider::radius)
			.property("centre", &SphereCollider::centre);

		rttr::registration::class_<Camera>("Camera")
			.property("m_Active", &Camera::m_Active)
			.property("m_Yaw", &Camera::m_Yaw)
			.property("m_Pitch", &Camera::m_Pitch)
			.property("m_Near", &Camera::m_Near)
			.property("m_Far", &Camera::m_Far)
			.property("m_Zoom", &Camera::m_Zoom);

		rttr::registration::class_<ParticleSystem::CurveVariable>("CurveVariable")
			.property("m_CurvePoints", &ParticleSystem::CurveVariable::m_CurvePoints)
			.property("m_MultiplierMin", &ParticleSystem::CurveVariable::m_MultiplierMin)
			.property("m_MultiplierMax", &ParticleSystem::CurveVariable::m_MultiplierMax)
			.property("m_CurveMin", &ParticleSystem::CurveVariable::m_CurveMin)
			.property("m_CurveMax", &ParticleSystem::CurveVariable::m_CurveMax);

		rttr::registration::class_<ParticleSystem>("ParticleSystem")
			.property("m_Color", &ParticleSystem::m_Color)
			.property("m_SpawnDirection", &ParticleSystem::m_SpawnDirection)
			.property("m_Force", &ParticleSystem::m_Force)
			.property("m_Speed", &ParticleSystem::m_Speed)
			.property("m_Scale", &ParticleSystem::m_Scale)
			.property("m_SpawnRadius", &ParticleSystem::m_SpawnRadius)
			.property("m_Delay", &ParticleSystem::m_Delay)
			//.property("m_TexID", &ParticleSystem::m_TexID)
			.property("m_SpawnCount", &ParticleSystem::m_SpawnCount)
			.property("m_Loop", &ParticleSystem::m_Loop)
			.property("m_Burst", &ParticleSystem::m_Burst)
			.property("m_RandomRotate", &ParticleSystem::m_RandomRotate)
			.property("m_Play", &ParticleSystem::m_Play);

		rttr::registration::class_<Canvas>("Canvas")
			.property("m_Color", &Canvas::m_Color);
			//.property("m_TexID", &Canvas::m_TexID);
	}
}

#endif // !REFLECTION_COMPONENTS
