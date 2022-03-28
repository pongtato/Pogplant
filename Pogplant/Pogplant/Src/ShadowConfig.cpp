#include "ShadowConfig.h"

namespace Pogplant
{
	std::vector<float> ShadowCFG::m_CascadeIntervals = { 3.0f, 2.0f, 1.0f, 1.0f };
	int ShadowCFG::m_ShadowMapW = 4096;
	int ShadowCFG::m_ShadowMapH = 4096;
}