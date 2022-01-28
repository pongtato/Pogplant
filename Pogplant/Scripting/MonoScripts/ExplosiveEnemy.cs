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
		bool m_armed = false;

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

		}

		public override void Update(float dt)
		{
		 
		}

		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			Console.WriteLine("Something entered");
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
