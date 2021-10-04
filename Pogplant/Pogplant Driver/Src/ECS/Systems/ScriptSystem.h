#ifndef _SCRIPT_SYSTEM_
#define _SCRIPT_SYSTEM_

#include "../ECS.h"
#include "../ScriptBinder.h"
#include "../Components/ScriptComponents.h"

#include <vector>

class ECS;

class ScriptSystem
{
public:
	// Embedding and setting up mono
	ScriptSystem();
	// Cleanup mono
	~ScriptSystem();

	void Init(ECS* ecs);

	void Update();

	MonoMethod* FindMethod(MonoClass* klass, std::string methodName, int params = -1);

private:
	// Functions to invoke from C# using mono
	//bool testfuncwithreturn();
	//void testfuncwithparam(char key);
	void Start();

	ECS* m_registry;
	MonoDomain* m_ptrMonoDomain = nullptr;
	MonoAssembly* m_ptrGameAssembly = nullptr;
	MonoImage* m_ptrGameAssemblyImage = nullptr;
	// Mono base class
	MonoClass* m_MonoClass;
	// Mono virtual class
	MonoClass* m_VirtualMonoClass;
	// Save all my objects here, find method in mono class using help function Find_Method(), key is the GC handle
	std::unordered_map<uint32_t, MonoObject*> m_MonoObjects;
};

#endif // _SCRIPT_SYSTEM_
