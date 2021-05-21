#include "OBB.h"
#include <algorithm>

namespace Pogplant
{
	bool OBB::RayObb(const Ray& _Ray)
	{
		glm::vec3 pos = { m_Model[3].x,m_Model[3].y, m_Model[3].z };
		glm::vec3 dirFromRay = pos - _Ray.m_Origin;
		glm::vec3 xAxis{ m_Model[0].x,m_Model[1].y,m_Model[0].z };
		float e = glm::dot(xAxis, dirFromRay);
		float f = glm::dot(_Ray.m_Direction, dirFromRay);

		if (fabsf(f) > 0.001f)
		{
			float tMax = INFINITY;
			float tMin = -INFINITY;
			float t1 = (e + m_Min.x) / f;
			float t2 = (e + m_Max.x) / f;

			if (t1 > t2)
			{
				std::swap(t1, t2);
			}
			tMax = t2 < tMax ? t2 : tMax;
			tMin = t1 > tMin ? t1 : tMin;

			if (tMax < tMin)
			{
				return false;
			}
		}
		else if (-e + m_Min.x > 0.0f || -e + m_Max.x < 0.0f)
		{
			return false;
		}

		return true;
	}
}