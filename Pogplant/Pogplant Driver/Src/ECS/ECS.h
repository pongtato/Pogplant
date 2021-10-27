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

	Entity CreateEntity(std::string name = "pepe",
						glm::vec3 pos = { 0, 0, 0 }, 
						glm::vec3 rot = { 0, 0, 0 },
						glm::vec3 scale = { 1, 1, 1 },
						std::string tag = "Untagged");
	
	Entity CreateChild(	entt::entity parent,
						std::string name = "pepe",
						glm::vec3 pos = { 0, 0, 0 },
						glm::vec3 rot = { 0, 0, 0 },
						glm::vec3 scale = { 1, 1, 1 },
						std::string tag = "Untagged");

	void DestroyEntity(	entt::entity entity);
	//template<typename... Type>
	//auto& GetEntitiesWith()
	//{
	//	return 0;
	//}



	entt::registry& GetReg();

	entt::entity FindEntityWithName(std::string _name);
	entt::entity FindEntityWithTag(std::string _tag);
	//std::vector<entt::entity> FindAllEntityWithName(std::string& _name);

private:
	entt::registry m_registry;

	friend Entity;
};


#endif ECS_H