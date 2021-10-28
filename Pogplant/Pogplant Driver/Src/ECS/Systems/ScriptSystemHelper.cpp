#include "ScriptSystem.h"
#include "ScriptSystemHelper.h"
#include <Pogplant.h>

namespace SSH
{
	void AddComponentTransform(unsigned int id, Components::Transform transform)
	{
		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Transform>(static_cast<entt::entity>(id), transform);
	}

	void AddComponentRigidbody(unsigned int id, Components::Rigidbody rigidbody)
	{
		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Rigidbody>(static_cast<entt::entity>(id), rigidbody);
	}

	void AddComponentRenderer(unsigned int id, glm::vec3 colorTint, int useLight, bool editorDrawOnly, MonoString* name)
	{
		std::string modelName = mono_string_to_utf8(name);
		Pogplant::Model* model = Pogplant::ModelResource::m_ModelPool[modelName];
		Pogplant::Mesh3D* mesh3d = &model->m_Meshes.begin()->second;

		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Renderer>(static_cast<entt::entity>(id), Components::Renderer{ colorTint, model, mesh3d, useLight, editorDrawOnly });
	}

	MonoArray* GetModelKeysIC()
	{
		MonoArray* data = mono_array_new(mono_domain_get(), mono_get_string_class(), Pogplant::ModelResource::m_ModelPool.size());
		int i = 0;
		for (auto it = Pogplant::ModelResource::m_ModelPool.begin(); it != Pogplant::ModelResource::m_ModelPool.end(); ++it)
		{
			mono_array_set(data, MonoString*, i, mono_string_new(mono_domain_get(), it->first.c_str()));
			++i;
		}
		return data;
	}
}