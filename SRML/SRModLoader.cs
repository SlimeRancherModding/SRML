using System;
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
using SRML.SR;
using Newtonsoft.Json.Linq;

namespace SRML
{
    public static class SRModLoader
    {
        internal const string ModJson = "modinfo.json";

        internal static readonly Dictionary<string,SRMod> Mods = new Dictionary<string, SRMod>();

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
            try
            {
                IModEntryPoint entryPoint = (IModEntryPoint)Activator.CreateInstance(entryType);

                if (entryPoint is ModEntryPoint)
                    ((ModEntryPoint)entryPoint).ConsoleInstance = new Console.Console.ConsoleInstance(modInfo.name);

                var newmod = new SRMod(modInfo.ToModInfo(), entryPoint, modInfo.path);
                Mods.Add(modInfo.id, newmod);
                return newmod;
            }
            catch (Exception e)
            {
                throw new Exception($"Error initializing '{modInfo.id}'!: {e}");
            }
        }

        internal static void PreLoadMods()
        {
            CurrentLoadingStep = LoadingStep.PRELOAD;
            Console.Console.Reload += Main.Reload;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    EnumHolderResolver.RegisterAllEnums(mod.EntryType.Module);
                    ConfigManager.PopulateConfigs(mod);
                    mod.PreLoad();
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

        internal static void ReloadMods()
        {
            CurrentLoadingStep = LoadingStep.RELOAD;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    SRMod.ForceModContext(mod);
                    foreach (var v in mod.Configs)
                    {
                        v.TryLoadFromFile();
                    }
                    SRMod.ClearModContext();
                    mod.Reload();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error reloading mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
            }
            CurrentLoadingStep = LoadingStep.FINISHED;
        }

        internal static void UnloadMods()
        {
            CurrentLoadingStep = LoadingStep.UNLOAD;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    mod.Unload();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error unloading mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        internal static void UpdateMods()
        {
            if (CurrentLoadingStep != LoadingStep.FINISHED) return;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    mod.Update();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error updating mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        internal static void UpdateModsFixed()
        {
            if (CurrentLoadingStep != LoadingStep.FINISHED) return;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    mod.FixedUpdate();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error fixed-updating mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        internal static void UpdateModsLate()
        {
            if (CurrentLoadingStep != LoadingStep.FINISHED) return;
            foreach (var modid in loadOrder)
            {
                var mod = Mods[modid];
                try
                {
                    mod.LateUpdate();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error late-updating mod '{modid}'!\n{e.GetType().Name}: {e}");
                }
            }
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
            RELOAD,
            UNLOAD,
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
            public string[] load_after;
            public string[] load_before;

            [JsonExtensionData]
            public IDictionary<string, JToken> dependencies;
            [JsonIgnore]
            public DependencyChecker.Dependency[] parsedDependencies;

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
                    return parsedDependencies != null && parsedDependencies.Length > 0;
                }
            }

            /// <summary>
            /// Create a protomod from json info
            /// </summary>
            /// <param name="jsonFile">Path of the json file</param>
            /// <returns>The parsed <see cref="ProtoMod"/></returns>
            public static ProtoMod ParseFromJson(string jsonFile) => ParseFromJson(File.ReadAllText(jsonFile), jsonFile);

            public static ProtoMod ParseFromJson(string jsonData, string path)
            {
                var proto = JsonConvert.DeserializeObject<ProtoMod>(jsonData, new ProtoModConverter());
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

            public override string ToString() => $"{id} {version}";

            /// <summary>
            /// Make sure fields are in the correct form and not null
            /// </summary>
            void ValidateFields()
            {
                if (id == null) throw new Exception($"{path} is missing an id field!");
                id = id.ToLower();
                if (id.Contains(" ")) throw new Exception($"Invalid mod id: {id}");
                load_after = load_after ?? new string[0];
                load_before = load_before ?? new string[0];
                /*if (dependencies == null || dependencies.Count == 0) return;
                try
                {
                    List<DependencyChecker.Dependency> depends = new List<DependencyChecker.Dependency>();
                    foreach (JProperty prop in ((JObject)dependencies.First().Value).Properties()) 
                        depends.Add(new DependencyChecker.Dependency(prop.Name, prop.Value.Value<string>()));
                    parsedDependencies = depends.ToArray();
                }
                catch
                {
                    throw new Exception($"Error parsing mod dependencies for mod {id}");
                }*/
            }

            /// <summary>
            /// Turn the protomod into a proper <see cref="SRModInfo"/> instance
            /// </summary>
            /// <returns>Converted <see cref="SRModInfo"/></returns>
            public SRModInfo ToModInfo()
            {
                return new SRModInfo(id, name, author, SRModInfo.ModVersion.Parse(version), description, parsedDependencies == null ? new Dictionary<string, SRModInfo.ModVersion>() : parsedDependencies.ToDependencyDictionary());
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

            public class ProtoModConverter : JsonConverter
            {
                public override bool CanConvert(Type objectType) => objectType == typeof(ProtoMod);

                public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                {
                    ProtoMod pm = new ProtoMod();
                    JObject token = (JObject)JToken.ReadFrom(reader);

                    try
                    {
                        pm.id = token["id"].ToObject<string>();
                        pm.version = token["version"].ToObject<string>();
                        pm.name = token["name"].ToObject<string>();

                        if (token.ContainsKey("author"))
                            pm.author = token["author"].ToObject<string>();
                        if (token.ContainsKey("description"))
                            pm.description = token["description"].ToObject<string>();
                    }
                    catch (Exception e)
                    {
                        if (pm.id == null || pm.id == string.Empty)
                            throw new Exception($"Error parsing unknown basic mod information! {e}");
                        else
                            throw new Exception($"Error parsing basic mod information for {pm.id}! {e}");
                    }

                    try
                    {
                        if (token.ContainsKey("load_after"))
                            pm.load_after = token["load_after"].ToObject<string[]>();
                        if (token.ContainsKey("load_after"))
                            pm.load_after = token["load_after"].ToObject<string[]>();
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error parsing mod loading order for {pm.id}! {e}");
                    }

                    try
                    {
                        if (token.ContainsKey("dependencies"))
                        {
                            if (token["dependencies"].Type == JTokenType.Array)
                            {
                                pm.parsedDependencies = token["dependencies"].ToObject<string[]>().Select(x =>
                                    new DependencyChecker.Dependency(x.Split(' ')[0], x.Split(' ')[1])).ToArray();
                            }
                            else if (token["dependencies"].Type == JTokenType.Object)
                            {
                                pm.parsedDependencies = ((JObject)token["dependencies"]).Properties().Select(x =>
                                    new DependencyChecker.Dependency(x.Name, x.Value.ToObject<string>())).ToArray();
                            }
                            else
                            {
                                throw new InvalidOperationException($"Malformed dependencies in {pm.id}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error parsing dependencies in {pm.id}! {e}");
                    }

                    return pm;
                }

                public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                }
            }
        }
    }
}
