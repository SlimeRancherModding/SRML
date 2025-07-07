using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using SRML.Utils;
using SRML.Utils.Enum;
using SRML.Config;
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
            foreach (string jsonFile in Directory.GetFiles(FileSystem.ModPath, ModJson, SearchOption.AllDirectories))
            {
                var mod = ProtoMod.ParseFromJson(jsonFile);
                    throw new Exception($"Found mod with duplicate id {mod.id} in {jsonFile}!");
            }

            // process mods with embedded modinfo.jsons
            foreach (string dllFile in Directory.GetFiles(FileSystem.ModPath, "*.dll", SearchOption.AllDirectories))
            {
                if (!ProtoMod.TryParseFromDLL(dllFile, out ProtoMod[] mods)) 
                    continue;

                foreach (ProtoMod mod in mods)
                {
                    if (!foundMods.Add(mod))
                        throw new Exception($"Found mod with duplicate id {mod.id} in {dllFile}!");
                }
            }
            
            // Make sure all dependencies are in order, otherwise throw an exception from checkdependencies
            DependencyChecker.CheckDependencies(foundMods);
            DependencyChecker.CalculateLoadOrder(foundMods, loadOrder);

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

        internal static bool TryGetEntryType(Assembly assembly, out Type entryType)
        {
            // as attribute modinfos are loaded BEFORE an embedded modinfo.json, they get first pick over the entrypoints in the mod
            // this way, a modinfo.json can load one mod, and the rest can be loaded by attributes
            // I don't know why you'd ever want to do this, but you sure can!
            entryType = assembly.ManifestModule.GetTypes().FirstOrDefault(x => !Mods.Any(y => y.Value.EntryType == x) && typeof(IModEntryPoint).IsAssignableFrom(x));
            return entryType != default;
        }

        static void DiscoverAndLoadAssemblies(ICollection<ProtoMod> protomods)
        {
            HashSet<AssemblyInfo> foundAssemblies = new HashSet<AssemblyInfo>();
            foreach (ProtoMod mod in protomods)
            {
                if (mod.type == ProtoMod.InfoType.FILE_JSON)
                {
                    foreach (string file in Directory.GetFiles(mod.path, "*.dll", SearchOption.AllDirectories))
                    {
                        // ensure no assemblies get found multiple times
                        if (!foundAssemblies.Any(x => x.Path == Path.GetFullPath(file)))
                            foundAssemblies.Add(new AssemblyInfo(AssemblyName.GetAssemblyName(Path.GetFullPath(file)), Path.GetFullPath(file), mod));
                    }
                }
                else
                {
                    string fullpath = Path.Combine(mod.path, mod.entryFile);

                    // there's a chance a file JSON already claimed this assembly as its own, so overwrite that if that's the case
                    AssemblyInfo existingInfo = foundAssemblies.FirstOrDefault(x => x.Path == fullpath);
                    if (existingInfo != null)
                        existingInfo.mod = mod;
                    else
                        foundAssemblies.Add(new AssemblyInfo(AssemblyName.GetAssemblyName(fullpath), fullpath, mod));
                }
            }

            Assembly FindAssembly(object obj, ResolveEventArgs args) => foundAssemblies.FirstOrDefault((x) => x.DoesMatch(new AssemblyName(args.Name)))?.LoadAssembly();
            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;

            try
            {
                // load all assemblies related to a mod
                // if none of these assemblies are a mod assembly, something went wrong
                foreach (ProtoMod mod in protomods)
                {
                    SRMod newMod = null;

                    foreach (AssemblyInfo assembly in foundAssemblies.Where((x) => x.mod == mod))
                    {
                        Assembly a = assembly.LoadAssembly(); // always load assemblies just for the sake of them being in memory
                        if (newMod != null || assembly.IsModAssembly || !TryGetEntryType(a, out Type entryType) || 
                            (mod.type == ProtoMod.InfoType.EMBEDDED_JSON && Path.GetFullPath(assembly.Path) != Path.GetFullPath(Path.Combine(mod.path, mod.entryFile)))) 
                            continue;
                        
                        assembly.IsModAssembly = true;
                        
                        if (mod.entryType != null)
                            entryType = mod.entryType;
                        
                        newMod = AddMod(assembly.mod, entryType);
                        HarmonyOverrideHandler.LoadOverrides(entryType.Module);
                    }

                    if (newMod == null)
                        throw new EntryPointNotFoundException($"Could not find assembly for mod '{mod}'");
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

                var newmod = new SRMod(modInfo.ToModInfo(), entryPoint, Path.Combine(modInfo.path, modInfo.entryFile));
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
            public string[] load_after;
            public string[] load_before;

            [JsonExtensionData]
            public IDictionary<string, JToken> dependencies;
            [JsonIgnore]
            public DependencyChecker.Dependency[] parsedDependencies;

            public string path;
            public string entryFile;
            public InfoType type = InfoType.FILE_JSON;

            public Type entryType;

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
                ProtoMod proto = JsonConvert.DeserializeObject<ProtoMod>(jsonData, new ProtoModConverter());
                proto.path = Path.GetDirectoryName(path);
                proto.entryFile = Path.GetFileName(path);
                proto.ValidateFields();
                return proto;
            }

            /// <summary>
            /// Try to create protomods from a DLL's attributes and embedded files
            /// </summary>
            /// <param name="dllFile">Path to the DLL file to process</param>
            /// <param name="mods">The parsed <see cref="ProtoMod"/>s</param>
            /// <returns>Whether the parsing was successful</returns>
            public static bool TryParseFromDLL(string dllFile, out ProtoMod[] mods)
            {
                Assembly assembly = Assembly.LoadFile(dllFile);
                List<ProtoMod> modList = new List<ProtoMod>();

                foreach (ModInfoAttribute att in assembly.GetCustomAttributes<ModInfoAttribute>())
                {
                    ProtoMod mod = att.Parse();

                    mod.type = InfoType.ATTRIBUTE;
                    mod.path = Path.GetDirectoryName(dllFile);
                    mod.entryFile = Path.GetFileName(dllFile);

                    modList.Add(mod);
                }

                if (assembly.GetManifestResourceNames().FirstOrDefault((x) => x.EndsWith("modinfo.json")) is string fileName)
                {
                    ProtoMod mod = null;
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(fileName)))
                        mod = ParseFromJson(reader.ReadToEnd(), dllFile);

                    mod.type = InfoType.EMBEDDED_JSON;

                    modList.Add(mod);
                }

                mods = modList.ToArray();
                return mods.Length > 0;
            }

            public override string ToString() => $"{id} {version}";

            /// <summary>
            /// Make sure fields are in the correct form and not null
            /// </summary>
            void ValidateFields()
            {
                if (id == null) 
                    throw new Exception($"{path} is missing an id field!");
                if (id.Contains(" "))
                    throw new Exception($"Invalid mod id: {id}");
                id = id.ToLower();
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

            public enum InfoType
            {
                FILE_JSON,
                EMBEDDED_JSON,
                ATTRIBUTE
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
