#include "DebugDraw.h"
#include <gtc/constants.hpp>

#include <iostream>
namespace Pogplant
{
	std::vector<glm::vec3> DebugDraw::m_DebugVerts;
	float DebugDraw::m_LineWidth = 3.0f;

	void DebugDraw::NewFrame()
	{
		m_DebugVerts.clear();
	}

	void DebugDraw::DebugLine(glm::vec3 _Position1, glm::vec3 _Position2)
	{
		m_DebugVerts.push_back(_Position1);
		m_DebugVerts.push_back(_Position2);
	}

	void DebugDraw::DebugCircle(glm::vec3 _Position, float _Radius, glm::vec3 _Axis1, glm::vec3 _Axis2, size_t _Segments)
	{
		float increment = glm::two_pi<float>() / _Segments;

		// Index to refer to for first circle position
		size_t currIndex = m_DebugVerts.size();
		glm::vec3 prevVert = glm::vec3{ 0 };
		// Iterate up until before last segment
		for (int i = 0; i < _Segments; i++)
		{
			const float currAngle = i * increment;
			
			glm::vec3 vert = _Position + CircleWrapper(currAngle, _Radius, _Axis1, _Axis2);
			
			if (i > 0)
			{
				DebugLine(prevVert, vert);
			}
			prevVert = vert;
		}
		// Complete circle, last vert to first vert
		DebugLine(prevVert, m_DebugVerts[currIndex]);
	}

	void DebugDraw::DebugCube(glm::vec3 _Min, glm::vec3 _Max)
	{
		// 8 verts of the cube
		const glm::vec3 v0 = _Min;
		const glm::vec3 v1 = glm::vec3{ _Min.x, _Min.y, _Max.z };
		const glm::vec3 v2 = glm::vec3{ _Max.x, _Min.y, _Max.z };
		const glm::vec3 v3 = glm::vec3{ _Max.x, _Min.y, _Min.z };

		const glm::vec3 v4 = glm::vec3{ _Min.x, _Max.y, _Min.z };
		const glm::vec3 v5 = glm::vec3{ _Min.x, _Max.y, _Max.z };
		const glm::vec3 v6 = _Max;
		const glm::vec3 v7 = glm::vec3{ _Max.x, _Max.y, _Min.z };

		// Bottom plane
		DebugLine(v0, v1);
		DebugLine(v1, v2);
		DebugLine(v2, v3);
		DebugLine(v3, v0);

		// Top plane
		DebugLine(v4, v5);
		DebugLine(v5, v6);
		DebugLine(v6, v7);
		DebugLine(v7, v4);

		// Connect bottom plane to top plane
		DebugLine(v0, v4);
		DebugLine(v1, v5);
		DebugLine(v2, v6);
		DebugLine(v3, v7);
	}

	void DebugDraw::DebugSphere(glm::vec3 _Position, glm::vec3 _CamDirection, float _Radius, size_t _Segments)
	{
		DebugCircle(_Position, _Radius, { 1,0,0 }, { 0,1,0 }, _Segments);
		DebugCircle(_Position, _Radius, { 1,0,0 }, { 0,0,1 }, _Segments);
		DebugCircle(_Position, _Radius, { 0,1,0 }, { 0,0,1 }, _Segments);

		// View circle
		const float d = glm::length(_CamDirection);
		const float l = sqrtf(d * d - _Radius * _Radius);
		// r prime
		const float r_p = (_Radius * l) / d;
		const float z = sqrtf(_Radius * _Radius - r_p * r_p);
		const glm::vec3 norm_dir = glm::normalize(_CamDirection);
		// c prime
		const glm::vec3 c_p = _Position - norm_dir * z;

		// Perpendicular vectors for disc
		glm::vec3 right = glm::vec3{ 0 };
		glm::vec3 up = glm::vec3{ 0 };
		right = glm::cross(norm_dir, glm::vec3{ 0.0f,1.0f,0.0f });
		// This occurs if you cross against another vector that is 0,1,0
		if (glm::length(right) > 0)
		{
			right = glm::normalize(right);
		}
		// If that is the case assume right to be 0,0,1
		else
		{
			right = glm::vec3{ 0.0f, 0.0f, 1.0f };
		}
		up = glm::normalize(glm::cross(right, _CamDirection));

		DebugCircle(_Position, _Radius, right, up, _Segments);
	}

	glm::vec3 DebugDraw::CircleWrapper(float _Angle, float _Radius, glm::vec3 _Axis1, glm::vec3 _Axis2)
	{
		return _Radius * (cosf(_Angle) * _Axis2 + sinf(_Angle) * _Axis1);
	}
}