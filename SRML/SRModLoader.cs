using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SRML
{
    internal static class SRModLoader
    {
        public const string ModJson = "modinfo.json";

        static readonly Dictionary<String,SRMod> Mods = new Dictionary<string, SRMod>();

        public static void LoadMods()
        {
            FileSystem.CheckDirectory(FileSystem.ModPath);
            HashSet<ProtoMod> foundMods = new HashSet<ProtoMod>(new ProtoMod.Comparer());
            foreach (var v in Directory.GetFiles(FileSystem.ModPath, ModJson, SearchOption.AllDirectories))
            {
                var mod = ProtoMod.ParseFromJson(v);
                if (!foundMods.Add(mod))
                {
                    throw new Exception("Found mod with duplicate id '"+mod.id+"' in "+v+"!");
                }



            }

            DependencyChecker.CheckDependencies(foundMods);

            DiscoverAndLoadAssemblies(foundMods);
        }

        static void DiscoverAndLoadAssemblies(ICollection<ProtoMod> protomods)
        {
            HashSet<AssemblyInfo> foundAssemblies = new HashSet<AssemblyInfo>();
            foreach (var mod in protomods)
            {
                foreach (var file in Directory.GetFiles(mod.path,"*.dll", SearchOption.AllDirectories))
                {
                    foundAssemblies.Add(new AssemblyInfo(AssemblyName.GetAssemblyName(Path.GetFullPath(file)),
                        Path.GetFullPath(file), mod));
                   
                }

               


            }

            Assembly FindAssembly(object obj, ResolveEventArgs args)
            {
                var name = new AssemblyName(args.Name);
                return foundAssemblies.First((x) => x.DoesMatch(name)).LoadAssembly();
            }

            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;
            try
            {
                foreach (var mod in protomods)
                {
                    foreach (var v in foundAssemblies.Where((x)=>x.mod==mod))
                    {
                        var a = v.LoadAssembly();
                        Type entryType = a.ManifestModule.GetTypes()
                            .FirstOrDefault((x) => (x.BaseType?.FullName ?? "") == "SRML.ModEntryPoint");
                        if (entryType == default(Type)) continue;
                        AddMod(v.mod, entryType);

                        goto foundmod;
                    }

                    throw new EntryPointNotFoundException($"Could not find assembly for mod '{mod}'");

                    foundmod:
                    continue;
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= FindAssembly;
            }
        }

        public static SRMod GetMod(string id)
        {
            return Mods[id];
        }

        public static SRMod GetModForAssembly(Assembly a)
        {
            return Mods.FirstOrDefault((x) => x.Value.EntryType.Assembly == a).Value;
        }

        static void AddMod(ProtoMod modInfo, Type entryType)
        {
            ModEntryPoint entryPoint = (ModEntryPoint) Activator.CreateInstance(entryType);
            var newmod = new SRMod(modInfo.ToModInfo(), entryPoint,modInfo.path);
            Mods.Add(modInfo.id,newmod);
        }

        public static void PreLoadMods()
        {
            foreach (var mod in Mods)
            {
                mod.Value.PreLoad();

            }
        }

        public static void PostLoadMods()
        {
            foreach (var mod in Mods)
            {
                mod.Value.PostLoad();
            }
        }

        internal struct AssemblyInfo
        {
            public AssemblyName AssemblyName;
            public String Path;
            public ProtoMod mod;
            public AssemblyInfo(AssemblyName name, String path,ProtoMod mod)
            {
                AssemblyName = name;
                Path = path;
                this.mod = mod;
            }

            public bool DoesMatch(AssemblyName name)
            {
                return name.Name == AssemblyName.Name;
            }

            public Assembly LoadAssembly()
            {
                return Assembly.LoadFrom(Path);
            }
        }
        internal class ProtoMod
        {
            public string id;
            public string name;
            public string author;
            public string version;
            public string path;
            public string[] dependencies;
            public override bool Equals(object o)
            {
                if (!(o is ProtoMod obj)) return base.Equals(o);
                return id == obj.id;
            }

            public bool HasDependencies
            {
                get
                {
                    return dependencies != null && dependencies.Length > 0;
                }
            }

            public static ProtoMod ParseFromJson(String jsonFile)
            {

                var proto =
                    JsonConvert.DeserializeObject<ProtoMod>(File.ReadAllText(jsonFile));
                proto.path = Path.GetDirectoryName(jsonFile);
                proto.ValidateFields();
                return proto;

            }

            public override String ToString()
            {
                return $"{id} {version}";
            }

            void ValidateFields()
            {
                id = id.ToLower();
                if (id.Contains(" ")) throw new Exception($"Invalid mod id: {id}");
            }

            public SRModInfo ToModInfo()
            {
                return new SRModInfo(id, name, author, SRModInfo.ModVersion.Parse(version));
            }
            public override int GetHashCode()
            {
                return 1877310944 + EqualityComparer<string>.Default.GetHashCode(id);
            }

            public class Comparer : IEqualityComparer<ProtoMod>
            {
                public bool Equals(ProtoMod x, ProtoMod y)
                {
                    return x.Equals(y);
                }

                public int GetHashCode(ProtoMod obj)
                {
                    return obj.GetHashCode();
                }
            }

        }
    }
}
