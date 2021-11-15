/******************************************************************************/
/*!
\file	Colliders.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	This file contains implementation of the collision shape/collider types'
	internal functions

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "Colliders.h"
#include <algorithm>
#include <gtx/norm.hpp>

using namespace PhysicsDLC::Collision::Shapes;

Ray::Ray()
	:
	m_start{ 0.f, 0.f, 0.f },
	m_dir{ 0.f, 0.f, 1.f }
{
}

Ray::Ray(vec3 start, vec3 dir)
	:
	m_start{ start },
	m_dir{ dir }
{
}

Sphere::Sphere()
	:
	m_pos{ 0.f, 0.f, 0.f },
	m_radius{ 1.f }
{
}

Sphere::Sphere(const vec3& pos, floatType radius)
	:
	m_pos{ pos },
	m_radius{ radius }
{
}

void Sphere::SetCentroid(const std::vector<vec3>& points)
{
	if (points.empty())
		return;

	vec3 min{ points[0] };
	vec3 max{ points[0] };

	for (auto it = points.begin(); it != points.end(); ++it)
	{
		if (min.x > (*it).x)
			min.x = (*it).x;
		if (min.y > (*it).y)
			min.y = (*it).y;
		if (min.z > (*it).z)
			min.z = (*it).z;

		if (max.x < (*it).x)
			max.x = (*it).x;
		if (max.y < (*it).y)
			max.y = (*it).y;
		if (max.z < (*it).z)
			max.z = (*it).z;
	}

	m_pos = (max + min) * 0.5f;
	m_radius = 0.f;

	for (auto it = points.begin(); it != points.end(); ++it)
	{
		float radius = glm::length2((m_pos - (*it)));

		if (radius > m_radius)
			m_radius = radius;
	}

	m_radius = static_cast<float>(sqrt(m_radius));
}

void Sphere::SetRitter(const std::vector<vec3>& points)
{
	vec3 xmin = points[0],
		xmax = points[0],
		ymin = points[0],
		ymax = points[0],
		zmin = points[0],
		zmax = points[0];

	for (auto it = points.begin(); it != points.end(); ++it)
	{
		if ((*it).x < xmin.x)
			xmin = (*it);
		if ((*it).x > xmax.x)
			xmax = (*it);

		if ((*it).y < ymin.y)
			ymin = (*it);
		if ((*it).y > ymax.y)
			ymax = (*it);

		if ((*it).z < zmin.z)
			zmin = (*it);
		if ((*it).z > zmax.z)
			zmax = (*it);
	}
	vec3 span;

	vec3 delta{
		xmax.x - xmin.x,
		xmax.y - xmin.y,
		xmax.z - xmin.z
	};
	span.x = glm::length2(delta);

	delta = {
		ymax.x - ymin.x,
		ymax.y - ymin.y,
		ymax.z - ymin.z
	};

	span.y = glm::length2(delta);

	delta = {
		zmax.x - zmin.x,
		zmax.y - zmin.y,
		zmax.z - zmin.z
	};

	span.z = glm::length2(delta);

	vec3 pt1 = xmin, pt2 = xmax;
	float spanMax = span.x;

	if (span.y > spanMax)
	{
		spanMax = span.y;
		pt1 = ymin;
		pt2 = ymax;
	}
	if (span.z > spanMax)
	{
		pt1 = zmin;
		pt2 = zmax;
	}

	vec3 centerPt{
		(pt1.x + pt2.x) * 0.5f,
		(pt1.y + pt2.y) * 0.5f,
		(pt1.z + pt2.z) * 0.5f
	};

	delta = { pt2.x - centerPt.x,
		pt2.y - centerPt.y,
		pt2.z - centerPt.z
	};

	float radiusSqrt = glm::length2(delta);
	float radius = static_cast<float>(sqrt(radiusSqrt));

	//2nd pass
	for (size_t i = 0; i < points.size(); i++)
	{
		delta = points[i] - centerPt;
		float distSqr = glm::length2(delta);

		if (distSqr > radiusSqrt)
		{
			centerPt = 0.5f *
				(centerPt - (radius * glm::normalize(delta)) + points[i]);

			//Calculate new radius
			radius = glm::length(points[i] - centerPt);
			radiusSqrt = radius * radius;
		}
	}

	m_pos = centerPt;
	m_radius = radius;
}

AABB::AABB()
	:
	m_min{ -1.f, -1.f, -1.f },
	m_max{ 1.f, 1.f, 1.f }
{
}

AABB::AABB(vec3 min, vec3 max)
	:
	m_min{ min },
	m_max{ max }
{

}

void AABB::CalculateAABBFromExtends(const vec3& position, const vec3& extends)
{
	m_min = position - extends;
	m_max = position + extends;
}

AABB OBB::CalculateAABB()
{
	AABB aabb;

	aabb.m_min = m_pos;
	aabb.m_max = m_pos;

	vec3 edgeX = m_pos + m_extendX;
	vec3 edgeY = m_pos + m_extendY;
	vec3 edgeZ = m_pos + m_extendZ;

	aabb.m_min.x = std::min<float>({aabb.m_min.x, edgeX.x, edgeY.x, edgeZ.z});
	aabb.m_min.y = std::min<float>({aabb.m_min.y, edgeX.y, edgeY.y, edgeZ.y});
	aabb.m_min.z = std::min<float>({aabb.m_min.z, edgeX.z, edgeY.z, edgeZ.z});

	aabb.m_max.x = std::max<float>({ aabb.m_max.x, edgeX.x, edgeY.x, edgeZ.z });
	aabb.m_max.y = std::max<float>({ aabb.m_max.y, edgeX.y, edgeY.y, edgeZ.y });
	aabb.m_max.z = std::max<float>({ aabb.m_max.z, edgeX.z, edgeY.z, edgeZ.z });

	return aabb;
}

Plane::Plane()
	:
	m_pos{ 0.f, 0.f, 0.f },
	m_normal{ 0.f, 1.f, 0.f }
{
}

Plane::Plane(vec3 pos, vec3 normal)
	:
	m_pos{ pos },
	m_normal{ normal }
{
}

/******************************************************************************/
/*!
\brief
	Functions done by Aloysius
*/
/******************************************************************************/
float HeightMap::GetHeight(const vec3& _Position)
{
	if (m_Heightmap.size() == 0)
	{
		return 0;
	}

	if (_Position.x < -0.5f || _Position.x > 0.5f || _Position.z < -0.5f || _Position.z > 0.5f)
	{
		return 0;
	}

	float gridSquareSize = 1.f / m_HeightMapDim;

	size_t gridX = static_cast<size_t>((_Position.x + 0.5f) / gridSquareSize);
	size_t gridZ = static_cast<size_t>((_Position.z + 0.5f) / gridSquareSize);

	float xCoord = fmod((_Position.x + 0.5f), gridSquareSize) / gridSquareSize;
	float zCoord = fmod((_Position.z + 0.5f), gridSquareSize) / gridSquareSize;

	float HeightmapReturn = 0.f;

	size_t var1 = xCoord <= (1 - zCoord) ? 0 : 1;
	HeightmapReturn = Barycentric
	(
		glm::vec3(var1, m_Heightmap[gridZ * m_HeightMapDim + gridX + var1], 0),
		glm::vec3(1, m_Heightmap[(gridZ + var1) * m_HeightMapDim + gridX + 1], var1),
		glm::vec3(0, m_Heightmap[(gridZ + 1) * m_HeightMapDim + gridX], 1),
		glm::vec3(xCoord, 0, zCoord)
	);

	return HeightmapReturn / m_HeightMapDim;
}

/******************************************************************************/
/*!
\brief
	Functions done by Aloysius
*/
/******************************************************************************/
float HeightMap::Barycentric(const vec3& _V1, const vec3& _V2, const vec3& _V3, const vec3& _Position) const
{
	float det = (_V2.z - _V3.z) * (_V1.x - _V3.x) + (_V3.x - _V2.x) * (_V1.z - _V3.z);

	float l1 = ((_V2.z - _V3.z) * (_Position.x - _V3.x) + (_V3.x - _V2.x) * (_Position.z - _V3.z)) / det;

	float l2 = ((_V3.z - _V1.z) * (_Position.x - _V3.x) + (_V1.x - _V3.x) * (_Position.z - _V3.z)) / det;

	float l3 = 1.0f - l1 - l2;

	return l1 * _V1.y + l2 * _V2.y + l3 * _V3.y;
}
