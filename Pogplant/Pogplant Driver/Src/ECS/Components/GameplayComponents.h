#ifndef GAMEPLAYCOMPONENTS_H_
#define GAMEPLAYCOMPONENTS_H_

#include <rttr/registration>
#include <gtc/type_ptr.hpp>
#include <gtc/random.hpp>

namespace Components
{
	struct Projectile
	{
		enum class OwnerType // Will change to just int if it does not work
		{
			Nobody,
			Player,
			Enemy
		};
		enum class ProjectileType
		{
			Player,
			True,
			False
		};

		bool m_Homing;
		float m_Lifetime;
		float m_CurentLifetime;
		float m_Speed;
		float m_Damage;
		OwnerType m_Ownertype;
		ProjectileType m_Type;

		Projectile(float lifetime, float speed,  OwnerType ownertype, float damage = 0.f) :
			m_Lifetime(lifetime), m_CurentLifetime(0.f), m_Speed(speed), m_Damage(damage), m_Ownertype(ownertype) {};
	};

}

#endif // !GENERICCOMPONENTS_H_
