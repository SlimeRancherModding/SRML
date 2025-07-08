using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML
{
    internal static class DependencyChecker
    {
        public static bool CheckDependencies(HashSet<SRModLoader.ProtoMod> mods)
        {
            foreach (var mod in mods)
            {
                if (!mod.HasDependencies) 
                    continue;

                IEnumerable<Dependency> unmet = mod.parsedDependencies.Where(x => !mods.Any(y => x.SatisfiedBy(y)));
                if (unmet.Any())
                    throw new Exception($"Unresolved dependency for '{mod.id}'! Cannot find '{unmet.First().mod_id} {unmet.First().version}'");
            }
            return true;
        }

        private static int CompareLoadingOrder(SRModLoader.ProtoMod mod1, SRModLoader.ProtoMod mod2)
        {
            if (mod1.load_before.Contains(mod2.id) && mod2.load_before.Contains(mod1.id))
                throw new Exception($"{mod1.id} and {mod2.id} attempting to load before one another.");
            if (mod1.load_after.Contains(mod2.id) && mod2.load_after.Contains(mod1.id))
                throw new Exception($"{mod1.id} and {mod2.id} attempting to load after one another.");

            if (mod1.load_before.Contains(mod2.id) || mod2.load_after.Contains(mod1.id))
                return -1;
            else if (mod1.load_after.Contains(mod2.id) || mod2.load_before.Contains(mod1.id))
                return 1;
            else
                return 0;
        }

        public static void CalculateLoadOrder(ref HashSet<SRModLoader.ProtoMod> mods, out List<string> loadOrder)
        {
            List<SRModLoader.ProtoMod> modsSorted = new List<SRModLoader.ProtoMod>(mods);
            modsSorted.Sort(CompareLoadingOrder);
            mods = modsSorted.ToHashSet();
            loadOrder = modsSorted.Select(x => x.id).ToList();

            foreach (string s in loadOrder)
                UnityEngine.Debug.Log(s);
        }

        public static Dictionary<string, SRModInfo.ModVersion> ToDependencyDictionary(this Dependency[] dependencies) => dependencies.ToDictionary(x => x.mod_id, y => y.version);

        internal class Dependency
        {
            public string mod_id;
            public SRModInfo.ModVersion version { get; private set; }

            public Dependency(string id, string version)
            {
                mod_id = id;
                this.version = SRModInfo.ModVersion.Parse(version);
            }

            [Obsolete]
            public static Dependency ParseFromString(string s)
            {
                var strings = s.Split(' ');
                var dep = new Dependency(strings[0], strings[1]);
                return dep;
            }

            public bool SatisfiedBy(SRModLoader.ProtoMod mod) => mod.id == mod_id && SRModInfo.ModVersion.Parse(mod.version).CompareTo(version) <= 0;
        }
    }
}
