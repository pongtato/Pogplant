#include "3DInspector.h"
#include <gtc/matrix_transform.hpp>

namespace Pogplant
{
	std::vector<Plane> Cube::m_Planes;
	Cube ThreeDInspector::m_Cube;
	std::vector<OBB> ThreeDInspector::m_InspectorOBBs;

	void ThreeDInspector::Init3DInspector(glm::vec3 _Position, glm::vec3 _Scale)
	{
		glm::mat4 Parent = glm::mat4{ 1 };
		Parent = glm::translate(Parent, _Position);
		Parent = glm::scale(Parent, _Scale);

		/// To form a cube for orientation
		// Front
		glm::mat4 Model = glm::translate(Parent, glm::vec3(0.0f, 0.0f, 0.4f));
		m_Cube.m_Planes.push_back({ Model,glm::vec4{0.0f,0.0f,1.0f,1.0f} });

		// Right
		Model = glm::translate(Parent, glm::vec3(0.4f, 0, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 0, 1, 0 });
		m_Cube.m_Planes.push_back({ Model,glm::vec4{1.0f,0.0f,0.0f,1.0f} });

		// Left
		Model = glm::translate(Parent, glm::vec3(-0.6f, 0, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 0, 1, 0 });
		m_Cube.m_Planes.push_back({ Model,glm::vec4{1.0f,0.2f,0.6f,1.0f} });

		// Back
		Model = glm::translate(Parent, glm::vec3(0.0f, 0.0f, -0.6f));
		m_Cube.m_Planes.push_back({ Model,glm::vec4{0.2f,0.6f,1.0f,1.0f} });

		// Top
		Model = glm::translate(Parent, glm::vec3(0.0f, 0.6f, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 1, 0, 0 });
		m_Cube.m_Planes.push_back({ Model,glm::vec4{0.0f,1.0f,0.0f,1.0f} });

		// Bottom
		Model = glm::translate(Parent, glm::vec3(0.0f, -0.4f, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 1, 0, 0 });
		m_Cube.m_Planes.push_back({ Model,glm::vec4{0.6f,1.0f,0.2f,1.0f} });
	}
}
