using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.Core.API.BuiltIn
{
    public interface IEnumRegistry
    {
        void Process(Enum toProcess);
        void Register(object toRegister);
    }

    public abstract class EnumRegistry<R, T> : Registry<R, EnumObject<T>>, IEnumRegistry
        where R : EnumRegistry<R, T>
        where T : Enum
    {
        protected Dictionary<string, List<T>> valuesForMod = new Dictionary<string, List<T>>();

        public abstract void Process(T toProcess);
        public void Process(Enum toProcess) => Process((T)toProcess);

        public T Register(string name) => Register(null, name);
        public T Register(object value, string name)
        {
            EnumObject<T> enumOb = new EnumObject<T>(name, value);
            Register(enumOb);
            return (T)enumOb.Value;
        }

        public T[] RegisteredForMod(string id)
        {
            if (!valuesForMod.ContainsKey(id))
                return null;
            return valuesForMod[id].ToArray();
        }

        public bool IsRegistered(T registered) => valuesForMod.Any(x => x.Value.Contains(registered));

        public override void Initialize()
        {
            EnumPatcher.RegisterEnumRegistry(typeof(T), this);
        }

        public sealed override bool IsRegistered(EnumObject<T> registered)
        {
            if (registered.Value == null || registered.Value.GetType() != typeof(T))
                return false;

            return IsRegistered((T)registered.Value);
        }

        public sealed override void Register(EnumObject<T> toRegister)
        {
            if (toRegister.Value == null)
                toRegister.Value = EnumPatcher.AddEnumValue<T>(toRegister.Name);
            else
                EnumPatcher.AddEnumValue<T>(toRegister.Value, toRegister.Name);

            T newVal = (T)toRegister.Value;

            string id = CoreLoader.Instance.GetExecutingModContext()?.ModInfo.Id;
            if (!valuesForMod.ContainsKey(id))
                valuesForMod[id] = new List<T>();

            valuesForMod[id].Add(newVal);
        }
    }
}
