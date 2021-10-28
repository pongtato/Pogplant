#include "../../Application.h" // To get the model function
#include "Pogplant.h"
#include "ScriptSystem.h"
#include "../Entity.h"

std::uint32_t ScriptSystem::CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag)
{
	std::string _name = mono_string_to_utf8(name);
	std::string _tag = mono_string_to_utf8(tag);

	return static_cast<std::uint32_t>(m_ecs->CreateEntity(_name, pos, rot, scale, _tag).GetID());
}

void ScriptSystem::DestroyEntity(std::uint32_t entityID)
{
	m_ecs->DestroyEntity(static_cast<entt::entity>(entityID));
}

std::uint32_t ScriptSystem::CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag)
{
	std::string _name = mono_string_to_utf8(name);
	std::string _tag = mono_string_to_utf8(tag);

	return static_cast<std::uint32_t>(m_ecs->CreateChild(static_cast<entt::entity>(parentID), _name, pos, rot, scale, _tag));
}

std::uint32_t ScriptSystem::FindEntityWithName(MonoString* name)
{
	std::string _name = mono_string_to_utf8(name);
	return static_cast<std::uint32_t>(m_ecs->FindEntityWithName(_name));
}

void ScriptSystem::AddModel(std::uint32_t _Entity, MonoString* _Model, glm::vec3 _Color, bool _UseLight, bool _EditorOnly, bool _FirstIt)
{

	entt::entity value = static_cast<entt::entity>(_Entity);
	Entity id { value,m_ecs };
	std::string _name = mono_string_to_utf8(_Model);
	PP::Model* model = nullptr;
	//Can be in map
	if (_name == "Sphere")
	{
		model = PP::ModelResource::m_ModelPool["sphere"];
	}
	PogplantDriver::Application& instance = PogplantDriver::Application::GetInstance();
	if (model)
	{
		instance.ConstructModel(id, model, &model->m_Meshes.begin()->second, _Color, _UseLight, _EditorOnly, _FirstIt);
	}
}

//void ScriptSystem::AddComponentTransform(unsigned int id, Components::Transform transform)
//{
//	m_ecs->GetReg().emplace<Components::Transform>(static_cast<entt::entity>(id), transform);
//}
//
//void ScriptSystem::AddComponentRigidbody(unsigned int id, Components::Rigidbody rigidbody)
//{
//	m_ecs->GetReg().emplace<Components::Rigidbody>(static_cast<entt::entity>(id), rigidbody);
//}

//template<typename Type, typename... Args>
//void ScriptSystem::AddComponent(unsigned int id, Args&&... args)
//{
//	m_ecs->m_registry.emplace<Type>(id, std::forward<Args>(args)...);
//}

//template<typename Type, typename... Args>
//void ScriptSystem::AddComponent(unsigned int id, Args&&... args)
//{
//	m_ecs->m_registry.emplace<Type>(id, std::forward<Args>(args)...);
//}