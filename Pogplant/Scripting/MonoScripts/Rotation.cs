using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class Rotation : MonoBehaviour
	{
		//Variables
		bool m_rotate_endlessly;

		float m_rotation_speed;
		Vector3 m_rotation_axis;

		//only used if m_rotate_endlessly is false;
		//used to rotate fixed degree
		float m_rotation_degree;

		Vector3 m_rotation_value;

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
			m_rotate_endlessly = ECS.GetValue<bool>(entityID, false, "Rotation_rotate_endlessly");
			m_rotation_speed = ECS.GetValue<float>(entityID, 1.0f, "Rotation_speed");
			m_rotation_axis = ECS.GetValue<Vector3>(entityID, new Vector3(0, 1, 0), "Rotation_axis");
			m_rotation_degree = ECS.GetValue<float>(entityID, 69.0f, "Rotation_degree");
		}

		public override void Update(float dt)
		{
			if (m_rotate_endlessly)
			{
				SpinObjectEndless(entityID, m_rotation_axis, m_rotation_speed, dt);
			}
			else
			{
				if(m_rotation_degree > 0.0f)
					FixedRotation(entityID, m_rotation_axis, m_rotation_speed, dt, m_rotation_degree);
			}

            //if (m_do_once)
            //{
            //    Console.WriteLine(entityID + " Has be activated");
            //    m_do_once = false;
            //}
        }

		void SpinObjectEndless(uint id, Vector3 axis, float spin_speed, float dt)
		{

			m_rotation_value += axis * spin_speed * dt;

			ClampRotationValue(ref m_rotation_value.X, -360.0f, 360.0f);
			ClampRotationValue(ref m_rotation_value.Y, -360.0f, 360.0f);
			ClampRotationValue(ref m_rotation_value.Z, -360.0f, 360.0f);

			ECS.SetRotation(id, m_rotation_value);

		}

		void FixedRotation(uint id, Vector3 axis, float spin_speed, float dt, float limit)
		{
			SpinObjectEndless(id, axis, spin_speed, dt);
			m_rotation_degree -= spin_speed * dt;
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
