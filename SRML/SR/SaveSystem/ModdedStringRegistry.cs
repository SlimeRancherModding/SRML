using SRML.SR.SaveSystem.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        internal static void ClaimNewID(string prefix, SRMod mod,string alias)
        {
            var reg = GetRegistry(prefix);
            reg.RegisterID(reg.GenerateNewID(), mod,alias);
        }

        public static void ClaimID(string prefix, string alias)
        {
            ClaimNewID(prefix, SRMod.GetCurrentMod(), alias);
        }

        internal static Dictionary<string,Dictionary<string,string>> GetModKeys(SRMod mod)
        {
            var dict = new Dictionary<string,Dictionary<string, string>>();
            foreach(var reg in Registries)
            {
                var newDict = new Dictionary<string, string>();
                foreach(var key in reg.Where(x=>x.Value.Item1==mod).Select(x=>new KeyValuePair<string,string>(x.Value.Item2,x.Key)))
                {
                    newDict.Add(key.Key, key.Value);
                }
                if (newDict.Count > 0) dict[reg.Prefix] = newDict;
            }
            return dict;
        }

        internal static void ReadModKeys(SRMod mod, Dictionary<string,Dictionary<string,string>> dict)
        {
            foreach(var v in dict)
            {
                var reg = GetRegistry(v.Key);
                foreach(var key in v.Value)
                {
                    if (reg.ContainsKey(key.Value))
                    {
                        var temp = reg[key.Value];
                        reg.Remove(key.Value);
                        reg[reg.GenerateNewID()] = temp;
                    }
                    reg[key.Value] = new SRML.Utils.Tuple<SRMod, string>(mod, key.Key);
                }
            }
        }
        internal static void Push(ModdedSaveData data)
        {
            foreach (var v in new HashSet<SRMod>(Registries.SelectMany(x => x.Values.Select(y => y.Item1))))
            {
                var seg = data.GetSegmentForMod(v);
                seg.idData = GetModKeys(v);
            }

        }

        internal static string ParseModColonAlias(string prefix, string identifier)
        {
            var reg = GetRegistry(prefix);
            var strings = identifier.Split(':');
            var mod = SRModLoader.GetMod(strings[0]);
            var alias = strings[1];
            return reg.FromAliasAndMod(alias, mod);
        }
        internal static void Pull(ModdedSaveData data)
        {
            foreach(var seg in data.segments)
            {
                var mod = SRModLoader.GetMod(seg.modid);
                ReadModKeys(mod, seg.idData);
            }
        }
    
    }
}
