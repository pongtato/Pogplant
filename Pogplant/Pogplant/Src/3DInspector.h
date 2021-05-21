#pragma once

#include "OBB.h"
#include <glm.hpp>
#include <vector>

namespace Pogplant
{
	struct Plane
	{
		glm::mat4 m_Model;
		glm::vec4 m_Color;
	};

	struct Cube
	{
		static std::vector<Plane> m_Planes;
	};

	struct ThreeDInspector
	{
		static void Init3DInspector(glm::vec3 _Position, glm::vec3 _Scale);

		static Cube m_Cube;
		static std::vector<OBB> m_InspectorOBBs;
	};
}