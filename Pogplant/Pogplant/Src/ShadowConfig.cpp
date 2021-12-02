#include "ShadowConfig.h"

namespace Pogplant
{
	glm::mat4 ShadowCFG::m_LightProj = glm::mat4{ 1 };
	int ShadowCFG::m_ShadowMapW = 8192;
	int ShadowCFG::m_ShadowMapH = 8192;
}