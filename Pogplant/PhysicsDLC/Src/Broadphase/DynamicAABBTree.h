/******************************************************************************/
/*!
\file	DynamicAABBTree.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the definition of the broadphase dynamic aabb tree

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef DYNAMICAABBTREE_H_
#define DYNAMICAABBTREE_H_

#include "../Collision/Colliders.h"
#include <unordered_map>

namespace PhysicsDLC::Broadphase
{
	template <typename IDType>
	class DynamicAABBTree
	{
	public:
		DynamicAABBTree() = default;
		~DynamicAABBTree();

		void InsertData(void** key, const IDType& entityID, const Collision::Shapes::AABB& aabb);
		void UpdateData(void** key, const IDType& entityID, const Collision::Shapes::AABB& aabb);
		void RemoveData(const IDType& entityID);

		void QueryTree(BroadphaseQuery<IDType>& query);
		void SetNullObject(const IDType& object);

		std::vector<Collision::Shapes::AABB> GetAABBTreeBoxes();

		void Clear();

	private:
		enum class NODELOCSTAT
		{
			N_LEFT,
			N_RIGHT
		};

		struct Node
		{
			Node* m_parent = nullptr;
			Node* m_left = nullptr;
			Node* m_right = nullptr;

			PhysicsDLC::Collision::Shapes::AABB m_aabb;
			IDType m_entity;

			int m_height = 0;
		};

		/**>Root of the AABB Tree*/
		Node* m_treeRoot = nullptr;
		IDType m_IDNullObj;

		std::unordered_map<IDType, Node*> m_IDmap;

		const float m_AABBFatteningFactor = 1.1f;


		void RemoveData(void** key);

		void GetAABBTreeBoxes(std::vector<Collision::Shapes::AABB>& c_boxes, Node* c_curNode);
		void SelfQueryTree(Node* c_node, BroadphaseQuery<IDType>& results);
		void SelfQueryTree(Node* c_nodeA, Node* c_nodeB, BroadphaseQuery<IDType>& results);
		void SplitNode(Node* c_nodeA, Node* c_nodeB, BroadphaseQuery<IDType>& results);

		NODELOCSTAT GetNodeLoc(Node* c_curNode, Node* c_loc);
		

		Node* SelectNodeArea(Node* c_node0, Node* c_node1, Node* c_insertNode);

		void BalanceTree(Node* c_curNode);
		void RotateTree(Node* c_old, Node* c_pivot, Node* c_small);
		

		void InsertNode(Node* c_curNode, Node* c_nodeToInsert);

		void RecomputeHeight(Node* c_curNode);
		void RecomputeAABB(Node* c_curNode);

		void DeleteTree(Node* c_curNode);
	};
}

#include "DynamicAABBTree.hpp"

#endif // !DYNAMICAABBTREE_H_
