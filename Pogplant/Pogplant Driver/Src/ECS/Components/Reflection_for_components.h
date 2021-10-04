#ifndef REFLECTION_COMPONENTS
#define REFLECTION_COMPONENTS

#include "GenericComponents.h"
#include "PhysicsComponents.h"



namespace Components
{
	RTTR_REGISTRATION
	{
		rttr::registration::class_<Transform>("Transform")
		//.constructor<glm::vec3, glm::vec3,glm::vec3>()
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

		rttr::registration::class_<BoxCollider>("BoxCollider")
			.property("extends", &BoxCollider::extends)
			.property("centre", &BoxCollider::centre);

		rttr::registration::class_<SphereCollider>("SphereCollider")
			.property("radius", &SphereCollider::radius)
			.property("centre", &SphereCollider::centre);
	}
}

#endif // !REFLECTION_COMPONENTS
