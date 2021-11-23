/******************************************************************************/
/*!
\file	GeneralSystem.h
\author 
\par	email: 
\details

	This file contains implementations for a system handling generic components
	that are basic enough that do not require a whole file or class
	delicated to it

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef GENERAL_SYSTEM_H_
#define GENERAL_SYSTEM_H_

#include "../ECS.h"

class ECS;

class GeneralSystem
{
public:
	GeneralSystem();
	~GeneralSystem();

	void Init(ECS* ecs);
	void Update(float c_dt);
	void UpdateTransforms();
	void DeleteEntities();
private:
	ECS* m_registry;

	void UpdateTransform(entt::entity _id, Components::Transform& parent_transform);

};


#endif // ! IMAGINARY_H
