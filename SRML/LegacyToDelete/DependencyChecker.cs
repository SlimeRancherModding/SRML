using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HarmonyLib;
using SRML.Utils;
using UnityEngine;

namespace SRML
{
    internal static class DependencyChecker
    {
        public static bool CheckDependencies(HashSet<SRModLoader.ProtoMod> mods)
        {
            foreach (var mod in mods)
            {
                if (!mod.HasDependencies) continue;
                foreach (var dep in mod.parsedDependencies)
                {
                    if (!mods.Any((x) => dep.SatisfiedBy(x)))
                        throw new Exception(
                            $"Unresolved dependency for '{mod.id}'! Cannot find '{dep.mod_id} {dep.version}'");
                }
            }

            return true;
        }

        public static void CalculateLoadOrder(HashSet<SRModLoader.ProtoMod> mods, List<string> loadOrder)
        {
            loadOrder.Clear();
            var modList = new List<SRModLoader.ProtoMod>();
            
            HashSet<string> currentlyLoading = new HashSet<string>();


            void FixAfters(SRModLoader.ProtoMod mod)
            {
                foreach(var h in mod.load_before)
                {

                    if (mods.FirstOrDefault((x) => x.id == h) is SRModLoader.ProtoMod proto)
                    {
                        proto.load_after = new HashSet<string>(proto.load_after.AddToArray(mod.id)).ToArray();
                        
                    }
                }

            }

            foreach (var v in mods)
            {
                FixAfters(v);
            }

            void LoadMod(SRModLoader.ProtoMod mod)
            {
                if (modList.Contains(mod)) return;
                currentlyLoading.Add(mod.id);
                foreach (var v in mod.load_after)
                {
                    if (!(mods.FirstOrDefault((x) => x.id == v) is SRModLoader.ProtoMod proto)) continue;
                    if (currentlyLoading.Contains(v)) throw new Exception("Circular dependency detected "+mod.id+" "+v);
                    LoadMod(proto);
                }


                modList.Add(mod);

                currentlyLoading.Remove(mod.id);

                
            }

            foreach (var v in mods)
            {
                LoadMod(v);
            }

            loadOrder.AddRange(modList.Select((x)=>x.id));
        }

        public static Dictionary<string, SRModInfo.ModVersion> ToDependencyDictionary(this Dependency[] dependencies)
        {
            Dictionary<string, SRModInfo.ModVersion> result = new Dictionary<string, SRModInfo.ModVersion>();
            foreach (Dependency dependency in dependencies) result.Add(dependency.mod_id, dependency.version);
            return result;
        }

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
