/******************************************************************************/
/*!
\file	BroadphaseQuery.hpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the implementations of the broadphase query

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
namespace PhysicsDLC::Broadphase
{
	template<typename IDType>
	BroadphaseQuery<IDType>::BroadphaseQuery(const BroadphaseQuery& rhs)
	{
		if (this == &rhs)
			return;

		m_query = rhs.m_query;
	}

	template<typename IDType>
	BroadphaseQuery<IDType>::BroadphaseQuery(BroadphaseQuery&& rhs)
	{
		if (this == &rhs)
			return;

		m_query = std::move(rhs.m_query);
	}

	template<typename IDType>
	BroadphaseQuery<IDType>& BroadphaseQuery<IDType>::operator=(const BroadphaseQuery& rhs)
	{
		if (this != &rhs)
		{
			m_query = rhs.m_query;
		}
		return *this;
	}

	template<typename IDType>
	BroadphaseQuery<IDType>& BroadphaseQuery<IDType>::operator=(BroadphaseQuery&& rhs)
	{
		if (this != &rhs)
		{
			m_query = std::move(rhs.m_query);
		}
		return *this;
	}

	template<typename IDType>
	void BroadphaseQuery<IDType>::AddQuery(IDType id1, IDType id2)
	{
		m_query.push_back(QueryType{ id1, id2 });
	}

	template<typename IDType>
	void BroadphaseQuery<IDType>::Clear()
	{
		m_query.clear();
	}
}