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
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using static ModDirector;

namespace SRML
{
    public static class SRModLoader
    {
        internal const string ModJson = "modinfo.json";

        internal static readonly Dictionary<string, SRMod> Mods = new Dictionary<string, SRMod>();

        public static IEnumerable<SRModInfo> LoadedMods => Mods.Select(x => x.Value.ModInfo);

        private static List<string> loadOrder = new List<string>();

        public static LoadingStep CurrentLoadingStep { get; private set; }

        /// <summary>
        /// Searches for valid mods and their assemblies, and decides the load order based on their settings
        /// </summary>
        internal static void InitializeMods()
        {
            CurrentLoadingStep = LoadingStep.INITIALIZATION;

            FileSystem.CheckDirectory(FileSystem.ModPath);
            HashSet<ProtoMod> foundMods = new HashSet<ProtoMod>(new ProtoMod.Comparer());

            // process mods with embedded modinfo.jsons
            foreach (string dllFile in Directory.GetFiles(FileSystem.ModPath, "*.dll", SearchOption.AllDirectories))
            {
                if (!ProtoMod.TryParseFromDLL(dllFile, out ProtoMod[] mods))
                    continue;

                foreach (ProtoMod mod in mods)
                {
                    if (!foundMods.Add(mod))
                        mod.encounteredError = new Exception($"Found mod with duplicate id {mod.id} in {dllFile}!");
                }
            }

            // process mods without embedded modinfo.jsons
            foreach (string jsonFile in Directory.GetFiles(FileSystem.ModPath, ModJson, SearchOption.AllDirectories))
            {
                ProtoMod mod = ProtoMod.ParseFromJson(jsonFile);
                if (foundMods.Add(mod))
                    mod.encounteredError = new Exception($"Found mod with duplicate id {mod.id} in {jsonFile}!");
            }

            foreach (ProtoMod mod in foundMods)
            {
                try
                {
                    mod.ValidateFields();
                }
                catch (Exception e) { mod.encounteredError = e; }
            }

            DependencyChecker.CheckDependencies(foundMods);

            // Start loading the assemblies
            // mods are currently in an order that ensures attribute modinfos get first picks
            DiscoverAndLoadAssemblies(foundMods);

            // now that every assembly has been found and verified, loading can commence in order
            DependencyChecker.CalculateLoadOrder(ref foundMods, out loadOrder);
            AddMods(foundMods);
        }

        /// <summary>
        /// Check if <paramref name="modid"/> corresponds to any loaded mod
        /// </summary>
        /// <param name="modid">Mod ID to check</param>
        /// <returns>Whether or not the mod is loaded</returns>
        public static bool IsModLoaded(string modid) => LoadedMods.FirstOrDefault(x => x.Id == modid)?.IsLoaded ?? false;

        /// <summary>
        /// Check if <paramref name="modid"/> corresponds with a valid mod
        /// </summary>
        /// <param name="modid">Mod ID to check</param>
        /// <returns>Whether or not the mod exists</returns>
        public static bool IsModPresent(string modid) => Mods.Keys.Any((x) => modid == x);


        /// <summary>
        /// Gets the associated <see cref="SRModInfo"/> for the associated <paramref name="modid"/>
        /// </summary>
        /// <param name="modid">Relevant Mod ID</param>
        /// <returns>The associated ModInfo</returns>
        public static SRModInfo GetModInfo(string modid) => Mods.TryGetValue(modid, out var mod) ? mod.ModInfo : null;

        internal static bool TryGetEntryType(Assembly assembly, out Type entryType, IEnumerable<ProtoMod> otherPMs = null)
        {
            entryType = assembly.ManifestModule.GetTypes().FirstOrDefault(x => !otherPMs.Any(z => z.entryType == x) && !Mods.Any(y => y.Value.EntryType == x) && typeof(IModEntryPoint).IsAssignableFrom(x));
            return entryType != default;
        }

