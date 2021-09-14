#ifndef ECS_H
#define ECS_H


#include "entt_ecs/entt.hpp"
//#include "Entity.h"

class Entity;

class ECS
{
public:
	ECS();
	~ECS();

	Entity CreateEntity(std::string str = "pepe");

	//template<typename... Type>
	//auto& GetEntitiesWith()
	//{
	//	return 0;
	//}



	entt::registry& GetReg();

private:
	entt::registry m_registry;

	friend Entity;

};


#endif ECS_H