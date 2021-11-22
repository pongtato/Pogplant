/******************************************************************************/
/*!
\file	BroadphaseQuery.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the definition of the broadphase query

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef BROADPHASEQUERY_H_
#define BROADPHASEQUERY_H_

#include <vector>

namespace PhysicsDLC::Broadphase
{
	template <typename IDType>
	struct BroadphaseQuery
	{
		BroadphaseQuery() = default;
		~BroadphaseQuery() = default;

		BroadphaseQuery(const BroadphaseQuery& rhs);
		BroadphaseQuery(BroadphaseQuery&& rhs);

		BroadphaseQuery& operator=(const BroadphaseQuery& rhs);
		BroadphaseQuery& operator=(BroadphaseQuery&& rhs);

		void AddQuery(IDType id1, IDType id2);
		void Clear();

		struct QueryType
		{
			IDType m_ID1;
			IDType m_ID2;
		};

		std::vector<QueryType> m_query;
	};
}

#include "BroadphaseQuery.hpp"

#endif // !BROADPHASEQUERY_H_
