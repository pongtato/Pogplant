#ifndef ECS_H
#define ECS_H


#include "entt_ecs/entt.hpp"
//#include "Entity.h"
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

//debug flags

#define ECS_DEBUG //shows entity ID, parent ID and child ID in inspector
#define SHOW_PREFAB //Show Prefab in SceneHeirachy, ASK VINCEEN IF UNSURE

class Entity;

namespace Components
{
	struct Prefab;
}

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


	entt::registry& GetReg();

	entt::entity FindEntityWithName(std::string _name);
	entt::entity FindEntityWithTag(std::string _tag);
	//std::vector<entt::entity> FindAllEntityWithName(std::string& _name);

	template<typename... Component, typename... Exclude>
	inline auto view(entt::exclude_t<Exclude...> _exclude = {})
	{
		return m_registry.view<Component...>(entt::exclude_t<Exclude..., Components::Prefab>());
	}

	//dont use this unless you really know lol
	template<typename... Component, typename... Exclude>
	inline auto view_SHOW_PREFAB(entt::exclude_t<Exclude...> _exclude = {})
	{
		return m_registry.view<Component...>(_exclude);
	}

private:
	entt::registry m_registry;

	friend Entity;
};


#endif ECS_H