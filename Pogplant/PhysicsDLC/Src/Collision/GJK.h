/******************************************************************************/
/*!
\file	GJK.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Header defining the GJK support shapes and functions

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef GJK_H_
#define GJK_H_

#include <glm.hpp>
#include <vector>

#include "..\TypeDefines.h"

#define GJK_MAX_ITERATIONS 256

namespace PhysicsDLC::Collision::GJK
{
	/**> Base support shape function*/
	struct GJKSupportShape
	{
		vec3 GetCenter(const glm::mat4& modelMtx, const std::vector<vec3>& vertices) const;
		vec3 FindFurthestPoint(const vec3& direction, const glm::mat4& modelMtx, const std::vector<vec3>& vertices) const;

		virtual vec3 GetCenter() const = 0;
		virtual vec3 FindFurthestPoint(const vec3& direction) const = 0;
	};

	struct GJK
	{
		enum class VoronoiRegionType
		{
			Point0, Point1, Point2, Point3,
			Edge01, Edge02, Edge03, Edge12, Edge13, Edge23,
			Triangle012, Triangle013, Triangle023, Triangle123,
			Tetrahedra0123,
			Unknown
		};

		struct CsoPoint
		{
			vec3 m_point1;
			vec3 m_point2;
			vec3 m_csoPt;
		};

		static bool Intersect(const GJKSupportShape* collider1, const GJKSupportShape* collider2);

	private:
		static VoronoiRegionType IdentifyVoronoiRegionPoint(
			const vec3& q, const vec3& p0,
			size_t& newSize, int newIndices[4],
			vec3& closestPoint, vec3& searchDirection);

		static VoronoiRegionType IdentifyVoronoiRegionEdge(
			const vec3& q, const vec3& p0, const vec3& p1,
			size_t& newSize, int newIndices[4],
			vec3& closestPoint, vec3& searchDirection);

		static VoronoiRegionType IdentifyVoronoiRegionTriangle(
			const vec3& q, const vec3& p0, const vec3& p1, const vec3& p2,
			size_t& newSize, int newIndices[4],
			vec3& closestPoint, vec3& searchDirection);
		
		static VoronoiRegionType IdentifyVoronoiRegionTetrahedron(
			const vec3& q, const vec3& p0, const vec3& p1, const vec3& p2, const vec3& p3,
			size_t& newSize, int newIndices[4],
			vec3& closestPoint, vec3& searchDirection);

		static VoronoiRegionType SetPoint(
			const vec3& q, const vec3& pt,
			size_t& newSize, int newIndices[4],
			vec3& closestPoint, vec3& searchDirection, GJK::VoronoiRegionType type);

		static VoronoiRegionType SetEdge(
			const vec3& q, const vec3& pt0, const vec3& pt1,
			size_t& newSize, int newIndices[4], float u, float v,
			vec3& closestPoint, vec3& searchDirection, GJK::VoronoiRegionType type);

		static VoronoiRegionType SetTriangle(
			const vec3& q, const vec3& pt0, const vec3& pt1, const vec3& pt2, const vec3& pt3,
			size_t& newSize, int newIndices[4], float u, float v, float w,
			vec3& closestPoint, vec3& searchDirection, GJK::VoronoiRegionType type);

		inline static float CheckTriangleSide(const vec3& q, const vec3& p0, const vec3& p1, const vec3& p2, const vec3& p3)
		{
			vec3 normal = glm::cross((p1 - p0), (p2 - p0));

			if (glm::dot(normal, (p3 - p0)) > 0)
			{
				normal = -normal;
			}

			return glm::dot(normal, (q - p0));
		}

		inline static CsoPoint ComputeSupport(const GJKSupportShape* collider1, const GJKSupportShape* collider2, const vec3& searchDirection)
		{
			CsoPoint result;

			result.m_point1 = collider1->FindFurthestPoint(searchDirection);
			result.m_point2 = collider2->FindFurthestPoint(-searchDirection);
			result.m_csoPt = result.m_point1 - result.m_point2;

			return result;
		}
	};

}

#endif