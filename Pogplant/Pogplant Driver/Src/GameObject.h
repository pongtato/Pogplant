#pragma once

#include <glm.hpp>

namespace Pogplant
{
	struct RenderObject;
}

class GameObject
{
public:
	GameObject(glm::vec3 _Pos, glm::vec3 _Rot, glm::vec3 _Scale, Pogplant::RenderObject* _RenderObject);

	float m_Position[3];
	float m_Rotation[3];
	float m_Scale[3];

	// This is for the gizmo to edit, have to update
	float m_ModelMtx[16];
	Pogplant::RenderObject* m_RenderObject;
};