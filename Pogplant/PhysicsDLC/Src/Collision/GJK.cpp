/******************************************************************************/
/*!
\file	GJK.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	This file contains implementation of the GJK algorithm and its support
	shape functions

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "Collision.h"
#include "GJK.h"
#include "..\Utility.h"

namespace PhysicsDLC::Collision::GJK
{
	vec3 GJKSupportShape::GetCenter(const glm::mat4& modelMtx, const std::vector<vec3>& vertices) const
	{
		glm::vec3 average{ 0.f, 0.f, 0.f };

		for (auto& vertex : vertices)
			average += vertex;

		average = average * (1.f / (float)vertices.size());

		return (modelMtx * glm::vec4{ average.x, average.y, average.z, 1.0f });
	}

	vec3 GJKSupportShape::FindFurthestPoint(const vec3& direction, const glm::mat4& modelMtx, const std::vector<vec3>& vertices) const
	{
		size_t index = 0;
		float maxProjection = std::numeric_limits<float>::lowest();

		glm::mat4 worldToLocal = glm::inverse(modelMtx);

		vec3 localDirection{
			glm::dot(direction, {modelMtx[0].x, modelMtx[0].y, modelMtx[0].z}),
			glm::dot(direction, {modelMtx[1].x, modelMtx[1].y, modelMtx[1].z}),
			glm::dot(direction, {modelMtx[2].x, modelMtx[2].y, modelMtx[2].z})
		};

		for (size_t i = 0; i < vertices.size(); ++i)
		{
			float projection = glm::dot(localDirection, vertices[i]);

			if (projection > maxProjection)
			{
				maxProjection = projection;
				index = i;
			}
		}

		glm::vec4 pos{ vertices[index].x, vertices[index].y, vertices[index].z, 0.f };

		return (modelMtx * pos);
	}

	GJK::VoronoiRegionType GJK::SetPoint(
		const vec3& q, const vec3& pt,
		size_t& newSize, int newIndices[4],
		vec3& closestPoint, vec3& searchDirection, GJK::VoronoiRegionType type)
	{
		newSize = 1;

		closestPoint = pt;

		switch (type)
		{
		case VoronoiRegionType::Point0:
			newIndices[0] = 0;
			break;
		case VoronoiRegionType::Point1:
			newIndices[0] = 1;
			break;
		case VoronoiRegionType::Point2:
			newIndices[0] = 2;
			break;
		case VoronoiRegionType::Point3:
			newIndices[0] = 3;
			break;
		default:
			assert(false);
		}

		searchDirection = q - closestPoint;

		return type;
	}

	CollisionResults GJK::Intersect(const GJKSupportShape* collider1, const GJKSupportShape* collider2)
	{
		CollisionResults collisionResult;
		std::vector<CsoPoint> simplex;
		simplex.reserve(5);

		vec3 q{ 0.f, 0.f, 0.f };

		vec3 searchDir = collider1->GetCenter() - collider2->GetCenter();

		if (glm::dot(searchDir, searchDir) < std::numeric_limits<float>::epsilon())
			searchDir = glm::vec3{ -1.f, 0.f, 0.f };
		else
			glm::normalize(searchDir);

		simplex.push_back(ComputeSupport(collider1, collider2, searchDir));

		size_t newSize = 0;
		int newIndices[4];
		vec3 closestPt{ 0.f, 0.f, 0.f };

		for (size_t i = 0; i < GJK_MAX_ITERATIONS; i++)
		{
			//If hit max iterations please double check
			//assert(i != GJK_MAX_ITERATIONS - 1);

			VoronoiRegionType currentType = VoronoiRegionType::Unknown;

			switch (simplex.size())
			{
			case 1:
				currentType = IdentifyVoronoiRegionPoint(q, simplex[0].m_csoPt, newSize, newIndices, closestPt, searchDir);
				break;
			case 2:
				currentType = IdentifyVoronoiRegionEdge(q, simplex[0].m_csoPt, simplex[1].m_csoPt, newSize, newIndices, closestPt, searchDir);
				break;
			case 3:
				currentType = IdentifyVoronoiRegionTriangle(q, simplex[0].m_csoPt, simplex[1].m_csoPt, simplex[2].m_csoPt, newSize, newIndices, closestPt, searchDir);
				break;
			case 4:
				currentType = IdentifyVoronoiRegionTetrahedron(q, simplex[0].m_csoPt, simplex[1].m_csoPt, simplex[2].m_csoPt, simplex[3].m_csoPt, newSize, newIndices, closestPt, searchDir);
				break;
			default:
				assert(false && "You done goofed");
			}

			if(currentType == VoronoiRegionType::Unknown)
			{
				collisionResult.collided = false;
				return collisionResult;
			}

			//Intersection
			if (glm::dot(closestPt, closestPt) < std::numeric_limits<float>::epsilon())
			{
				collisionResult.collided = true;
				return collisionResult;
			}

			//If search direction is invalid
			if (glm::dot(searchDir, searchDir) < std::numeric_limits<float>::epsilon())
			{
				collisionResult.collided = false;
				return collisionResult;
			}

			//Get new simplex after getting the new "selected" point for the simplex
			if (newSize < simplex.size())
			{
				std::vector<CsoPoint> newSimplex;
				newSimplex.reserve(newSize);

				for (size_t j = 0; j < newSize; j++)
				{
					newSimplex.push_back(simplex[newIndices[j]]);
				}

				simplex = std::move(newSimplex);
			}

			glm::normalize(searchDir);

			simplex.push_back(ComputeSupport(collider1, collider2, searchDir));

			if (glm::dot(searchDir, (simplex.back().m_csoPt - closestPt)) <= std::numeric_limits<float>::epsilon())
			{
				//If the search direction does not exceed origin, means already not possible to reach
				//So no collision
				collisionResult.collided = false;
				return collisionResult;
			}
		}
		
		collisionResult.collided = false;
		return collisionResult;
	}

	GJK::VoronoiRegionType GJK::SetEdge(
		const vec3& q, const vec3& pt0, const vec3& pt1,
		size_t& newSize, int newIndices[4], float u, float v,
		vec3& closestPoint, vec3& searchDirection, GJK::VoronoiRegionType type)
	{
		newSize = 2;

		closestPoint = u * pt0 + v * pt1;

		switch (type)
		{
		case VoronoiRegionType::Edge01:
			newIndices[0] = 0;
			newIndices[1] = 1;
			break;
		case VoronoiRegionType::Edge02:
			newIndices[0] = 0;
			newIndices[1] = 2;
			break;
		case VoronoiRegionType::Edge03:
			newIndices[0] = 0;
			newIndices[1] = 3;
			break;
		case VoronoiRegionType::Edge12:
			newIndices[0] = 1;
			newIndices[1] = 2;
			break;
		case VoronoiRegionType::Edge13:
			newIndices[0] = 1;
			newIndices[1] = 3;
			break;
		case VoronoiRegionType::Edge23:
			newIndices[0] = 2;
			newIndices[1] = 3;
			break;
		default:
			assert(false);
		}

		searchDirection = q - closestPoint;

		return type;
	}

	GJK::VoronoiRegionType GJK::SetTriangle(
		const vec3& q, const vec3& pt0, const vec3& pt1, const vec3& pt2, const vec3& pt3,
		size_t& newSize, int newIndices[4], float u, float v, float w,
		vec3& closestPoint, vec3& searchDirection, GJK::VoronoiRegionType type)
	{
		newSize = 3;

		switch (type)
		{
		case VoronoiRegionType::Triangle012:
			newIndices[0] = 0;
			newIndices[1] = 1;
			newIndices[2] = 2;
			closestPoint = u * pt0 + v * pt1 + w * pt2;
			break;
		case VoronoiRegionType::Triangle013:
			newIndices[0] = 0;
			newIndices[1] = 1;
			newIndices[2] = 3;
			closestPoint = u * pt0 + v * pt1 + w * pt3;
			break;
		case VoronoiRegionType::Triangle023:
			newIndices[0] = 0;
			newIndices[1] = 2;
			newIndices[2] = 3;
			closestPoint = u * pt0 + v * pt2 + w * pt3;
			break;
		case VoronoiRegionType::Triangle123:
			newIndices[0] = 1;
			newIndices[1] = 2;
			newIndices[2] = 3;
			closestPoint = u * pt1 + v * pt2 + w * pt3;
			break;
		default:
			assert(false);
		}

		searchDirection = q - closestPoint;

		return type;
	}

	GJK::VoronoiRegionType GJK::IdentifyVoronoiRegionPoint(const vec3& q, const vec3& p0, size_t& newSize, int newIndices[4], vec3& closestPoint, vec3& searchDirection)
	{
		newSize = 1;
		closestPoint = p0;
		newIndices[0] = 0;

		searchDirection = q - closestPoint;

		return VoronoiRegionType::Point0;
	}

	GJK::VoronoiRegionType GJK::IdentifyVoronoiRegionEdge(const vec3& q, const vec3& p0, const vec3& p1, size_t& newSize, int newIndices[4], vec3& closestPoint, vec3& searchDirection)
	{
		float u{ 0.f }, v{ 0.f };
		Utility::BarycentricCoordsLine(q, p0, p1, u, v);

		if (v <= 0)
			return SetPoint(q, p0, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point0);
		else if (u <= 0)
			return SetPoint(q, p1, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point1);

		return SetEdge(q, p0, p1, newSize, newIndices, u, v, closestPoint, searchDirection, VoronoiRegionType::Edge01);
	}

	GJK::VoronoiRegionType GJK::IdentifyVoronoiRegionTriangle(const vec3& q, const vec3& p0, const vec3& p1, const vec3& p2, size_t& newSize, int newIndices[4], vec3& closestPoint, vec3& searchDirection)
	{
		float u01{ 0.f }, v01{ 0.f }, u12{ 0.f }, v12{ 0.f }, u20{ 0.f }, v20{ 0.f };
		Utility::BarycentricCoordsLine(q, p0, p1, u01, v01);
		Utility::BarycentricCoordsLine(q, p1, p2, u12, v12);
		Utility::BarycentricCoordsLine(q, p2, p0, u20, v20);


		if (v01 <= 0 && u20 <= 0)
			return SetPoint(q, p0, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point0);
		else if (v12 <= 0 && u01 <= 0)
			return SetPoint(q, p1, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point1);
		else if (v20 <= 0 && u12 <= 0)
			return SetPoint(q, p2, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point2);

		float u{ 0.f }, v{ 0.f }, w{ 0.f };
		if (!Utility::BarycentricCoordsTriangle(q, p0, p1, p2, u, v, w))
			return VoronoiRegionType::Unknown;

		if (u > 0 && v > 0 && w > 0)
			return SetTriangle(q, p0, p1, p2, p2, newSize, newIndices, u, v, w, closestPoint, searchDirection, VoronoiRegionType::Triangle012);
		else if (w < 0 && u01 > 0 && v01 > 0)
			return SetEdge(q, p0, p1, newSize, newIndices, u01, v01, closestPoint, searchDirection, VoronoiRegionType::Edge01);
		else if (v < 0 && u20 > 0 && v20 > 0)
			return SetEdge(q, p2, p0, newSize, newIndices, u20, v20, closestPoint, searchDirection, VoronoiRegionType::Edge02);

		return SetEdge(q, p1, p2, newSize, newIndices, u12, v12, closestPoint, searchDirection, VoronoiRegionType::Edge12);
	}

	GJK::VoronoiRegionType GJK::IdentifyVoronoiRegionTetrahedron(const vec3& q, const vec3& p0, const vec3& p1, const vec3& p2, const vec3& p3, size_t& newSize, int newIndices[4], vec3& closestPoint, vec3& searchDirection)
	{
		float u01{ 0.f }, v01{ 0.f }, u02{ 0.f }, v02{ 0.f }, u03{ 0.f }, v03{ 0.f },
			u12{ 0.f }, v12{ 0.f }, u13{ 0.f }, v13{ 0.f }, u23{ 0.f }, v23{ 0.f };

		Utility::BarycentricCoordsLine(q, p0, p1, u01, v01);
		Utility::BarycentricCoordsLine(q, p0, p2, u02, v02);
		Utility::BarycentricCoordsLine(q, p0, p3, u03, v03);
		Utility::BarycentricCoordsLine(q, p1, p2, u12, v12);
		Utility::BarycentricCoordsLine(q, p1, p3, u13, v13);
		Utility::BarycentricCoordsLine(q, p2, p3, u23, v23);


		if (v01 < 0 && v02 < 0 && v03 < 0)
			return SetPoint(q, p0, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point0);
		else if (u01 < 0 && v12 < 0 && v13 < 0)
			return SetPoint(q, p1, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point1);
		else if (u02 < 0 && u12 < 0 && v23 < 0)
			return SetPoint(q, p2, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point2);
		else if (u03 < 0 && u23 < 0 && u13 < 0)
			return SetPoint(q, p3, newSize, newIndices, closestPoint, searchDirection, VoronoiRegionType::Point3);

		vec3 uvw012{ 0.f }, uvw013{ 0.f }, uvw023{ 0.f }, uvw123{ 0.f };

		bool isValid = Utility::BarycentricCoordsTriangle(q, p0, p1, p2, uvw012.x, uvw012.y, uvw012.z);
		isValid &= Utility::BarycentricCoordsTriangle(q, p0, p1, p3, uvw013.x, uvw013.y, uvw013.z);
		isValid &= Utility::BarycentricCoordsTriangle(q, p0, p2, p3, uvw023.x, uvw023.y, uvw023.z);
		isValid &= Utility::BarycentricCoordsTriangle(q, p1, p2, p3, uvw123.x, uvw123.y, uvw123.z);

		if (!isValid)
			return VoronoiRegionType::Unknown;

		if (u01 > 0 && v01 > 0 && uvw012.z < 0 && uvw013.z < 0)
			return SetEdge(q, p0, p1, newSize, newIndices, u01, v01, closestPoint, searchDirection, VoronoiRegionType::Edge01);
		else if (u02 > 0 && v02 > 0 && uvw012.y < 0 && uvw023.z < 0)
			return SetEdge(q, p0, p2, newSize, newIndices, u02, v02, closestPoint, searchDirection, VoronoiRegionType::Edge02);
		else if (u03 > 0 && v03 > 0 && uvw013.y < 0 && uvw023.y < 0)
			return SetEdge(q, p0, p3, newSize, newIndices, u03, v03, closestPoint, searchDirection, VoronoiRegionType::Edge03);
		else if (u12 > 0 && v12 > 0 && uvw012.x < 0 && uvw123.z < 0)
			return SetEdge(q, p1, p2, newSize, newIndices, u12, v12, closestPoint, searchDirection, VoronoiRegionType::Edge12);
		else if (u13 > 0 && v13 > 0 && uvw123.y < 0 && uvw013.x < 0)
			return SetEdge(q, p1, p3, newSize, newIndices, u13, v13, closestPoint, searchDirection, VoronoiRegionType::Edge13);
		else if (u23 > 0 && v23 > 0 && uvw023.x < 0 && uvw123.x < 0)
			return SetEdge(q, p2, p3, newSize, newIndices, u23, v23, closestPoint, searchDirection, VoronoiRegionType::Edge23);
		else if (uvw012.x > 0 && uvw012.y > 0 && uvw012.z > 0 && CheckTriangleSide(q, p0, p1, p2, p3) >= 0)
			return SetTriangle(q, p0, p1, p2, p3, newSize, newIndices, uvw012.x, uvw012.y, uvw012.z, closestPoint, searchDirection, VoronoiRegionType::Triangle012);
		else if (uvw013.x > 0 && uvw013.y > 0 && uvw013.z > 0 && CheckTriangleSide(q, p0, p1, p3, p2) >= 0)
			return SetTriangle(q, p0, p1, p2, p3, newSize, newIndices, uvw013.x, uvw013.y, uvw013.z, closestPoint, searchDirection, VoronoiRegionType::Triangle013);
		else if (uvw023.x > 0 && uvw023.y > 0 && uvw023.z > 0 && CheckTriangleSide(q, p0, p2, p3, p1) >= 0)
			return SetTriangle(q, p0, p1, p2, p3, newSize, newIndices, uvw023.x, uvw023.y, uvw023.z, closestPoint, searchDirection, VoronoiRegionType::Triangle023);
		else if (uvw123.x > 0 && uvw123.y > 0 && uvw123.z > 0 && CheckTriangleSide(q, p1, p2, p3, p0) >= 0)
			return SetTriangle(q, p0, p1, p2, p3, newSize, newIndices, uvw123.x, uvw123.y, uvw123.z, closestPoint, searchDirection, VoronoiRegionType::Triangle123);

		//Collide
		newSize = 4;
		newIndices[0] = 0;
		newIndices[1] = 1;
		newIndices[2] = 2;
		newIndices[3] = 3;

		
		searchDirection = glm::vec3{ 0.f, 0.f, 0.f };
		closestPoint = q;

		//return VoronoiRegionType::Unknown;
		return VoronoiRegionType::Tetrahedra0123;
	}
}