using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class L1BossBehaviour : MonoBehaviour
	{
        public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
		}

        public override void Start()
		{
			Console.WriteLine("AWUIODHAWOIDHAOIWDHAIUOWDHAOIWUDHGOIUAWHDIOUWAHDA");
		}

        public override void Update(float dt)
		{
			
		}
		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{

		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
