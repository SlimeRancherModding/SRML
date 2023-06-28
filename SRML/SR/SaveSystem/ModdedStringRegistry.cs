using SRML.SR.SaveSystem.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem
{
    public static class ModdedStringRegistry
    {
        internal static List<StringRegistry> Registries = new List<StringRegistry>();

        internal static void RegisterRegistry(StringRegistry registry)
        {
            Registries.Add(registry);
        }
        internal static StringRegistry GetRegistry(string prefix)
        {
            var registry = Registries.FirstOrDefault(x=>x.Prefix==prefix);
            if(registry == null)
            {
                registry = new StringRegistry(prefix);
                Registries.Add(registry);
            }
            return registry;
        }

        internal static string ClaimID(string prefix, SRMod mod,string id)
        {
            var reg = GetRegistry(prefix);
            return reg.RegisterID(id, mod);
        }

        public static string ClaimID(string prefix,string id)
        {
            // TODO: Upgrade to new system
            return null;
            //return ClaimID(prefix, SRMod.GetCurrentMod(),id);
        }

        public static void FreeID(string prefix, string id)
        {
            // TODO: Upgrade to new system
            GetRegistry(prefix).UnRegisterID(id,null/*SRMod.GetCurrentMod()*/);
        }

        public static void FreeAllIDs(string prefix)
        {
            // TODO: Upgrade to new system
            GetRegistry(prefix).UnRegisterAll(null/*SRMod.GetCurrentMod()*/);
        }

        public static void FreeAllIDs()
        {
            foreach(var v in Registries)
            {
                // TODO: Upgrade to new system
                v.UnRegisterAll(/*SRMod.GetCurrentMod()*/null);
            }
        }
        internal static bool IsModdedString(string str) => StringRegistry.IsLongForm(str);

        internal static bool IsValidString(string str) => !IsModdedString(str) || GetModForModdedString(str) != null;

        internal static bool IsValidModdedString(string str) => IsModdedString(str) && GetModForModdedString(str) != null;

        internal static SRMod GetModForModdedString(string str)
        {
            if (!StringRegistry.IsLongForm(str)) return null;
            StringRegistry.ParseLongForm(str, out var prefix, out var modid, out var id);
            var mod = SRModLoader.GetMod(modid);
            if (!GetRegistry(prefix).Contains(mod,id)) return null;
            return mod;
        }
    
    }
}
