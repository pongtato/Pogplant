#include "imaginary_system.h"
#include "../Entity.h"

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
	//entities will be a container of objects with it
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

	}
}