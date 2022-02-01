#include "ShadowConfig.h"

namespace Pogplant
{
	glm::mat4 ShadowCFG::m_LightProj = glm::mat4{ 1 };
	std::vector<float> ShadowCFG::m_CascadeIntervals = { 10.0f, 5.0f, 2.0f, 1.0f };
	int ShadowCFG::m_ShadowMapW = 8192;
	int ShadowCFG::m_ShadowMapH = 8192;
}