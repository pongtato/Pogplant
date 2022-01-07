using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class TestPauseUpdate : PauseBehaviour
    {
        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            Console.WriteLine("PauseScript Init!");
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            Console.WriteLine("PauseScript Updating!");
        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {
        }
    }
}
