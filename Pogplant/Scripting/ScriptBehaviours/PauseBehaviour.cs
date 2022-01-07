namespace Scripting
{
    public abstract class PauseBehaviour
    {
        public uint entityID;

        public abstract void Init(ref uint _entityID);
        public abstract void Start();
        public abstract void Update(float dt);
        public abstract void OnTriggerEnter(uint id);
        public abstract void OnTriggerExit(uint id);
    }
}
