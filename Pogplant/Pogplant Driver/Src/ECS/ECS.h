#ifndef ECS_H
#define ECS_H


#include "entt_ecs/entt.hpp"
//#include "Entity.h"
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

//debug flags

#define ECS_DEBUG //shows entity ID, parent ID and child ID in inspector
//#define SHOW_PREFAB //Show Prefab in SceneHeirachy, ASK VINCEEN IF UNSURE
//#define SHOW_IMGUIDEMOWINDOW

class Entity;
class GeneralSystem;

namespace Components
{
	struct Prefab;
	struct Disabled;
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
	entt::entity FindChildEntityWithName(entt::entity parentID, const std::string& _name);
	//std::vector<entt::entity> FindAllEntityWithName(std::string& _name);

	bool IsChildOf(entt::entity _parent, entt::entity _child);

	void SetParent(entt::entity _parent, entt::entity _child);
	entt::entity RemoveParentFrom(entt::entity _id);
	void SetChild(entt::entity _parent, entt::entity _child);
	void RemoveChildFrom(entt::entity _parent, entt::entity _child);

	entt::entity CopyEntity(entt::entity _target, entt::entity _original_value_override = entt::null);

	std::string GenerateGUID();

	void DisableEntity(entt::entity _entity);
	void EnableEntity(entt::entity _entity);
	void ToggleEntity(entt::entity _entity);

	//clears the ECS
	void Clear();

	template<typename... Component, typename... Exclude>
	auto view(entt::exclude_t<Exclude...> _exclude = {})
	{
		return m_registry.view<Component...>(entt::exclude_t<Exclude..., Components::Prefab, Components::Disabled>());
	}

	//dont use this unless you really know lol
	template<typename... Component, typename... Exclude>
	auto view_SHOW_PREFAB(entt::exclude_t<Exclude...> _exclude = {})
	{
		return m_registry.view<Component...>(_exclude);
	}


	//prefab map
	//file path to entity id
	std::unordered_map<std::string, entt::entity> m_prefab_map;
	bool m_edit_prefab = false;



private:
	entt::registry m_registry;

	template<typename T>
	void Try_Copy(entt::entity _new_entity, entt::entity _target)
	{
		const auto& component = m_registry.try_get<T>(_target);
		if (component)
			m_registry.emplace_or_replace<T>(_new_entity, m_registry.get<T>(_target));
	}

	//used for delayed deleting of entites
	std::set<entt::entity> m_EntitiesToDelete;
	void TrulyDestroyEntity(entt::entity _id);

	std::set<entt::entity> m_EntitiesToDisable;
	void TrulyDisableEntity(entt::entity _id);

	std::set<entt::entity> m_EntitiesToEnable;
	void TrulyEnableEntity(entt::entity _id);

	friend Entity;
	friend GeneralSystem;
};


#endif ECS_H