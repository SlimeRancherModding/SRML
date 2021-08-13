﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;
using SRML.Utils;
using Exception = System.Exception;
using SRML.Utils.Enum;
using System.Collections.ObjectModel;
using SRML.Config;

namespace SRML
{
    public static class SRModLoader
    {
        internal const string ModJson = "modinfo.json";

        static readonly Dictionary<string,SRMod> Mods = new Dictionary<string, SRMod>();

        public static IEnumerable<SRModInfo> LoadedMods => Mods.Select(x => x.Value.ModInfo);

        private static readonly List<string> loadOrder = new List<string>();
        
        public static LoadingStep CurrentLoadingStep { get; private set; }

        /// <summary>
        /// Searches for valid mods and their assemblies, and decides the load order based on their settings
        /// </summary>
        internal static void InitializeMods()
        {
            FileSystem.CheckDirectory(FileSystem.ModPath);
            HashSet<ProtoMod> foundMods = new HashSet<ProtoMod>(new ProtoMod.Comparer());

            // process mods without embedded modinfo.jsons
            foreach (var jsonFile in Directory.GetFiles(FileSystem.ModPath, ModJson, SearchOption.AllDirectories))
            {
                var mod = ProtoMod.ParseFromJson(jsonFile);
                if (!foundMods.Add(mod))
                {
                    throw new Exception("Found mod with duplicate id '"+mod.id+"' in "+jsonFile+"!");
                }



            }
            // process mods with embedded modinfo.jsons
            foreach (var dllFile in Directory.GetFiles(FileSystem.ModPath, "*.dll", SearchOption.AllDirectories))
            {
                if(!ProtoMod.TryParseFromDLL(dllFile,out var mod)||mod.id==null) continue;
                if (!foundMods.Add(mod))
                {
                    throw new Exception("Found mod with duplicate id '" + mod.id + "' in " + dllFile + "!");
                }
            }

            
            // Make sure all dependencies are in order, otherwise throw an exception from checkdependencies
            DependencyChecker.CheckDependencies(foundMods);
            
            DependencyChecker.CalculateLoadOrder(foundMods,loadOrder);

            // Start loading the assemblies
            DiscoverAndLoadAssemblies(foundMods);
        }


        /// <summary>
        /// Check if <paramref name="modid"/> corresponds with a valid mod
        /// </summary>
        /// <param name="modid">Mod ID to check</param>
        /// <returns>Whether or not the mod exists</returns>
        public static bool IsModPresent(string modid) => loadOrder.Any((x) => modid == x);


        /// <summary>
        /// Gets the associated <see cref="SRModInfo"/> for the associated <paramref name="modid"/>
        /// </summary>
        /// <param name="modid">Relevant Mod ID</param>
        /// <returns>The associated ModInfo</returns>
        public static SRModInfo GetModInfo(string modid) => Mods.TryGetValue(modid, out var mod) ? mod.ModInfo : null;

        internal static bool TryGetEntryType(Assembly assembly,out Type entryType)
        {
            entryType = assembly.ManifestModule.GetTypes()
                .FirstOrDefault((x) => (typeof(IModEntryPoint).IsAssignableFrom(x)));
            return entryType != default(Type);
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
                return foundAssemblies.FirstOrDefault((x) => x.DoesMatch(name))?.LoadAssembly();
            }

            

            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;
            try
            {
                foreach (var mod in protomods)
                {
                    foreach (var assembly in foundAssemblies.Where((x)=>x.mod==mod))
                    {
                        var a = assembly.LoadAssembly();
                        if (!TryGetEntryType(a, out var entryType)||assembly.IsModAssembly||(!mod.isFromJSON&&Path.GetFullPath(assembly.Path)!=Path.GetFullPath(mod.entryFile))) continue;
                        assembly.IsModAssembly = true;
                        var newMod = AddMod(assembly.mod, entryType);
                        HarmonyOverrideHandler.LoadOverrides(entryType.Module);
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
        
        /// <summary>
        /// Get an <see cref="SRMod"/> instance from a Mod ID
        /// </summary>
        /// <param name="id">The ModID</param>
        /// <returns>The corresponding <see cref="SRMod"/> instance, or null</returns>
        internal static SRMod GetMod(string id)
        {
            return Mods.TryGetValue(id,out var mod)?mod:null;
        }

        internal static SRMod GetModForAssembly(Assembly a)
        {
            return Mods.FirstOrDefault((x) => x.Value.EntryType.Assembly == a).Value;
        }

        internal static ICollection<SRMod> GetMods()
        {
            return Mods.Values;
        }

        static SRMod AddMod(ProtoMod modInfo, Type entryType)
        {
            IModEntryPoint entryPoint = (IModEntryPoint) Activator.CreateInstance(entryType);
            var newmod = new SRMod(modInfo.ToModInfo(), entryPoint,modInfo.path);
            Mods.Add(modInfo.id,newmod);
            return newmod;
        }

        internal static void PreLoadMods()
        {

            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    EnumHolderResolver.RegisterAllEnums(mod.EntryType.Module);
                    ConfigManager.PopulateConfigs(mod);
                    mod.PreLoad();
                    Console.Console.Reload += () =>
                    {
                        SRMod.ForceModContext(mod);
                        foreach(var v in mod.Configs)
                        {
                            v.TryLoadFromFile();
                        }
                        SRMod.ClearModContext();
                    };
                }
                catch (Exception e)
                {
                    throw new Exception($"Error pre-loading mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
               
            }
        }
        
        internal static void LoadMods()
        {
            CurrentLoadingStep = LoadingStep.LOAD;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    mod.Load();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error loading mod '{modid}'!\n{e.GetType().Name}: {e}");
                }

            }
        }

        internal static void PostLoadMods()
        {
            CurrentLoadingStep = LoadingStep.POSTLOAD;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    mod.PostLoad();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error post-loading mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
            }

            CurrentLoadingStep = LoadingStep.FINISHED;
        }
        /// <summary>
        /// Utility class to help with the discovery and loading of mod assemblies
        /// </summary>
        internal class AssemblyInfo
        {
            public AssemblyName AssemblyName;
            public String Path;
            public ProtoMod mod;
            public bool IsModAssembly;
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

