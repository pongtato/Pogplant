#ifndef ECS_H
#define ECS_H


#include "entt_ecs/entt.hpp"
//#include "Entity.h"
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

class Entity;

class ECS
{
public:
	ECS();
	~ECS();		

	Entity CreateEntity(std::string str = "pepe", 
						glm::vec3 pos = { 0, 0, 0 }, 
						glm::vec3 rot = { 0, 0, 0 },
						glm::vec3 scale = { 1, 1, 1 });
	
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