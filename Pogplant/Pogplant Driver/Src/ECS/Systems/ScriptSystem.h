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
#include "../Components/ScriptComponents.h"
#include "../../Events/Events.h"

class ECS;
struct MonoObjectWithGC;

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
	void LateUpdate(float dt);
	static void SetReload(bool _isReload);

	static ECS* GetECS() { return m_ecs; }
	
private:

	// Helper function to find the monomethod in monoclass
	void Reload();
	void LoadMemory();
	void Cleanup();
	void Unload();
	MonoMethod* FindMethod(MonoClass* klass, const std::string& methodName, int params = -1);
	void BindFunctions();

	static ECS* m_ecs;
	// Mono Stuff to save
	MonoDomain* m_ptrMonoDomain = nullptr;
	MonoAssembly* m_ptrGameAssembly = nullptr;
	MonoImage* m_ptrGameAssemblyImage = nullptr;

	const std::string m_namespace{ "Scripting" };
	static bool isReload;
};

#endif // _SCRIPT_SYSTEM_H_