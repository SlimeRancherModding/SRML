using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SRML.Core.API.BuiltIn
{
    public interface IEnumRegistry
    {
    }

    public abstract class EnumRegistry<R, TEnum> : Registry<R>, IEnumRegistry
        where R : EnumRegistry<R, TEnum>
        where TEnum : Enum
    {
        internal Dictionary<string, List<TEnum>> moddedEnums = new Dictionary<string, List<TEnum>>();

        public delegate void EnumRegisterEvent(TEnum id);
        public readonly EnumRegisterEvent OnRegisterEnum;

        public virtual TEnum Create(string name) => Create(name, null);

        public virtual TEnum Create(string name, object value)
        {
            if (value == null)
                value = EnumPatcher.AddEnumValue(typeof(TEnum), name);
            else
                EnumPatcher.AddEnumValue(typeof(TEnum), value, name);

            TEnum result = (TEnum)value;
            OnRegisterEnum?.Invoke(result);
            // TODO: Add processors, add categorization.

            return result;
        }
    }
}
