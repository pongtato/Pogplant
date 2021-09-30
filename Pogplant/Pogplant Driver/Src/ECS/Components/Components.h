#ifndef COMPONENTS_H
#define COMPONENTS_H

#include "../../Pogplant/Src/Model.h"
#include "../entt_ecs/entt.hpp"

#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

#include <rttr/registration>

namespace Components
{
	struct Tag
	{
		std::string m_tag;
	};

	struct Name
	{
		Name(std::string name) : m_name(name) {}
		std::string m_name;
	};

	struct TheUnkillable6969xx
	{
		//anything with this won't be deleted
	};

	struct PositionList
	{
		short m_index;
	};

	struct Imaginary_object
	{
		std::string str;
	};

	struct Relationship
	{
		entt::entity m_parent = entt::null;
		std::set<entt::entity> m_children;

	};

	struct Transform
	{
		glm::vec3 m_position;
		glm::vec3 m_rotation;
		glm::vec3 m_scale;

		float m_ModelMtx[16];

		Transform() = default;
		Transform(const Transform&) = default;
		Transform(glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
			: m_position(pos), m_rotation(rot), m_scale(scale)
		{
			init();
		}

		void init(void)
		{
			glm::mat4 model = glm::mat4{ 1 };
			model = glm::translate(model, m_position);
			model = glm::rotate(model, m_rotation[0], { 1,0,0 });
			model = glm::rotate(model, m_rotation[1], { 0,1,0 });
			model = glm::rotate(model, m_rotation[2], { 0,0,1 });
			model = glm::scale(model, m_scale);

			// Convert back to float
			memcpy(m_ModelMtx, glm::value_ptr(model), sizeof(model));

			//std::cout << "init called " << std::endl;
		}
	};


	struct Render
	{
		glm::mat4 m_Model;
		glm::vec3 m_ColorTint;
		Pogplant::Model* m_RenderModel;
		int m_UseLight = 1; // int cause opegnl no bool kekw
	};

	struct Light
	{
		Light(){};
		Light(glm::vec3 _color, float _intensity)
			: m_Color(_color), m_Intensity(_intensity) {}

		glm::vec3 m_Color;
		float m_Intensity;

		RTTR_ENABLE();
	};

	struct Point_Light : public Light
	{
		Point_Light(){};
		Point_Light(glm::vec3 _color, float _intensity, float _Linear, float _Quadratic)
			: Light{ _color, _intensity }, m_Linear(_Linear), m_Quadratic(_Quadratic){}

		float m_Linear;
		float m_Quadratic;

		RTTR_ENABLE(Light);
	};

	struct Directional_Light : public Light
	{
		Directional_Light() {};
		Directional_Light(glm::vec3 _color, float _intensity, glm::vec3 _direction, float _diffuse, float _specular)
			: Light{_color, _intensity }, m_Direction(_direction), m_Diffuse(_diffuse), m_Specular(_specular) {}

		glm::vec3 m_Direction;
		float m_Diffuse;
		float m_Specular;

		RTTR_ENABLE(Light);
	};



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
	}


}



#endif