using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public abstract class MonoBehaviour
    {
        public abstract void Start();
        public abstract void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt);
    }
}
