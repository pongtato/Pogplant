/******************************************************************************/
/*!
\file	Colliders.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Header defining collision shape/collider types

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef COLLIDERS_H_
#define COLLIDERS_H_

#include <glm.hpp>
#include <vector>

#include "..\TypeDefines.h"

namespace PhysicsDLC
{
	namespace Collision
	{
		namespace Shapes
		{
			struct Ray
			{
				Ray();
				Ray(vec3 start, vec3 dir);
				~Ray() = default;

				vec3 m_start;
				vec3 m_dir;
			};

			struct Sphere
			{
				Sphere();
				Sphere(const vec3& pos, floatType radius);
				~Sphere() = default;

				void SetCentroid(const std::vector<vec3>& points);
				void SetRitter(const std::vector<vec3>& points);

				vec3 m_pos;
				floatType m_radius;
			};

			struct AABB
			{
				AABB();
				AABB(const vec3& min, const vec3& max);

				vec3 m_min;
				vec3 m_max;

				void FattenAABB(float multiplier);
				void CalculateAABBFromExtends(const vec3& position, const vec3& extends);
				float GetSurfaceArea() const;
				float GetVolume() const;
				bool Contains(const AABB& aabb) const;

				static AABB Combine(const AABB& aabb1, const AABB& aabb2);
			};

			struct OBB
			{
				vec3 m_pos;
				vec3 m_extendX;
				vec3 m_extendY;
				vec3 m_extendZ;

				AABB CalculateAABB();
			};

			struct Plane
			{
				Plane();
				Plane(vec3 pos, vec3 normal);

				vec3 m_pos;
				vec3 m_normal;
			};

			struct HeightMap
			{
				std::vector<unsigned char> m_Heightmap;
				size_t m_HeightMapDim;

				float GetHeight(const vec3& _Position);
				float Barycentric(const vec3& _V1, const vec3& _V2, const vec3& _V3, const vec3& _Position) const;
			};
		}
	}
}

#endif // !COLLIDERS_H_
