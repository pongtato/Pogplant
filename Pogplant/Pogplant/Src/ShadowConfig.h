#pragma once

#include <glm.hpp>

namespace Pogplant
{
	struct ShadowCFG
	{
		static glm::mat4 m_LightProj;
		static int m_ShadowMapW;
		static int m_ShadowMapH;
	};
}