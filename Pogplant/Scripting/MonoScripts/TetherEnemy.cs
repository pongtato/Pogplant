using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class TetherEnemy : MonoBehaviour
	{
		//Variables

		//for fine tuning
		//if you need to rotate,
		//rotate the main object, not this.
		public float m_Min_Height;
		public float m_Max_Height;

		//score to deduce when collide
		public float m_Score_Penalty;
		public float m_Speed;

		bool m_do_once = false;

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
			m_Min_Height = ECS.GetValue<float>(entityID, 0, "Tether_Min_Height");
			m_Max_Height = ECS.GetValue<float>(entityID, 4, "Tether_Max_Height");

			m_Score_Penalty = ECS.GetValue<float>(entityID, 0, "Tether_Score_Penalty");
			m_Speed = ECS.GetValue<float>(entityID, 1, "Tether_Speed");
		}

		public override void Update(float dt)
		{
			if (m_do_once)
            {
				Console.WriteLine(entityID + " Has be activated");
				m_do_once = false;

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

		public void EnableLogic()
        {
			m_do_once = true;

		}
	}
}
