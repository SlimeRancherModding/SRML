using HarmonyLib;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SRML.Core.API
{
    public interface IRegistry
    {
        void Initialize();
    }

    public interface IRegistryNew : IRegistry
    {
        void RegisterAllOfType(Type processorType, object registerInto);
    }

    public abstract class Registry<R> : IRegistryNew where R : Registry<R>
    {
        private Dictionary<Type, List<IProcessor>> registered = new Dictionary<Type, List<IProcessor>>();
        private List<IProcessor> alreadyRegistered = new List<IProcessor>();

        public abstract void Initialize();

        protected bool Register(IProcessor processor)
        {
            Type processorType = processor.GetType();
            if (!registered.ContainsKey(processorType))
            {
                registered.Add(processorType, new List<IProcessor>());
                registered[processorType].Add(processor);
                return true;
            }

            if (registered[processorType].Any(x => x.Equals(processor)))
                return false;

            registered[processorType].Add(processor);
            return true;
        }

        public void RegisterAllOfType(Type processorType, object registerInto)
        {
            if (!registered.ContainsKey(processorType))
                return;

            foreach (IProcessor processor in registered[processorType])
            {
                if (alreadyRegistered.Contains(processor))
                    return;

                processor.Register(registerInto);
                alreadyRegistered.Add(processor);
            }
        }

        private static R _instance;
        public static R Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Activator.CreateInstance<R>();

                return _instance;
            }
        }
    }

    // EVERYTHING AFTER THIS IS OBSOLETE LOL

    public abstract class Registry<R, T> : IRegistry where R : Registry<R, T>
    {
        protected Dictionary<IMod, List<T>> registeredForMod = new Dictionary<IMod, List<T>>();
        public T[] Registered { get => _registered.ToArray(); }
        protected List<T> _registered = new List<T>();

        public abstract void Initialize();

        public abstract void Register(T toRegister);
        public abstract bool IsRegistered(T registered);

        public List<T> GetRegisteredForMod(string id) => registeredForMod.First(x => x.Key.ModInfo.ID == id).Value;

        public static R Instance { get; private set; }

        internal Registry() => Instance = (R)this;
    }

    public abstract class Registry<R, T, T2> : IRegistry where R : Registry<R, T, T2>
    {
        protected Dictionary<IMod, List<(T, T2)>> registeredForMod = new Dictionary<IMod, List<(T, T2)>>();
        public (T, T2)[] Registered { get => _registered.ToArray(); }
        protected List<(T, T2)> _registered = new List<(T, T2)>();

        public abstract void Initialize();

        public abstract void Register(T toRegister, T2 toRegister2);
        public abstract bool IsRegistered(T registered, T2 registered2);

        public List<(T, T2)> GetRegisteredForMod(string id) => registeredForMod.First(x => x.Key.ModInfo.ID == id).Value;

        public static R Instance { get; private set; }

        internal Registry() => Instance = (R)this;
    }

    public abstract class Registry<R, T, T2, T3> : IRegistry where R : Registry<R, T, T2, T3>
    {
        protected Dictionary<IMod, List<(T, T2, T3)>> registeredForMod = new Dictionary<IMod, List<(T, T2, T3)>>();
        public (T, T2, T3)[] Registered { get => _registered.ToArray(); }
        protected List<(T, T2, T3)> _registered = new List<(T, T2, T3)>();

        public abstract void Initialize();

        public abstract void Register(T toRegister, T2 toRegister2, T3 toRegister3);
        public abstract bool IsRegistered(T registered, T2 registered2, T3 toRegister3);

        public List<(T, T2, T3)> GetRegisteredForMod(string id) => registeredForMod.First(x => x.Key.ModInfo.ID == id).Value;

        public static R Instance { get; private set; }

        internal Registry() => Instance = (R)this;
    }
}
