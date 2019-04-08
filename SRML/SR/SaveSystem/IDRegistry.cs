using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    internal class IDRegistry<T> : Dictionary<T, SRMod>, IIDRegistry
    {

        public Type RegistryType => typeof(T);

        public IList GetAllModdedIDs()
        {
            return Keys.ToList();
        }

        public IList GetIDsForMod(SRMod mod)
        {
            return this.Where((x) => x.Value == mod).Select((x) => x.Key).ToList();
        }

        public SRMod GetModForID(object val)
        {
            return this[(T)val];
        }

        public bool IsModdedID(object val)
        {
            return val.GetType() == RegistryType && ContainsKey((T)val);
        }

        public T RegisterValue(T id)
        {
            if (ContainsKey(id))
                throw new Exception(
                    $"{id} is already registered to {this[id].ModInfo.Id}");
            var sr = SRMod.GetCurrentMod();
            if (sr != null) this[id] = sr;
            return id;
        }

        public T RegisterValueWithEnum(T id, string name)
        {
            var newid = RegisterValue(id);
            EnumPatcher.AddEnumValue(RegistryType, newid, name);
            return newid;
        }
    }

    internal interface IIDRegistry
    {

        Type RegistryType { get; }

        bool IsModdedID(object val);

        SRMod GetModForID(object val);

        IList GetIDsForMod(SRMod mod);

        IList GetAllModdedIDs();
    }
}
