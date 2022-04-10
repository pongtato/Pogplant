using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class SimpleMovement : MonoBehaviour
	{
		//Variables

		Vector3 m_direction;

		//they should be static position, only thing that should be moving is the parent 
		Vector3 m_waypoint_A = new Vector3();
		Vector3 m_waypoint_B = new Vector3();

		float m_speed_AB; //movement speed from point A to B
		float m_speed_BA; //movement speed from point B to A

		//delays before moving
		float m_delay_A;
		float m_delay_B;

		//bool m_do_once = false;

		int counter = 0;
		float timer_counter = 0.0f;

		uint m_null_entity;
		bool start_up_check = true;
		bool set_impulse_once = false;

		Vector3 temp_trans = new Vector3();
		Vector3 temp_rot = new Vector3();
		Vector3 temp_scale = new Vector3();

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
			uint parent_id = ECS.GetParent(entityID);
			//Console.WriteLine(parent_id + " is the parent");
			m_null_entity = ECS.GetNull();
			//Console.WriteLine(m_null_entity + " is the null entity id");

			uint waypoint = ECS.FindChildEntityWithName(parent_id, "A");

			Vector3 temp_rot = new Vector3();
			Vector3 temp_scale = temp_rot;

			if (waypoint != m_null_entity)
            {
                ECS.GetTransformECS(waypoint, ref m_waypoint_A, ref temp_rot, ref temp_scale);

				//deactivate ball render
				ECS.SetActive(waypoint, false);
			}
			else
                start_up_check = false;

			waypoint = ECS.FindChildEntityWithName(parent_id, "B");

			if (waypoint != m_null_entity)
            {
                ECS.GetTransformECS(waypoint, ref m_waypoint_B, ref temp_rot, ref temp_scale);

				//deactivate ball render
				ECS.SetActive(waypoint, false);
			}
            else
                start_up_check = false;

			m_speed_AB = ECS.GetValue<float>(entityID, 1.0f, "move_speed_AB");
			m_speed_BA = ECS.GetValue<float>(entityID, 1.0f, "move_speed_BA");

			m_delay_A = ECS.GetValue<float>(entityID, 0.0f, "delay_A");
			m_delay_B = ECS.GetValue<float>(entityID, 0.0f, "delay_B");

			if (!start_up_check)
				Console.WriteLine(entityID + " is has failed null entity check at start up");

		}
		void MoveObject(ref Vector3 waypoint, ref float speed, ref float timer_delay, ref float dt)
		{
			ECS.GetTransformECS(entityID, ref temp_trans, ref temp_rot, ref temp_scale);

			m_direction = Vector3.Normalise(waypoint - temp_trans);

			if (!set_impulse_once)
			{
				GameUtilities.MoveWithImpulse(entityID, m_direction, speed);
				set_impulse_once = true;
			}

			float val = Vector3.DistanceSquared(waypoint, temp_trans);

			if (val < 0.1f)
			{
				GameUtilities.StopMoving(entityID);

				timer_counter += dt;

				if (timer_counter > timer_delay)
				{
					set_impulse_once = false;
					counter++;
					timer_counter = 0;
				}

			}
		}

		public override void Update(float dt)
		{
			if (start_up_check)
			{

				int val = (counter % 2);
				//Console.WriteLine(val);
				if (val == 0)
				{
					MoveObject(ref m_waypoint_A, ref m_speed_AB, ref m_delay_A, ref dt);
				}
				else
				{
					MoveObject(ref m_waypoint_B, ref m_speed_BA, ref m_delay_B, ref dt);
				}

				//if (m_do_once)
				//{
				//	Console.WriteLine(entityID + " Has be activated");
				//	m_do_once = false;
				//}
			}
		}



		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			//Console.WriteLine("Something entered");
		}

		public override void OnTriggerExit(uint id)
		{

		}

		//public void EnableLogic()
		//{
		//	m_do_once = true;
		//}
	}
}
