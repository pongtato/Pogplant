#ifndef _SCRIPTSYSTEM_HELPER_
#define _SCRIPTSYSTEM_HELPER_

#include "../Components/Components.h"
#include "../Components/DependantComponents.h"

namespace SSH
{
	void AddComponentTransform(unsigned int id, Components::Transform transform);
	void AddComponentRigidbody(unsigned int id, Components::Rigidbody rigidbody);
	void AddComponentRenderer(unsigned int id, glm::vec3 colorTint, int useLight, bool editorDrawOnly, MonoString* name);
	MonoArray* GetModelKeysIC();
}

#endif // _SCRIPTSYSTEM_HELPER_