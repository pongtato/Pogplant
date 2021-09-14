#ifndef _SCRIPT_SYSTEM_
#define _SCRIPT_SYSTEM_

#include <mono/jit/jit.h>
#include <iostream>

#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include "ScriptBinder.h"

class ScriptSystem
{
public:
	// Embedding and setting up mono
	ScriptSystem();
	// Cleanup mono
	~ScriptSystem();

	// Functions to invoke from C# using mono
	bool testfuncwithreturn();
	void testfuncwithparam(char key);

private:

	MonoDomain* m_ptrMonoDomain = nullptr;
	MonoAssembly* m_ptrGameAssembly = nullptr;
	MonoImage* m_ptrGameAssemblyImage = nullptr;
	MonoObject* m_ptrGameObject = nullptr;
	uint32_t m_gameObjectGCHandle;
	MonoMethod* m_ptrTickMethod = nullptr;
	MonoMethod* m_ptrKeyEventMethod = nullptr;
};

#endif
