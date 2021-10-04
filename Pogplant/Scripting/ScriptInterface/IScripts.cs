using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // Interface class for scripts
    public interface IScripts
    {
        void testing123();

        void Start();

        void Update();

        void Move(float dt, ref Transform transform);

        void RigidbodyMove(ref Rigidbody rigidbody);
    }
}
