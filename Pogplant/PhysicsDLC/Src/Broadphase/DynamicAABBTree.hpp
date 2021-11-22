/******************************************************************************/
/*!
\file	DynamicAABBTree.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the implementations of the broadphase dynamic aabb tree

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
namespace PhysicsDLC::Broadphase
{
	template<typename IDType>
	PhysicsDLC::Broadphase::DynamicAABBTree<IDType>::~DynamicAABBTree()
	{
		DeleteTree(m_treeRoot);
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::QueryTree(BroadphaseQuery<IDType>& query)
	{
		SelfQueryTree(m_treeRoot, query);
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::SetNullObject(const IDType& object)
	{
		m_IDNullObj = object;
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::Clear()
	{
		DeleteTree(m_treeRoot);
	}

	template<typename IDType>
	typename DynamicAABBTree<IDType>::Node* DynamicAABBTree<IDType>::SelectNodeArea(Node* c_node0, Node* c_node1, Node* c_insertNode)
	{
		if (c_node0 == nullptr)
			return c_node1;

		if (c_node1 == nullptr)
			return c_node0;

		if (PhysicsDLC::Collision::Shapes::AABB::Combine(c_node0->m_aabb, c_insertNode->m_aabb).GetSurfaceArea() - c_node0->m_aabb.GetSurfaceArea()
			< PhysicsDLC::Collision::Shapes::AABB::Combine(c_node1->m_aabb, c_insertNode->m_aabb).GetSurfaceArea() - c_node1->m_aabb.GetSurfaceArea())
		{
			return c_node0;
		}
		else
		{
			return c_node1;
		}
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::BalanceTree(Node* c_curNode)
	{
		//Balance
		Node* traverse = c_curNode;
		while (traverse)
		{
			if (traverse->m_height >= 3)
			{
				int heightLeft = traverse->m_left ? traverse->m_left->m_height : -1;
				int heightRight = traverse->m_right ? traverse->m_right->m_height : -1;

				Node* childNode = traverse->m_left;
				if (heightLeft < heightRight)
					childNode = traverse->m_right;

				//If unbalanced
				if (std::abs(heightLeft - heightRight) > 1)
				{
					Node* restorePosition = traverse;

					if (childNode->m_left->m_height < childNode->m_right->m_height)
						RotateTree(traverse, childNode, childNode->m_left);
					else
						RotateTree(traverse, childNode, childNode->m_right);

					traverse = restorePosition;

					for (Node* traverse = c_curNode; traverse != nullptr; traverse = traverse->m_parent)
						RecomputeHeight(traverse);
				}
			}

			RecomputeAABB(traverse);

			if (traverse)
				traverse = traverse->m_parent;
		}
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::RotateTree(Node* c_old, Node* c_pivot, Node* c_small)
	{
		//Old is nullptr
		assert(c_old);

		if (!c_old->m_parent)
		{
			m_treeRoot = c_pivot;
			c_pivot->m_parent = nullptr;
		}
		else
		{
			//Relink top level old's parent
			if (c_old->m_parent->m_left == c_old)
				c_old->m_parent->m_left = c_pivot;
			else if (c_old->m_parent->m_right == c_old)
				c_old->m_parent->m_right = c_pivot;
			else
				assert(false);

			c_pivot->m_parent = c_old->m_parent;
		}

		//relink pivot and old swap
		if (c_pivot->m_left == c_small)
		{
			c_pivot->m_left = c_old;
		}
		else
		{
			c_pivot->m_right = c_old;
		}

		c_old->m_parent = c_pivot;

		//relink smaller node to where pivot was
		if (c_old->m_left == c_pivot)
		{
			c_old->m_left = c_small;
		}
		else
		{
			c_old->m_right = c_small;
		}

		c_small->m_parent = c_old;

		//Recompute heights for old and pivot, the rest should have the heights stay the same
		RecomputeHeight(c_old);
		RecomputeHeight(c_pivot);

		//Recompute aabb
		RecomputeAABB(c_old);
		RecomputeAABB(c_pivot);
	}

	/******************************************************************************/
	/*!
	\brief
		Selfquerys the entire tree
	\param c_node
		The current node in the recursive loop
	\param results
		The results of the query
	*/
	/******************************************************************************/
	template<typename IDType>
	void DynamicAABBTree<IDType>::SelfQueryTree(Node* c_node, BroadphaseQuery<IDType>& results)
	{
		if (!c_node || (!c_node->m_left && !c_node->m_right))
			return;

		SelfQueryTree(c_node->m_left, results);
		SelfQueryTree(c_node->m_right, results);
		SelfQueryTree(c_node->m_left, c_node->m_right, results);
	}

	/******************************************************************************/
	/*!
	\brief
		Selfquerys the tree
	\param c_nodeA
		The 1st node to query
	\param c_nodeB
		The 2nd node to query
	\param results
		The results of the query
	*/
	/******************************************************************************/
	template<typename IDType>
	void DynamicAABBTree<IDType>::SelfQueryTree(Node* c_nodeA, Node* c_nodeB, BroadphaseQuery<IDType>& results)
	{
		if (!c_nodeA || !c_nodeB)
			return;

		if (!PhysicsDLC::Collision::StaticAABBAABB(c_nodeA->m_aabb, c_nodeB->m_aabb))
			return;//if not overlap

			//if both are leaf
		if (!c_nodeA->m_left && !c_nodeA->m_right && !c_nodeB->m_left && !c_nodeB->m_right)
		{
			results.AddQuery(c_nodeA->m_entity, c_nodeB->m_entity);
			return;
		}
		else
		{
			SplitNode(c_nodeA, c_nodeB, results);
		}
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::SplitNode(Node* c_nodeA, Node* c_nodeB, BroadphaseQuery<IDType>& results)
	{
		if (!c_nodeA->m_left && !c_nodeA->m_right)
		{
			SelfQueryTree(c_nodeA, c_nodeB->m_left, results);
			SelfQueryTree(c_nodeA, c_nodeB->m_right, results);
		}
		else if (!c_nodeB->m_left && !c_nodeB->m_right)
		{
			SelfQueryTree(c_nodeB, c_nodeA->m_left, results);
			SelfQueryTree(c_nodeB, c_nodeA->m_right, results);
		}
		else
		{
			if (c_nodeA->m_aabb.GetVolume() < c_nodeB->m_aabb.GetVolume())
			{
				SelfQueryTree(c_nodeA, c_nodeB->m_left, results);
				SelfQueryTree(c_nodeA, c_nodeB->m_right, results);
			}
			else
			{
				SelfQueryTree(c_nodeA->m_left, c_nodeB, results);
				SelfQueryTree(c_nodeA->m_right, c_nodeB, results);
			}
		}
	}

	template<typename IDType>
	typename DynamicAABBTree<IDType>::NODELOCSTAT DynamicAABBTree<IDType>::GetNodeLoc(Node* c_curNode, Node* c_loc)
	{
		if (c_curNode->m_left && c_curNode->m_left == c_loc)
			return NODELOCSTAT::N_LEFT;

		if (c_curNode->m_right && c_curNode->m_right == c_loc)
			return NODELOCSTAT::N_RIGHT;

		//Nodes detached incorrectly
		assert(false);
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::InsertNode(Node* c_curNode, Node* c_nodeToInsert)
	{
		if (!m_treeRoot)
		{
			m_treeRoot = c_nodeToInsert;
			return;
		}

		if (c_curNode->m_height == 0)
		{
			//    [Parent]
			//     /    \
			// [curr] [insert]

			//Create new parent node
			Node* newParent = new Node{ c_curNode->m_parent, c_curNode, c_nodeToInsert };

			//Relink parent of curNode's left/right node to new parent
			if (c_curNode->m_parent)
			{
				if (GetNodeLoc(c_curNode->m_parent, c_curNode) == NODELOCSTAT::N_LEFT)
					c_curNode->m_parent->m_left = newParent;
				else
					c_curNode->m_parent->m_right = newParent;
			}
			else
				m_treeRoot = newParent;

			c_curNode->m_parent = newParent;
			c_nodeToInsert->m_parent = newParent;

			//Increase heights
			for (Node* traverse = c_curNode->m_parent; traverse != nullptr; traverse = traverse->m_parent)
			{
				RecomputeAABB(traverse);
				RecomputeHeight(traverse);
			}

			BalanceTree(c_nodeToInsert);
		}
		else
		{
			InsertNode(SelectNodeArea(c_curNode->m_left, c_curNode->m_right, c_nodeToInsert), c_nodeToInsert);
		}
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::RecomputeHeight(Node* c_curNode)
	{
		int height = std::max(
			(c_curNode->m_left ? c_curNode->m_left->m_height : -1),
			(c_curNode->m_right ? c_curNode->m_right->m_height : -1));

		if (height < 0)
			c_curNode->m_height = 0;
		else
			c_curNode->m_height = static_cast<size_t>(height) + 1;
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::RecomputeAABB(Node* c_curNode)
	{
		if (!c_curNode)
			return;

		if (c_curNode->m_left && c_curNode->m_right)
		{
			c_curNode->m_aabb = PhysicsDLC::Collision::Shapes::AABB::Combine(c_curNode->m_left->m_aabb, c_curNode->m_right->m_aabb);
		}
		else if (c_curNode->m_left)
		{
			c_curNode->m_aabb = c_curNode->m_left->m_aabb;
		}
		else if (c_curNode->m_right)
		{
			c_curNode->m_aabb = c_curNode->m_right->m_aabb;
		}
	}

	template<typename IDType>
	void DynamicAABBTree<IDType>::DeleteTree(Node* c_curNode)
	{
		if (!c_curNode)
			return;

		if (c_curNode->m_left)
		{
			DeleteTree(c_curNode->m_left);
		}
		if (c_curNode->m_right)
		{
			DeleteTree(c_curNode->m_right);
		}

		delete c_curNode;
	}
}