#include "Gizmos.h"

namespace Pogplant
{
	void Gizmos::InitGizmos()
	{
		ThreeDInspector::Init3DInspector(glm::vec3{ 0.0f,0.0f,0.0f }, glm::vec3{ 2.0f,2.0f,2.0f });
	}
}