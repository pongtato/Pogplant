#include "Pogplant.h"
#include "../Src/ImguiHelper.h"
#include "../Src/ECS/Entity.h"
#include "../Src/ECS/Components/Components.h"
#include "../Src/ECS/Components/DependantComponents.h"
#include "GameScript.h"



namespace PPD = PogplantDriver;
using namespace Components;
namespace Scripting
{
	// Only checking the bound for player to it's parent and will not work anywhere else
	int CheckBounds(glm::vec3 _Position)
	{
		std::string parent{ "PlayerBox" };
		entt::entity parent_box = PPD::ImguiHelper::m_ecs->FindEntityWithName(parent);
		if (parent_box != entt::null)
		{
			auto boxcollider_comp = PPD::ImguiHelper::m_ecs->GetReg().try_get<BoxCollider>(parent_box);
			glm::vec3 max_bound = boxcollider_comp->extends;
			glm::vec3 min_bound = -(max_bound);
			int value = 0;
			if (_Position.x > max_bound.x || _Position.x < min_bound.x)
				value |= 1;
			if (_Position.y > max_bound.y || _Position.y < min_bound.y)
				value |= 2;
			return value;
		}
		return 0;
	}
	void FirePlayerBullet(glm::vec3 _Position, glm::vec3 _Rotation)
	{
		auto new_bullet = PPD::ImguiHelper::m_ecs->CreateEntity("Bullet", _Position, _Rotation);
		new_bullet.AddComponent<Projectile>(3.f, 10.f, Components::Projectile::OwnerType::Player);
		new_bullet.AddComponent<Renderer>(glm::vec3{ 0 }, PP::ModelResource::m_ModelPool["sphere"], &PP::ModelResource::m_ModelPool["sphere"]->m_Meshes.begin()->second);
		auto sp_collider = new_bullet.AddComponent<SphereCollider>();
		sp_collider.isTrigger = true;
		new_bullet.AddComponent<Rigidbody>();

		auto body = new_bullet.GetComponent<Rigidbody>();
		body.AddForce({ 0.f,100.f,10.f });

	}
}
