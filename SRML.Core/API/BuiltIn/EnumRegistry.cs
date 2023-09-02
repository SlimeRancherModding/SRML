using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SRML.Core.API.BuiltIn
{
    public interface IEnumRegistry
    {
        void Process(Enum toProcess);
        void Register(string name, object value);
    }

    public abstract class EnumRegistry<R, TEnum> : Registry<R, string, object>, IEnumRegistry
        where R : EnumRegistry<R, TEnum>
        where TEnum : Enum
    {
        protected Dictionary<string, List<TEnum>> valuesForMod = new Dictionary<string, List<TEnum>>();

        public abstract void Process(TEnum toProcess);
        public void Process(Enum toProcess) => Process((TEnum)toProcess);

        public TEnum Register(string name)
        {
            Register(name, null);
            return (TEnum)Enum.Parse(typeof(TEnum), name);
        }

        public TEnum RegisterAndParse(string name, object value)
        {
            Register(name, value);
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        public TEnum[] RegisteredForMod(string id)
        {
            if (!valuesForMod.ContainsKey(id))
                return null;
            return valuesForMod[id].ToArray();
        }

        public bool IsRegistered(TEnum registered) => valuesForMod.Any(x => x.Value.Contains(registered));

        public override void Initialize()
        {
            EnumPatcher.RegisterEnumRegistry(typeof(TEnum), this);
        }

        public sealed override bool IsRegistered(string name, object value)
        {
            if (value == null || value.GetType() != typeof(TEnum))
                return false;

            return IsRegistered((TEnum)value);
        }

        public sealed override void Register(string name, object value)
        {
            if (value == null)
                value = EnumPatcher.AddEnumValue<TEnum>(name);
            else
                EnumPatcher.AddEnumValue<TEnum>(value, name);

            TEnum newVal = (TEnum)value;

            string id = CoreLoader.Instance.GetExecutingModContext()?.ModInfo.Id;
            if (!valuesForMod.ContainsKey(id))
                valuesForMod[id] = new List<TEnum>();

            valuesForMod[id].Add(newVal);
        }
    }
}
