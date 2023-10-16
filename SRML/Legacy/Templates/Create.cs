using UnityEngine;

namespace SRML.SR.Templates
{
    public class Create<T> : ICreateComponent where T : Component
    {
        private readonly System.Action<T> action;

        public Create(System.Action<T> action)
        {
            this.action = action;
        }

        public void AddComponent(GameObject obj)
        {
            T comp = obj.AddComponent<T>();
            action?.Invoke(comp);
        }
    }
}
