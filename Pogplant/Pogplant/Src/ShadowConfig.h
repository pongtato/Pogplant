#pragma once

#include <glm.hpp>
#include <vector>

namespace Pogplant
{
	struct ShadowCFG
	{
		static glm::mat4 m_LightProj;
		static std::vector<float> m_CascadeIntervals;
		static int m_ShadowMapW;
		static int m_ShadowMapH;
	};
}