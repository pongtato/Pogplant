using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class ExplosiveEnemy : MonoBehaviour
	{
		//Variables

		//activate the entity
		bool m_armed = false;

		//outline which will be turned on and off
		uint m_outline_id;

		uint m_player_id;

		uint m_propeller_id;
		bool m_spin_propeller = true;
		float m_propeller_speed;
		Vector3 m_propeller_rot;
		Vector3 m_propeller_rot_axis;

		//how long the entity is alive for
		float m_lifetime;

		//just a timer
		float m_dt_counter = 0;
		bool test = false;
		// respective threshold
		// threshold_3 > threshold_2 > threshold_1 > BOOM
		// please use common sense, threshold values should be lower arming timer
		float m_threshold_2;
		float m_threshold_1;

		float m_flicker_rate_2;
		float m_flicker_rate_1;

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
		}

		public override void Start()
		{
			BindData();
		}

		//mainly for binding of datas and entity id
		void BindData()
		{
			m_outline_id = ECS.FindChildEntityWithName(entityID, "Outline");
			m_propeller_id = ECS.FindChildEntityWithName(entityID, "Propeller");
			m_player_id = ECS.FindEntityWithName("PlayerShip");

			m_threshold_1 = ECS.GetValue<float>(entityID, 3.69f, "m_ExplosiveEnemy_threshold_1");
			m_threshold_2 = ECS.GetValue<float>(entityID, 1.69f, "m_ExplosiveEnemy_threshold_2");

			m_flicker_rate_1 = ECS.GetValue<float>(entityID, 0.69f, "m_ExplosiveEnemy_flicker_rate_1");
			m_flicker_rate_2 = ECS.GetValue<float>(entityID, 0.69f, "m_ExplosiveEnemy_flicker_rate_2");

			m_propeller_speed = ECS.GetValue<float>(entityID, 200.0f, "m_ExplosiveEnemy_propeller_speed");
			//m_propeller_rot_axis = ECS.GetValue<Vector3>(entityID, new Vector3(0,1,0), "m_ExplosiveEnemy_propeller_rot_axis");
			m_propeller_rot_axis = new Vector3(0, 1, 0);
			m_armed = true;
			m_lifetime = ECS.GetValue<float>(entityID, 5.69f, "m_ExplosiveEnemy_lifetime");
			Console.WriteLine("m_player_id id: " + m_player_id);
			Console.WriteLine("m_outline_id id: " + m_outline_id);
			Console.WriteLine("m_propeller_id id: " + m_propeller_id);
		}

		public override void Update(float dt)
		{
			if (m_armed)
            {
				m_lifetime -= dt;

				// threshold_3 > threshold_2 > threshold_1 > BOOM
				if (m_lifetime < 0)
                {
					Console.WriteLine("BOOOOOOOOOOOOOOM");
				}
				else if(m_lifetime < m_threshold_1)
                {
					Console.WriteLine("beep beep beep");
					Flicker(ref dt, ref m_flicker_rate_1);
				}
				else if (m_lifetime < m_threshold_2)
				{
					Flicker(ref dt, ref m_flicker_rate_2);
					Console.WriteLine("beep beep");
				}
			}

			SpinObjectEndless(m_propeller_id, m_propeller_rot_axis, m_propeller_speed, dt);

		}

		void Flicker(ref float dt, ref float limit)
        {
			m_dt_counter += dt;
			if (m_dt_counter > limit)
            {
				//if (test)
				//	ECS.SetActive(m_outline_id, !test);
    //            else
				//	ECS.SetActive(m_outline_id, !test);

				test = !test;
				ECS.ToggleEntity(m_outline_id);
				m_dt_counter = 0;
			}
        }

		void SpinObjectEndless(uint id, Vector3 axis, float spin_speed, float dt)
		{
			if (m_spin_propeller)
			{
				m_propeller_rot += axis * spin_speed * dt;
				//m_propeller_rot.X += x_axis * spin_speed * dt;
				//m_propeller_rot.Y += y_axis * spin_speed * dt;
				//m_propeller_rot.Z += z_axis * spin_speed * dt;

				ClampRotationValue(ref m_propeller_rot.X, -360.0f, 360.0f);
				ClampRotationValue(ref m_propeller_rot.Y, -360.0f, 360.0f);
				ClampRotationValue(ref m_propeller_rot.Z, -360.0f, 360.0f);

				ECS.SetRotation(id, m_propeller_rot);
			}
		}

		void ClampRotationValue(ref float value, float min, float max)
		{
			if (value <= min)
				value = 0.0f;
			else if (value >= max)
				value = 0.0f;
		}

		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			if (id == m_player_id)
			{
				if (!m_armed)
					m_armed = true;
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
