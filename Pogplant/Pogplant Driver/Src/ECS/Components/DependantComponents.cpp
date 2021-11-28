#include "../../Pogplant/Src/Model.h"
#include "../../Pogplant/Src/Mesh.h"
#include "../entt_ecs/entt.hpp"

#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

#include "../../Application.h"

#include "DependantComponents.h"

glm::vec3 Components::MeshCollider::GetCenter() const
{
	auto& renderer = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Renderer>(m_id);
	auto& transform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Transform>(m_id);

	glm::vec3 average{ 0.f, 0.f, 0.f };

	for (auto& vertex : renderer.m_Mesh->m_Vertices)
		average += vertex.m_Position;

	average = average * (1.f / (float)renderer.m_Mesh->m_Vertices.size());

	return (transform.m_ModelMtx * glm::vec4{ average.x, average.y, average.z, 1.0f });
}

glm::vec3 Components::MeshCollider::FindFurthestPoint(const glm::vec3& direction) const
{
	auto& renderer = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Renderer>(m_id);
	auto& transform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Transform>(m_id);

	size_t index = 0;
	float maxProjection = std::numeric_limits<float>::lowest();

	glm::mat4 worldToLocal = glm::inverse(transform.m_ModelMtx);

	glm::vec3 localDirection{
		glm::dot(direction, {transform.m_ModelMtx[0].x, transform.m_ModelMtx[0].y, transform.m_ModelMtx[0].z}),
		glm::dot(direction, {transform.m_ModelMtx[1].x, transform.m_ModelMtx[1].y, transform.m_ModelMtx[1].z}),
		glm::dot(direction, {transform.m_ModelMtx[2].x, transform.m_ModelMtx[2].y, transform.m_ModelMtx[2].z})
	};

	for (size_t i = 0; i < renderer.m_Mesh->m_Vertices.size(); ++i)
	{
		float projection = glm::dot(localDirection, renderer.m_Mesh->m_Vertices[i].m_Position);

		if (projection > maxProjection)
		{
			maxProjection = projection;
			index = i;
		}
	}

	return (transform.m_ModelMtx * glm::vec4{
		renderer.m_Mesh->m_Vertices[index].m_Position.x,
		renderer.m_Mesh->m_Vertices[index].m_Position.y,
		renderer.m_Mesh->m_Vertices[index].m_Position.z,
		1.f });
}
