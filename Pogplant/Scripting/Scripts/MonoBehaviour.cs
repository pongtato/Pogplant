/******************************************************************************/
/*!
\file	Monobehaviour.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
	This base class of all scripts that are attached to game objects.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
namespace Scripting
{
    public abstract class MonoBehaviour
    {
        public uint entityID;

        public abstract void Init(ref uint _entityID);
        public abstract void Start();
        public abstract void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt);
        public abstract void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt);

        public abstract void OnTriggerEnter(uint id);
        public abstract void OnTriggerExit(uint id);
    }
}
