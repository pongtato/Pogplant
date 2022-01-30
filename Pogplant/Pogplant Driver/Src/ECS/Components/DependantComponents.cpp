#include "../../Pogplant/Src/Model.h"
#include "../../Pogplant/Src/Mesh.h"
#include "../entt_ecs/entt.hpp"

#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

#include "../../Application.h"

#include "DependantComponents.h"

float GetSign(float val)
{
	return val >= 0.0f ? 1.0f : -1.0f;
}

glm::vec3 Components::BoxCollider::GetCenter() const
{
	return (aabb.m_max + aabb.m_min) * 0.5f;
}

glm::vec3 Components::BoxCollider::FindFurthestPoint(const glm::vec3& direction) const
{
	size_t index = 0;
	float maxProjection = std::numeric_limits<float>::lowest();

	for (size_t i = 0; i < 8; ++i)
	{
		float projection = glm::dot(direction, OBBBoxCollider::m_OBBverts[i]);

		if (projection > maxProjection)
		{
			maxProjection = projection;
			index = i;
		}
	}

	return OBBBoxCollider::m_OBBverts[index] * 0.5f + GetCenter();
}

glm::vec3 Components::SphereCollider::GetCenter() const
{
	return sphere.m_pos;
}

glm::vec3 Components::SphereCollider::FindFurthestPoint(const glm::vec3& direction) const
{
	return sphere.m_pos + (glm::normalize(direction) * sphere.m_radius);
}

const glm::vec3 Components::OBBBoxCollider::m_OBBverts[8] =
{
	glm::vec3{1.f, 1.f, 1.f},
	glm::vec3{1.f, 1.f, -1.f},
	glm::vec3{1.f, -1.f, 1.f},
	glm::vec3{1.f, -1.f, -1.f},
	glm::vec3{-1.f, 1.f, 1.f},
	glm::vec3{-1.f, 1.f, -1.f},
	glm::vec3{-1.f, -1.f, 1.f},
	glm::vec3{-1.f, -1.f, -1.f}
};

glm::vec3 Components::OBBBoxCollider::GetCenter() const
{
	return (aabb.m_max + aabb.m_min) * 0.5f;
}

glm::vec3 Components::OBBBoxCollider::FindFurthestPoint(const glm::vec3& direction) const
{
	/*auto& transform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Transform>(m_id);

	glm::vec3 localPoint = centre;

	glm::vec3 localDirection{
		glm::dot(direction, {transform.m_ModelMtx[0].x, transform.m_ModelMtx[0].y, transform.m_ModelMtx[0].z}),
		glm::dot(direction, {transform.m_ModelMtx[1].x, transform.m_ModelMtx[1].y, transform.m_ModelMtx[1].z}),
		glm::dot(direction, {transform.m_ModelMtx[2].x, transform.m_ModelMtx[2].y, transform.m_ModelMtx[2].z})
	};

	localPoint += (GetSign(localDirection.x) * extends.x) * glm::vec3{ transform.m_ModelMtx[0].x, transform.m_ModelMtx[0].y, transform.m_ModelMtx[0].z };
	localPoint += (GetSign(localDirection.y) * extends.y) * glm::vec3{ transform.m_ModelMtx[1].x, transform.m_ModelMtx[1].y, transform.m_ModelMtx[1].z };
	localPoint += (GetSign(localDirection.z) * extends.z) * glm::vec3{ transform.m_ModelMtx[2].x, transform.m_ModelMtx[2].y, transform.m_ModelMtx[2].z };


	return (transform.m_ModelMtx * glm::vec4{
		localPoint.x,
		localPoint.y,
		localPoint.z,
		1.f });//*/

	auto& transform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Transform>(m_id);

	size_t index = 0;
	float maxProjection = std::numeric_limits<float>::lowest();

	glm::mat4 worldToLocal = glm::inverse(transform.m_ModelMtx);

	glm::vec3 localDirection{
		glm::dot(direction, {transform.m_ModelMtx[0].x, transform.m_ModelMtx[0].y, transform.m_ModelMtx[0].z}),
		glm::dot(direction, {transform.m_ModelMtx[1].x, transform.m_ModelMtx[1].y, transform.m_ModelMtx[1].z}),
		glm::dot(direction, {transform.m_ModelMtx[2].x, transform.m_ModelMtx[2].y, transform.m_ModelMtx[2].z})
	};

	for (size_t i = 0; i < 8; ++i)
	{
		float projection = glm::dot(localDirection, m_OBBverts[i]);

		if (projection > maxProjection)
		{
			maxProjection = projection;
			index = i;
		}
	}

	return (transform.m_ModelMtx * glm::scale(glm::mat4{ 1 }, extends) * glm::vec4{
		m_OBBverts[index].x,
		m_OBBverts[index].y,
		m_OBBverts[index].z,
		1.f });
}

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

Components::ScriptVariables::Variable::Variable(const Variable& rhs)
{
	if (!rhs.m_data)
		return;

	UpdateType(rhs.m_type);

	switch (rhs.m_type)
	{
	case Components::ScriptVariables::Variable::Type::FLOAT:
		SetValue<float>(*reinterpret_cast<float*>(rhs.m_data));
		break;
	case Components::ScriptVariables::Variable::Type::INT:
		SetValue<int>(*reinterpret_cast<int*>(rhs.m_data));
		break;
	case Components::ScriptVariables::Variable::Type::BOOL:
		SetValue<bool>(*reinterpret_cast<bool*>(rhs.m_data));
		break;
	case Components::ScriptVariables::Variable::Type::STRING:
		SetValue<std::string>(*reinterpret_cast<std::string*>(rhs.m_data));
		break;
	case Components::ScriptVariables::Variable::Type::VECTOR3:
		SetValue<glm::vec3>(*reinterpret_cast<glm::vec3*>(rhs.m_data));
		break;
	default:
		throw;
	}
}

Components::ScriptVariables::Variable& Components::ScriptVariables::Variable::operator=(const Variable& rhs)
{
	if (&rhs != this && rhs.m_data)
	{
		UpdateType(rhs.m_type);

		switch (rhs.m_type)
		{
		case Components::ScriptVariables::Variable::Type::FLOAT:
			SetValue<float>(*reinterpret_cast<float*>(rhs.m_data));
			break;
		case Components::ScriptVariables::Variable::Type::INT:
			SetValue<int>(*reinterpret_cast<int*>(rhs.m_data));
			break;
		case Components::ScriptVariables::Variable::Type::BOOL:
			SetValue<bool>(*reinterpret_cast<bool*>(rhs.m_data));
			break;
		case Components::ScriptVariables::Variable::Type::STRING:
			SetValue<std::string>(*reinterpret_cast<std::string*>(rhs.m_data));
			break;
		case Components::ScriptVariables::Variable::Type::VECTOR3:
			SetValue<glm::vec3>(*reinterpret_cast<glm::vec3*>(rhs.m_data));
			break;
		default:
			throw;
		}
	}
	return *this;
}