        public enum LoadingStep
        {
            PRELOAD,
            LOAD,
            POSTLOAD,
            FINISHED
        }

        /// <summary>
        /// Class that represents a mod before it has been loaded or fully processed
        /// </summary>
        internal class ProtoMod
        {
            public string id;
            public string name;
            public string author;
            public string version;
            public string description;
            public string path;
            public string[] dependencies;
            public string[] load_after;
            public string[] load_before;



            public bool isFromJSON = true;
            public string entryFile;
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
            /// <summary>
            /// Create a protomod from json info
            /// </summary>
            /// <param name="jsonFile">Path of the json file</param>
            /// <returns>The parsed <see cref="ProtoMod"/></returns>
            public static ProtoMod ParseFromJson(String jsonFile)
            {


                return ParseFromJson(File.ReadAllText(jsonFile), jsonFile);

            }

            public static ProtoMod ParseFromJson(String jsonData,string path)
            {

                var proto =
                    JsonConvert.DeserializeObject<ProtoMod>(jsonData);
                proto.path = Path.GetDirectoryName(path);
                proto.entryFile = path;
                proto.ValidateFields();
                return proto;

            }
            /// <summary>
            /// Try to create a protomod from an embedded modinfo json in a DLL
            /// </summary>
            /// <param name="dllFile">Path to the DLL file to process</param>
            /// <param name="mod">The parsed <see cref="ProtoMod"/>, or null</param>
            /// <returns>Whether the parsing was successful</returns>
            public static bool TryParseFromDLL(String dllFile,out ProtoMod mod)
            {
                
                var assembly = Assembly.LoadFile(dllFile);
                
                mod = new ProtoMod();
                mod.isFromJSON = false;
                mod.path = Path.GetDirectoryName(dllFile);
                mod.entryFile = dllFile;
                if (assembly.GetManifestResourceNames().FirstOrDefault((x) => x.EndsWith("modinfo.json")) is string
                    fileName)
                {
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(fileName)))
                    {
                        mod = ParseFromJson(reader.ReadToEnd(), dllFile);
                        mod.isFromJSON = false;
                    }
                }
                else return false;


                return true;
            }

            public override String ToString()
            {
                return $"{id} {version}";
            }
            /// <summary>
            /// Make sure no fields are null and in the correct form
            /// </summary>
            void ValidateFields()
            {
                if (id == null) throw new Exception($"{path} is missing an id field!");
                id = id.ToLower();
                if (id.Contains(" ")) throw new Exception($"Invalid mod id: {id}");
                load_after = load_after ?? new string[0];
                load_before = load_before ?? new string[0];
            }
            /// <summary>
            /// Turn the protomod into a proper <see cref="SRModInfo"/> instance
            /// </summary>
            /// <returns>Converted <see cref="SRModInfo"/></returns>
            public SRModInfo ToModInfo()
            {
                return new SRModInfo(id, name, author, SRModInfo.ModVersion.Parse(version),description);
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
