#include "imaginary_system.h"
#include "../Entity.h"

#include "../Components/PhysicsComponents.h"
#include "../../Input/GLFWInput.h"

using namespace Components;

Imaginary_system::Imaginary_system()
{
}

Imaginary_system::~Imaginary_system()
{
}

void Imaginary_system::Init(ECS* ecs)
{
	m_registry = ecs;
}

void Imaginary_system::Update()
{
	/*//entities will be a container of objects with it
	//get all entities with the imaginary_object component
	auto entities = m_registry->GetReg().view<Imaginary_object, Transform, Tag>();

	//get all entities with more than 1 component
	//auto entities = m_registry->GetReg().view<Imaginary_object, Transform, Tag>();
	
	//exclude entities with certain component
	//auto entities = m_registry->GetReg().view<Transform>(entt::exclude<Imaginary_object>);


	for (auto entity : entities)
	{
		//do stuffs
		//get components
		auto& imaginary_component = entities.get<Imaginary_object>(entity);

		(void)imaginary_component;
	}*/

	auto test = m_registry->GetReg().try_get<Components::Rigidbody>(testObject);

	if (test)
	{
		if (Pogplant::Input::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_UP))
		{
			test->AddForce({ 0.f, 0.f, -1.f });
		}

		if (Pogplant::Input::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_DOWN))
		{
			test->AddForce({ 0.f, 0.f, 1.f });
		}

		if (Pogplant::Input::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_LEFT))
		{
			test->AddForce({ -1.f, 0.f, 0.f });
		}

		if (Pogplant::Input::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_RIGHT))
		{
			test->AddForce({ 1.f, 0.f, 0.f });
		}

		if (Pogplant::Input::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_PAGE_UP))
			test->mass = 3.f;

		if (Pogplant::Input::GLFWInputManager::Instance()->onKeyHeld(GLFW_KEY_PAGE_DOWN))
			test->mass = 0.5f;
	}
}