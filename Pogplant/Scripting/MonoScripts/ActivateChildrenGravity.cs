using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	//toggle the gravity in children
	public class ActivateChildrenGravity : MonoBehaviour
	{
		string m_generic_child_name;
		int m_max_children;

		uint m_player_id;

		List<uint> m_list_of_children = new List<uint>();

		float m_despawn_timer;

		bool m_do_once = false;
		bool single_child_activate = false;
		bool enabled_gravity = false;

		uint null_entity_id;

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			null_entity_id = ECS.GetNull();
		}

		//mainly for binding of datas and entity id
		void BindData()
		{
			m_generic_child_name = ECS.GetValue<String>(entityID, "Default", "m_Children_name");
			m_max_children = ECS.GetValue<int>(entityID, 0, "m_Children_max_index_range");

			m_despawn_timer = ECS.GetValue<float>(entityID, 5.0f, "m_despawn_timer");

			//used for unique objects
			single_child_activate = ECS.GetValue<bool>(entityID, false, "m_Children_Single");

			m_player_id = ECS.FindEntityWithName("PlayerShip");

			uint result;
			if (single_child_activate)
			{
				result = ECS.FindChildEntityWithName(entityID, m_generic_child_name);
				if(result != null_entity_id)
					m_list_of_children.Add(result);
				Console.WriteLine("added " + result);
			}
			else
			{
				for (int i = 0; i < m_max_children; i++)
                {
					result = ECS.FindChildEntityWithName(entityID, m_generic_child_name + i);
					if(result != null_entity_id)
						m_list_of_children.Add(result);

					Console.WriteLine("added " + result);
				}
			}

		}

		void ErrorCheck()
		{
			if (m_player_id == null_entity_id)
				Console.WriteLine("m_player_id is null");
			if (m_generic_child_name == "Default")
				Console.WriteLine("m_generic_child_name has not been filled in!");
		}

		public override void Start()
		{
			BindData();
			ErrorCheck();
		}

		public override void Update(float dt)
		{
			if (m_do_once)
			{

				foreach (uint entity in m_list_of_children)
					ECS.RigidbodySetGravity(entity, true);

				m_do_once = false;
				enabled_gravity = true;
			}

			if (enabled_gravity)
            {
				m_despawn_timer -= dt;

				if (m_despawn_timer < 0.0f)
                {
					enabled_gravity = false;

					foreach (uint entity in m_list_of_children)
						ECS.SetActive(entity, false);

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
				Console.WriteLine("ss");
				m_do_once = true;
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
