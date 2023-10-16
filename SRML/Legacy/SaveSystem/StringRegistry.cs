using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using SRML.Utils;

namespace SRML.SR.SaveSystem
{
    /// <summary>
    /// Class for handling string ID's for the save system
    /// </summary>
    internal class StringRegistry : Dictionary<SRMod,HashSet<string>>
    {
        public string Prefix { get; }
        public StringRegistry(string prefix)
        {
            Prefix = prefix;
        }

        public HashSet<string> GetIDsForMod(SRMod mod)
        {
            if (mod == null) return null;
            if (!TryGetValue(mod, out var set))
            {
                set = new HashSet<string>(StringComparer.Ordinal);
                this[mod] = set;
            }
            return set;
        }

        public bool Contains(SRMod mod, string id) => GetIDsForMod(mod)?.Contains(id) ?? false;

        public bool ConformsToPrefix(string candidate) => candidate.StartsWith(Prefix);

        internal static string GetLongFormID(string prefix, string id, SRMod mod)
        {
            return $"{prefix}.{mod.ModInfo.Id}.{id}";
        }
        public string GetLongFormID(string id, SRMod mod)
        {
            return GetLongFormID(Prefix, id, mod);
        }

        public string ParseIDFromLongForm(string longform)
        {
            return longform.Split('.')[2];
        }

        public static void ParseLongForm(string longform, out string prefix, out string modid, out string id)
        {
            var split = longform.Split('.');
            prefix = split[0];
            id = split[2];
            modid = split[1];
        }
        
        public static bool IsLongForm(string longform)
        {
            return longform.Split('.').Length == 3;
        }

        public string RegisterID(string id, SRMod mod)
        {

            GetIDsForMod(mod).Add(id); 
            return GetLongFormID(id, mod);
        }

        public void UnRegisterID(string id,  SRMod mod)
        {
            GetIDsForMod(mod).Remove(id);
        }

        public void UnRegisterAll(SRMod mod)
        {
            GetIDsForMod(mod).Clear();
        }

       
    }
}
