#pragma once

#include "Model.h"

#include <glm.hpp>
#include <string>

namespace Pogplant
{
	struct RenderObject
	{
		glm::mat4 m_Model;
		Model* m_RenderModel;
	};
}