        internal static void DiscoverAndLoadAssemblies(ICollection<ProtoMod> protomods)
        {
            HashSet<AssemblyInfo> foundAssemblies = new HashSet<AssemblyInfo>();
            foreach (ProtoMod mod in protomods)
            {
                if (mod.encounteredError != null)
                    continue;

                if (mod.type == ProtoMod.InfoType.FILE_JSON)
                {
                    foreach (string file in Directory.GetFiles(mod.path, "*.dll", SearchOption.AllDirectories))
                    {
                        AssemblyInfo existing = foundAssemblies.FirstOrDefault(x => x.Path == Path.GetFullPath(file));
                        if (existing != null)
                            existing.externalJsonMod = mod;
                        else
                            foundAssemblies.Add(new AssemblyInfo(AssemblyName.GetAssemblyName(file), file, mod, true));
                    }
                }
                else
                {
                    string fullpath = Path.Combine(mod.path, mod.entryFile);

                    // there's a chance a file JSON already claimed this assembly as its own, so overwrite that if that's the case
                    AssemblyInfo existing = foundAssemblies.FirstOrDefault(x => x.Path == fullpath);
                    if (existing != null)
                        existing.mods.Add(mod);
                    else
                        foundAssemblies.Add(new AssemblyInfo(AssemblyName.GetAssemblyName(fullpath), fullpath, mod));
                }
            }

            Assembly FindAssembly(object obj, ResolveEventArgs args) => foundAssemblies.FirstOrDefault((x) => x.DoesMatch(new AssemblyName(args.Name)))?.LoadAssembly();
            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;

            try
            {
                // find an entrytype for each mod
                // attribute mods gets first pick for mod entry points, followed by embedded, followed by external
                foreach (AssemblyInfo assembly in foundAssemblies)
                {
                    Assembly a = assembly.LoadAssembly();
                    foreach (ProtoMod mod in assembly.AllMods)
                    {
                        if ((mod.entryType != null && mod.entryType.Assembly.FullName != a.FullName) || !TryGetEntryType(a, out Type entryType, protomods))
                            continue;

                        assembly.IsModAssembly = true;
                        mod.entryType = mod.entryType ?? entryType;
                    }
                }

                foreach (ProtoMod mod in protomods)
                {
                    if (mod.encounteredError == null && mod.entryType == null)
                        mod.encounteredError = new EntryPointNotFoundException($"Could not find a suitable entry point for '{mod}'");
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= FindAssembly;
            }
        }

        internal static void AddMods(ICollection<ProtoMod> mods)
        {
            foreach (ProtoMod mod in mods)
            {
                AddMod(mod, mod.entryType);

                if (mod.entryType != null)
                    HarmonyOverrideHandler.LoadOverrides(mod.entryType.Module);
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
            return Mods.FirstOrDefault((x) => x.Value.EntryType?.Assembly == a).Value;
        }

        internal static ICollection<SRMod> GetMods()
        {
            return Mods.Values;
        }

        static SRMod AddMod(ProtoMod modInfo, Type entryType)
        {
            SRModInfo parsedModInfo = modInfo.ToModInfo();
            IModEntryPoint entryPoint = null;

            if (modInfo.encounteredError == null)
            {
                try
                {
                    entryPoint = (IModEntryPoint)Activator.CreateInstance(entryType);

                    if (entryPoint is ModEntryPoint)
                        ((ModEntryPoint)entryPoint).ConsoleInstance = new Console.Console.ConsoleInstance(modInfo.name);
                }
                catch (Exception ex)
                {
                    modInfo.encounteredError = ex;
                    UnityEngine.Debug.LogError(ex);
                }
            }

            SRMod newmod = new SRMod(parsedModInfo, entryPoint, Path.Combine(modInfo.path, modInfo.entryFile));
            newmod.exception = modInfo.encounteredError;
            newmod.ModInfo.LoadState = modInfo.encounteredError == null ? SRModInfo.State.INITIALIZED : SRModInfo.State.INITIALIZATION_ERROR;

            Mods.Add(modInfo.id, newmod);
            return newmod;
        }

        internal static void PreLoadMods()
        {
            CurrentLoadingStep = LoadingStep.PRELOAD;
            Console.Console.Reload += Main.Reload;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

                try
                {
                    EnumHolderResolver.RegisterAllEnums(mod.EntryType.Module);
                    ConfigManager.PopulateConfigs(mod);
                    mod.PreLoad();
                }
                catch (Exception e)
                {
                    mod.ModInfo.LoadState = SRModInfo.State.PRELOAD_ERROR;
                    mod.exception = e;

                    UnityEngine.Debug.LogError(e);
                }
            }
        }

        internal static void LoadMods()
        {
            CurrentLoadingStep = LoadingStep.LOAD;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

                try
                {
                    mod.Load();
                }
                catch (Exception e)
                {
                    mod.ModInfo.LoadState = SRModInfo.State.LOAD_ERROR;
                    mod.exception = e;

                    UnityEngine.Debug.LogError(e);
                }
            }
        }

