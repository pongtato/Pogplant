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

	RTTR_REGISTRATION
	{
		rttr::registration::class_<Tag>("Tag")
			 .property("data", &Tag::m_tag);
	}

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
			glm::mat4 model = glm::mat4{ 1 };
			model = glm::translate(model, m_position);
			model = glm::rotate(model, m_rotation[0], { 1,0,0 });
			model = glm::rotate(model, m_rotation[1], { 0,1,0 });
			model = glm::rotate(model, m_rotation[2], { 0,0,1 });
			model = glm::scale(model, m_scale);

			// Convert back to float
			memcpy(m_ModelMtx, glm::value_ptr(model), sizeof(model));
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
		glm::vec3 m_Color;
		float m_Intensity;
	};

	struct Point_Light : public Light
	{
		float m_Linear;
		float m_Quadratic;
	};

	struct Directional_Light : public Light
	{
		glm::vec3 m_Direction;
		float m_Diffuse;
		float m_Specular;
	};
}



#endif