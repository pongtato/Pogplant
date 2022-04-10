using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	//activate a function in the children's script

	public class ActivateChildren : MonoBehaviour
	{
		string m_generic_child_name;
		string m_generic_child_script_name;
		string m_generic_child_function_name;
		int m_max_children;

		uint m_player_id;

		List<uint> m_list_of_children = new List<uint>();

		bool m_do_once = false;
		bool single_child_activate = false;
		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
		}

		//mainly for binding of datas and entity id
		void BindData()
		{
			m_generic_child_script_name = ECS.GetValue<String>(entityID, "Default", "m_Children_script_name");
			m_generic_child_function_name = ECS.GetValue<String>(entityID, "Default", "m_Children_function_name");
			Console.WriteLine(m_generic_child_script_name);
			Console.WriteLine(m_generic_child_function_name);
			m_generic_child_name = ECS.GetValue<String>(entityID, "Default", "m_Children_name");
			m_max_children = ECS.GetValue<int>(entityID, 0, "m_Children_max_index_range");
			single_child_activate = ECS.GetValue<bool>(entityID, false, "m_Children_Single");

			m_player_id = ECS.FindEntityWithName("PlayerShip");

			if (single_child_activate)
            {
				m_list_of_children.Add(ECS.FindChildEntityWithName(entityID, m_generic_child_name));
			}
			else
            {
				for (int i = 0; i < m_max_children; i++)
					m_list_of_children.Add(ECS.FindChildEntityWithName(entityID, m_generic_child_name + i));

			}

		}

		void ErrorCheck()
        {
			if (m_generic_child_script_name == "Default")
				Console.WriteLine("m_generic_child_script_name has not been filled in!");
			if (m_generic_child_function_name == "Default")
				Console.WriteLine("m_generic_child_function_name has not been filled in!");
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
				if (single_child_activate)
                {
					GameUtilities.InvokeScriptFunction(m_list_of_children[0], m_generic_child_script_name, m_generic_child_function_name);
				}
				else
                {
					foreach (uint entity in m_list_of_children)
						GameUtilities.InvokeScriptFunction(entity, m_generic_child_script_name, m_generic_child_function_name);
				}

				m_do_once = false;
			}
		}

		public override void LateUpdate(float dt)
		{
			
		}

		public override void OnTriggerEnter(uint id)
		{
			if (id == m_player_id)
            {
				m_do_once = true;
            }
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