        internal static void PostLoadMods()
        {
            CurrentLoadingStep = LoadingStep.POSTLOAD;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

                try
                {
                    mod.PostLoad();
                }
                catch (Exception e)
                {
                    mod.ModInfo.LoadState = SRModInfo.State.POSTLOAD_ERROR;
                    mod.exception = e;

                    UnityEngine.Debug.LogError(e);
                }
            }

            CurrentLoadingStep = LoadingStep.FINISHED;
        }

        internal static void ReloadMods()
        {
            CurrentLoadingStep = LoadingStep.RELOAD;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

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
                    throw new Exception($"Error reloading mod '{mod.ModInfo.Id}'!\n{e.GetType().Name}: {e}");
                }
            }
            CurrentLoadingStep = LoadingStep.FINISHED;
        }

        internal static void UnloadMods()
        {
            CurrentLoadingStep = LoadingStep.UNLOAD;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

                try
                {
                    mod.Unload();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error unloading mod '{mod.ModInfo.Id}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        internal static void UpdateMods()
        {
            if (CurrentLoadingStep != LoadingStep.FINISHED) return;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

                try
                {
                    mod.Update();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error updating mod '{mod.ModInfo.Id}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        internal static void UpdateModsFixed()
        {
            if (CurrentLoadingStep != LoadingStep.FINISHED) return;
            foreach (SRMod mod in Mods.Values)
            {
                try
                {
                    mod.FixedUpdate();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error fixed-updating mod '{mod.ModInfo.Id}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        internal static void UpdateModsLate()
        {
            if (CurrentLoadingStep != LoadingStep.FINISHED) return;
            foreach (SRMod mod in Mods.Values)
            {
                if (!mod.ModInfo.IsLoaded)
                    continue;

                try
                {
                    mod.LateUpdate();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error late-updating mod '{mod.ModInfo.Id}'!\n{e.GetType().Name}: {e}");
                }
            }
        }

        /// <summary>
        /// Utility class to help with the discovery and loading of mod assemblies
        /// </summary>
        internal class AssemblyInfo
        {
            public AssemblyName AssemblyName;
            public string Path;

            public ProtoMod externalJsonMod;
            public List<ProtoMod> mods;
            public bool IsModAssembly;

            public ProtoMod[] AllMods
            {
                get
                {
                    ProtoMod[] allMods = new ProtoMod[mods.Count + (externalJsonMod == null ? 0 : 1)];

                    Array.Copy(mods.ToArray(), allMods, mods.Count);
                    if (externalJsonMod != null)
                        allMods[mods.Count] = externalJsonMod;

                    return allMods;
                }
            }

            public AssemblyInfo(AssemblyName name, string path)
            {
                AssemblyName = name;
                Path = path;

                mods = new List<ProtoMod>();
            }

            public AssemblyInfo(AssemblyName name, string path, ProtoMod mod, bool isExternal = false) : this(name, path)
            {
                if (isExternal)
                    externalJsonMod = mod;
                else
                    mods.Add(mod);
            }
            public AssemblyInfo(AssemblyName name, string path, IEnumerable<ProtoMod> mods) : this(name, path) => this.mods.AddRange(mods);

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
            INITIALIZATION,
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
            public string url;
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
            public Exception encounteredError;

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
                Assembly assembly = Assembly.LoadFrom(dllFile);
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
            public void ValidateFields()
            {
                load_after = load_after ?? new string[0];
                load_before = load_before ?? new string[0];

                if (id == null)
                    throw new Exception($"{path} is missing an id field!");
                if (id.Contains(" "))
                    throw new Exception($"Invalid mod id: {id}");
                id = id.ToLowerInvariant();
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
                SRModInfo.ModVersion version = default;
                Dictionary<string, SRModInfo.ModVersion> dependencies = new Dictionary<string, SRModInfo.ModVersion>();

                if (encounteredError == null)
                {
                    try
                    {
                        // if version doesn't parse, it doesn't matter if dependencies parse,
                        SRModInfo.ModVersion.Parse(this.version);
                        dependencies = parsedDependencies?.ToDependencyDictionary() ?? dependencies;
                    }
                    catch (Exception ex) { encounteredError = ex; }
                }

                return new SRModInfo(id, name, author, version, description, url, dependencies);
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
                        if (token.ContainsKey("url"))
                            pm.url = token["url"].ToObject<string>();
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
