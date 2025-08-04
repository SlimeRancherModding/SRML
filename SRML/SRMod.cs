using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;
using SRML.Config;
using SRML.Utils;

namespace SRML
{
    /// <summary>
    /// A basic mod data class that is safe to share between mods (no logic in it)
    /// </summary>
    public class SRModInfo
    {
        public SRModInfo(string modid, string name, string author, ModVersion version, string description, string url, Dictionary<string, ModVersion> dependencies)
        {
            Id = modid;
            Name = name;
            Author = author;
            Version = version;
            Description = description;
            URL = url;
            Dependencies = dependencies;
            LoadState = State.INITIALIZED;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public string URL { get; private set; }
        public ModVersion Version { get; private set; }
        public Dictionary<string, ModVersion> Dependencies { get; private set; }

        /// <summary>
        /// The current loading step the mod is on, or the step that it encountered an error.<br />
        /// Value gets set AFTER mod's corresponding loading step has ran.
        /// </summary>
        public State LoadState { get; internal set; } // putting this in the info instead of SRMod as modders MAY find it useful

        /// <summary>
        /// If the mod hasn't been unloaded and hasn't encountered an error.
        /// </summary>
        public bool IsLoaded => (int)LoadState > -1 && !EncounteredError;
        /// <summary>
        /// If the mod's loading step is one of the error states.
        /// </summary>
        public bool EncounteredError => (int)LoadState >= 900;

        /// <summary>
        /// The mod info of the mod whose code is currently running.
        /// </summary>
        /// <returns>The found mod if referenced in a mod assembly, otherwise null.</returns>
        public static SRModInfo GetCurrentInfo()
        {
            var assembly = ReflectionUtils.GetRelevantAssembly();
            return SRModLoader.GetModForAssembly(assembly)?.ModInfo;
        }

        /// <summary>
        /// Data structure to simplify versioning and the comparing of versions
        /// </summary>
        public struct ModVersion : IComparable<ModVersion> // TODO: replace with SemVer library
        {
            public readonly int Major;
            public readonly int Minor;
            public readonly int Revision;
            public static readonly ModVersion DEFAULT = new ModVersion(1, 0);
            public ModVersion(int major, int minor, int revision = 0)
            {
                Major = major;
                Minor = minor;
                Revision = revision;
            }

            public override string ToString()
            {
                return $"{Major}.{Minor}.{Revision}";
            }

            /// <summary>
            /// Parses a mod version string of the format "Major.Minor" or "Major.Minor.Revision"
            /// </summary>
            /// <param name="s">Version string in the format "Major.Minor" or "Major.Minor.Revision"</param>
            /// <returns>The parsed ModVersion</returns>
            /// <exception cref="Exception">Throws if provided version string is not in the correct format.</exception>
            public static ModVersion Parse(string s)
            {
                string[] splits = s.Split('.');
                if (splits.Length < 2 || splits.Length > 3) goto uhoh;
                if (!Int32.TryParse(splits[0], out int major)|| !Int32.TryParse(splits[1], out int minor)) goto uhoh;
                int revision = 0;
                if (splits.Length == 3 && !Int32.TryParse(splits[2], out revision)) goto uhoh;

                return new ModVersion(major, minor, revision);

                uhoh:
                throw new Exception($"Invalid Version String: {s}");
            }

            /// <summary>
            /// Compare this version to another version
            /// </summary>
            /// <param name="other"></param>
            /// <returns>-1 if other is a lower version, 1 if other is a higher version, otherwise 0</returns>
            public int CompareTo(ModVersion other)
            {
                if (Major > other.Major) return -1;
                if (Major < other.Major) return 1;
                if (Minor > other.Minor) return -1;
                if (Minor < other.Minor) return 1;
                if (Revision > other.Revision) return -1;
                if (Revision < other.Revision) return 1;
                return 0;
            }
        }

        /// <summary>
        /// A mod's state, including loading steps, errors, and being unloaded.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// User has chosen to manually disable mod.
            /// </summary>
            UNLOADED = -1,

            /// <summary>
            /// Mod has been found and loaded successfully, and its constructor has ran, but no loading steps have occurred yet.
            /// </summary>
            INITIALIZED = 0,
            /// <summary>
            /// Corresponds to <see cref="IModEntryPoint.PreLoad"/>.
            /// </summary>
            PRELOADED = 1,
            /// <summary>
            /// Corresponds to <see cref="IModEntryPoint.Load"/>.
            /// </summary>
            LOADED = 2,
            /// <summary>
            /// Corresponds to <see cref="IModEntryPoint.PostLoad"/>.
            /// </summary>
            POSTLOADED = 3,
            
            /// <summary>
            /// Generic error. This will likely never appear.
            /// </summary>
            ERROR = 900,
            /// <summary>
            /// Error with SRML attempting to parse this mod.
            /// </summary>
            INITIALIZATION_ERROR = 901,
            /// <summary>
            /// Error during mod's <see cref="IModEntryPoint.PreLoad"/>
            /// </summary>
            PRELOAD_ERROR = 902,
            /// <summary>
            /// Error during mod's <see cref="IModEntryPoint.Load"/>
            /// </summary>
            LOAD_ERROR = 903,
            /// <summary>
            /// Error during mod's <see cref="IModEntryPoint.PostLoad"/>
            /// </summary>
            POSTLOAD_ERROR = 904,
        }
    }

