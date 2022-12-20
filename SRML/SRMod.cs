using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public SRModInfo(string modid, string name, string author, ModVersion version, string description, Dictionary<string, ModVersion> dependencies)
        {
            Id = modid;
            Name = name;
            Author = author;
            Version = version;
            Description = description;
            Dependencies = dependencies;
        }
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public ModVersion Version { get; private set; }
        public Dictionary<string, ModVersion> Dependencies { get; private set; }

        public static SRModInfo GetCurrentInfo()
        {
            var assembly = ReflectionUtils.GetRelevantAssembly();
            return SRModLoader.GetModForAssembly(assembly).ModInfo;
        }

        /// <summary>
        /// Data structure to simplify versioning and the comparing of versions
        /// </summary>
        public struct ModVersion : IComparable<ModVersion>
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

            public static ModVersion Parse(String s)
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

        internal IModEntryPoint entryPoint;
        internal ModEntryPoint entryPoint2;
        private bool useNewEntry = false;

        private static SRMod forcedContext;

        internal bool encounteredError = false;

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
            this.EntryType = entryPoint.GetType();
            if (entryPoint is ModEntryPoint)
            {
                entryPoint2 = (ModEntryPoint)entryPoint;
                useNewEntry = true;
            }
            this.entryPoint = entryPoint;
        }

        public SRMod(SRModInfo info, IModEntryPoint entryPoint, string path) : this(info, entryPoint)
        {
            this.Path = path;
        }

        public void PreLoad() => entryPoint.PreLoad();

        public void Load() => entryPoint.Load();

        public void PostLoad() => entryPoint.PostLoad();

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
