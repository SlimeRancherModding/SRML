using SRML.Core.ModLoader;
using System.Collections.Generic;

namespace SRML.Core.API
{
    public interface IRegistry
    {
        void Initialize();
        void Register(object toRegister);
        bool IsRegistered(object registered);
    }

    public abstract class Registry<R, T> : IRegistry where R : Registry<R, T>
    {
        protected Dictionary<IMod, List<T>> registeredForMod = new Dictionary<IMod, List<T>>();

        public abstract void Initialize();

        public void Register(object toRegister) => Register((T)toRegister);
        public bool IsRegistered(object registered) => IsRegistered((T)registered);

        public abstract void Register(T toRegister);
        public abstract bool IsRegistered(T registered);

        public static R Instance { get; private set; }

        internal Registry() => Instance = (R)this;
    }
}
