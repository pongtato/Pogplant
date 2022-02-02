#include "ShadowConfig.h"

namespace Pogplant
{
	std::vector<float> ShadowCFG::m_CascadeIntervals = { 2.0f, 1.5f, 1.25f, 1.0f };
	int ShadowCFG::m_ShadowMapW = 8192;
	int ShadowCFG::m_ShadowMapH = 8192;
}