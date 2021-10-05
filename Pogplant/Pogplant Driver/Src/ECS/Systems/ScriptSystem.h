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

#include "../ECS.h"
#include "../ScriptBinder.h"
#include "../Components/ScriptComponents.h"

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
	void Update();

private:
	// Call the Start function of every script needed for each object
	void Start();
	// Helper function to find the monomethod in monoclass
	MonoMethod* FindMethod(MonoClass* klass, std::string methodName, int params = -1);

	ECS* m_registry;
	// Mono Stuff to save
	MonoDomain* m_ptrMonoDomain = nullptr;
	MonoAssembly* m_ptrGameAssembly = nullptr;
	MonoImage* m_ptrGameAssemblyImage = nullptr;
	// Save all my mono objects here, key is the type of script.
	std::unordered_map<std::string, MonoObjectWithGC*> m_MonoObjects;
};

#endif // _SCRIPT_SYSTEM_H_
