/*****************************************************************************/
/*!
\file	ScriptSystem.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Scripting system that utilizes Mono for c# scripting

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#ifndef _SCRIPT_SYSTEM_H_
#define _SCRIPT_SYSTEM_H_

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

#include "../ECS.h"
//#include "../Entity.h"
#include "../ScriptBinder.h"
#include "../Components/ScriptComponents.h"
#include "../../Events/Events.h"

class ECS;

struct MonoObjectWithGC
{
	uint32_t m_GCHandle;
	MonoObject* m_MonoObject;
};

class ScriptSystem
{
public:
	// C-tor
	ScriptSystem();
	// Cleanup mono
	~ScriptSystem();
	// Initialize mono and scripts
	void Init(ECS* ecs);
	// Update loop
	void Update(float dt);
	void LateUpdate();
	static void SetReload(bool _isReload);

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);
	void OnTriggerExitEvent(std::shared_ptr<PPE::OnTriggerExitEvent> onTriggerExitEvent);

	std::vector<std::string> m_scriptNames;

private:
	// Call the Start function of every script needed for each object
	//void Start();
	//void Start(MonoObject* object, MonoClass* klass);
	// Helper function to find the monomethod in monoclass
	MonoMethod* FindMethod(MonoClass* klass,const std::string& methodName, int params = -1);
	void Reload();
	void LoadMemory();
	void Cleanup();
	void Unload();
	void BindFunctions();

	// WRAPPER FUNCTIONS FOR ECS, entt::entity is an unsigned int
	//std::uint32_t CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale);
	//typedef std::uint32_t(ScriptSystem::* CreateEntityPtr)(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale);
	static std::uint32_t CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale);
	static void DestroyEntity(std::uint32_t entityID);
	static std::uint32_t CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale);
	static std::uint32_t FindEntityWithTag(MonoString* name);

	//ECS* m_registry;
	static ECS* m_ecs;
	// Mono Stuff to save
	MonoDomain* m_ptrMonoDomain = nullptr;
	MonoAssembly* m_ptrGameAssembly = nullptr;
	MonoImage* m_ptrGameAssemblyImage = nullptr;
	const std::string m_namespace{"Scripting"};
	// Save all my mono objects here, key is the type of script.
	//std::unordered_map<std::string, MonoObjectWithGC*> m_MonoObjects;
	std::unordered_map<std::string, std::unique_ptr<MonoObjectWithGC>> m_MonoObjects;
	static bool isReload;
};

#endif // _SCRIPT_SYSTEM_H_
