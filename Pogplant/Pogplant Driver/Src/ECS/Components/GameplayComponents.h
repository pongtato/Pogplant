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

		Projectile(float lifetime, float speed,  OwnerType ownertype, float damage = 10.f) :
			m_Lifetime(lifetime), m_CurentLifetime(0.f), m_Speed(speed), m_Damage(damage), m_Ownertype(ownertype) {};
	};

	struct Laser
	{
		bool  m_ActivateLaser = false;
		bool  m_Onceflag = false;
		float m_Spawntime;
		float m_Activetime;
		float m_Inactivetime;
		float m_ChargeParticletime;
		float m_LaserLerptime;

		bool m_IsDeactivated = false;
		float m_AccumulatedActivetime = 0.f;
		float m_AccumulatedTime = 0.f;
		bool m_LaserCompleted = false;

		Laser(float Spawntime = 5.f, float Activetime = 0.f, float Inactivetime = 0.f, float ChargeParticletime = 0.f, float LaserLerptime = 0.f) :
			m_Spawntime(Spawntime), m_Activetime(Activetime), m_Inactivetime(Inactivetime), m_ChargeParticletime(ChargeParticletime), m_LaserLerptime(LaserLerptime) {};
	};
}

#endif // !GENERICCOMPONENTS_H_
