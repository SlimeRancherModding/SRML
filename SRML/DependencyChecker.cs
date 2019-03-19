using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML
{
    internal static class DependencyChecker
    {
        public static bool CheckDependencies(HashSet<SRModLoader.ProtoMod> mods)
        {
            foreach (var v in mods)
            {
                if (!v.HasDependencies) continue;
                foreach (var dep in v.dependencies.Select((x) => Dependency.ParseFromString(x)))
                {
                    if (!mods.Any((x) => dep.SatisfiedBy(x)))
                        throw new Exception(
                            $"Unresolved dependency for '{v.id}'! Cannot find '{dep.mod_id} {dep.mod_version}'");
                }
            }

            return true;
        }

        internal class Dependency
        {
            public string mod_id;
            public SRModInfo.ModVersion mod_version;

            public static Dependency ParseFromString(String s)
            {
                var strings = s.Split(' ');
                var dep = new Dependency();
                dep.mod_id = strings[0];
                dep.mod_version = SRModInfo.ModVersion.Parse(strings[1]);
                return dep;
            }

            public bool SatisfiedBy(SRModLoader.ProtoMod mod)
            {
                return mod.id == mod_id && SRModInfo.ModVersion.Parse(mod.version).CompareTo(mod_version)<=0;
            }
        }
    }
}
