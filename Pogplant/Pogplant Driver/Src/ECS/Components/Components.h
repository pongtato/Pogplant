#ifndef COMPONENTS_H
#define COMPONENTS_H

#include <Pogplant.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

namespace Components
{
	struct Tag
	{
		std::string m_tag;
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

	struct Renderer
	{
		Pogplant::RenderObject* render_object;
	};
}



#endif