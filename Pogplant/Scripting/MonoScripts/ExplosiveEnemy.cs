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

		//Indicators to show range
		uint m_indicator_white;
		uint m_indicator_red;

		uint m_player_id;

		//used to accumulate dt
		float m_timer_counter;

		// respective threshold
		// threshold_3 > threshold_2 > threshold_1 > BOOM
		// please use common sense, threshold values should be lower arming timer
		float m_threshold_3;
		float m_threshold_2;
		float m_threshold_1;

		public override void Init(ref uint _entityID)
		{
			m_timer_counter = 0;
			entityID = _entityID;
		}

		public override void Start()
		{
			BindData();
		}

		//mainly for binding of datas and entity id
		void BindData()
		{
			m_indicator_white = ECS.FindChildEntityWithName(entityID, "White");
			m_indicator_red = ECS.FindChildEntityWithName(entityID, "Red");

			m_player_id = ECS.FindEntityWithName("PlayerShip");

			m_threshold_1 = ECS.GetValue<float>(entityID, 0.69f, "m_ExplosiveEnemy_threshold_1");
			m_threshold_2 = ECS.GetValue<float>(entityID, 0.69f, "m_ExplosiveEnemy_threshold_2");
			m_threshold_3 = ECS.GetValue<float>(entityID, 0.69f, "m_ExplosiveEnemy_threshold_3");

			Console.WriteLine("m_player_id id: " + m_player_id);
		}

		public override void Update(float dt)
		{
			if (m_armed)
            {
				m_timer_counter += dt;

				// threshold_3 > threshold_2 > threshold_1 > BOOM
				if (m_timer_counter < 0)
                {
					Console.WriteLine("BOOOOOOOOOOOOOOM");
				}
				else if(m_timer_counter < m_threshold_1)
                {
					Console.WriteLine("beep beep beep");
				}
				else if (m_timer_counter < m_threshold_2)
				{
					Console.WriteLine("beep beep");
				}
				else if (m_timer_counter < m_threshold_3)
				{
					Console.WriteLine("beep");
				}
			}
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