    /// <summary>
    /// Actual internal implementation of a mod
    /// </summary>
    internal class SRMod 
    {
        /// <summary>
        /// Mods associated SRModInfo object
        /// </summary>
        public SRModInfo ModInfo { get; private set; }

        /// <summary>
        /// Path of the mod (usually the directory where the core modinfo.json is located)
        /// </summary>
        public string Path { get; private set; }
        public List<ConfigFile> Configs { get; private set; } = new List<ConfigFile>();
        public Type EntryType { get; private set; }
        private Harmony _harmonyInstance;

        private IModEntryPoint entryPoint;
        private ModEntryPoint entryPoint2;
        private bool useNewEntry = false;

        private static SRMod forcedContext;

        public Exception exception;

        /// <summary>
        /// Gets the current executing mod as an SRMod instance 
        /// </summary>
        /// <returns>The current executing mod</returns>
        public static SRMod GetCurrentMod()
        {
            if (forcedContext != null) return forcedContext;
            return SRModLoader.GetModForAssembly(ReflectionUtils.GetRelevantAssembly());
        }

        /// <summary>
        /// Forces a certain mod to be returned from <see cref="SRMod.GetCurrentMod"/> 
        /// </summary>
        /// <param name="mod">The mod to be forced</param>
        internal static void ForceModContext(SRMod mod)
        {
            forcedContext = mod;
        }
        /// <summary>
        /// Clears the current mod context
        /// </summary>
        internal static void ClearModContext()
        {
            forcedContext = null;
        }

        public Harmony HarmonyInstance
        {
            get
            {
                if (_harmonyInstance == null)
                {
                    CreateHarmonyInstance(GetDefaultHarmonyName());
                }

                return _harmonyInstance;
            }
            private set { _harmonyInstance = value; }
        }

        public void CreateHarmonyInstance(string name)
        {
            HarmonyInstance = new Harmony(name);
        }

        public string GetDefaultHarmonyName()
        {
            return $"net.{(ModInfo.Author == null || ModInfo.Author.Length == 0 ? "srml" : Regex.Replace(ModInfo.Author, @"\s+", ""))}.{ModInfo.Id}";
        }

        public SRMod(SRModInfo info, IModEntryPoint entryPoint)
        {
            this.ModInfo = info;

            if (entryPoint != null)
            {
                this.EntryType = entryPoint.GetType();
                if (entryPoint is ModEntryPoint)
                {
                    entryPoint2 = (ModEntryPoint)entryPoint;
                    useNewEntry = true;
                }
                this.entryPoint = entryPoint;
            }
        }

        public SRMod(SRModInfo info, IModEntryPoint entryPoint, string path) : this(info, entryPoint)
        {
            this.Path = path;
        }

        public void PreLoad()
        {
            entryPoint.PreLoad();
            ModInfo.LoadState = SRModInfo.State.PRELOADED;
        }

        public void Load()
        {
            entryPoint.Load();
            ModInfo.LoadState = SRModInfo.State.LOADED;
        }

        public void PostLoad()
        {
            entryPoint.PostLoad();
            ModInfo.LoadState = SRModInfo.State.POSTLOADED;
        }

        public void Reload()
        {
            if (useNewEntry)
                entryPoint2.Reload();
        }

        public void Unload()
        {
            if (useNewEntry)
                entryPoint2.Unload();
        }

        public void Update()
        {
            if (useNewEntry)
                entryPoint2.Update();
        }

        public void FixedUpdate()
        {
            if (useNewEntry)
                entryPoint2.FixedUpdate();
        }
        
        public void LateUpdate()
        {
            if (useNewEntry)
                entryPoint2.LateUpdate();
        }
    }
}
