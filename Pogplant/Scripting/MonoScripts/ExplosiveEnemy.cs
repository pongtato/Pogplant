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

		//-Indicators to show range
		uint m_near_indicator;
		uint m_medium_indicator;
		uint m_far_indicator;

		uint m_player_id;

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
			m_near_indicator = ECS.FindChildEntityWithName(entityID, "Near");
			m_medium_indicator = ECS.FindChildEntityWithName(entityID, "Medium");
			m_far_indicator = ECS.FindChildEntityWithName(entityID, "Far");

			m_player_id = ECS.FindEntityWithName("PlayerBox");


			Console.WriteLine("m_near_indicator id: " + m_near_indicator);
			Console.WriteLine("m_medium_indicator id: " + m_medium_indicator);
			Console.WriteLine("m_far_indicator id: " + m_far_indicator);
			Console.WriteLine("m_player_id id: " + m_player_id);
		}

		public override void Update(float dt)
		{
			if (m_armed)
            {
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
