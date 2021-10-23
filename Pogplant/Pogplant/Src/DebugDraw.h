#pragma once

#include <glm.hpp>
#include <vector>

namespace Pogplant
{
	struct DebugDraw
	{
		static void NewFrame();
		static void DebugLine(glm::vec3 _Position1, glm::vec3 _Position2);
		static void DebugCircle(glm::vec3 _Position, float _Radius, glm::vec3 _Axis1, glm::vec3 _Axis2, size_t _Segments = 36);
		static void DebugCube(glm::vec3 _Min, glm::vec3 _Max);
		static void DebugSphere(glm::vec3 _Position, glm::vec3 _CamDirection, float _Radius, size_t _Segments = 36);
		static void DebugFrustum(glm::vec3 _Position, float _Zoom, float _Aspect, float _Near, float _Far, glm::vec3 _Right, glm::vec3 _Up, glm::vec3 _Front);

		static std::vector<glm::vec3> m_DebugVerts;
		static float m_LineWidth;
		static float m_GridWidth;
		static float m_GridIntervalWidth;
		static int m_GridInterval;

	private:
		static glm::vec3 CircleWrapper(float _Angle, float _Radius, glm::vec3 _Axis1, glm::vec3 _Axis2);
	};
}