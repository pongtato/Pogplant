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
#include <gtx/norm.hpp>

using namespace PhysicsDLC::Collision::Shapes;

Ray::Ray()
{
}

Ray::Ray(vec3 start, vec3 dir)
	:
	m_start{ start },
	m_dir{ dir }
{
}

Sphere::Sphere()
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
{
}

AABB::AABB(vec3 min, vec3 max)
	:
	m_min{ min },
	m_max{ max }
{

}

Plane::Plane()
{
}

Plane::Plane(vec3 pos, vec3 normal)
	:
	m_pos{ pos },
	m_normal{ normal }
{
}
