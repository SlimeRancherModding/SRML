using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Harmony;
using SRML.Utils;

namespace SRML
{
    public class SRModInfo
    {
        public SRModInfo(string modid,string name, string author, ModVersion version,string description)
        {
            Id = modid;
            Name = name;
            Author = author;
            Version = version;
            Description = description;
        }
        public String Id { get; private set; }
        public String Name { get; private set; }
        public String Author { get; private set; }
        public String Description { get; private set; }
        public ModVersion Version { get; private set; }

        public static SRModInfo GetCurrentInfo()
        {
            var assembly = ReflectionUtils.GetRelevantAssembly();
            return SRModLoader.GetModForAssembly(assembly).ModInfo;
        }

        public struct ModVersion : IComparable<ModVersion>
        {
            public int Major;
            public int Minor;
            public int Revision;
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

    internal class SRMod 
    {

        public SRModInfo ModInfo { get; private set; }
        public String Path { get; private set; }
        public Type EntryType { get; private set; }
        private HarmonyInstance _harmonyInstance;

        private IModEntryPoint entryPoint;

        private static SRMod forcedContext;

        public static SRMod GetCurrentMod()
        {
            if (forcedContext != null) return forcedContext;
            return SRModLoader.GetModForAssembly(ReflectionUtils.GetRelevantAssembly());
        }

        internal static void ForceModContext(SRMod mod)
        {
            forcedContext = mod;
        }

        internal static void ClearModContext()
        {
            forcedContext = null;
        }

        public HarmonyInstance HarmonyInstance
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

        public void CreateHarmonyInstance(String name)
        {
            HarmonyInstance = HarmonyInstance.Create(name);
        }

        public String GetDefaultHarmonyName()
        {
            return $"net.{(ModInfo.Author==null||ModInfo.Author.Length==0?"srml":Regex.Replace(ModInfo.Author, @"\s+", ""))}.{ModInfo.Id}";
        }

        public SRMod(SRModInfo info,IModEntryPoint entryPoint)
        {
            this.ModInfo = info;
            this.EntryType = entryPoint.GetType();
            this.entryPoint = entryPoint;
        }

        public SRMod(SRModInfo info, IModEntryPoint entryPoint, String path) : this(info, entryPoint)
        {
            this.Path = path;
        }

        public void PreLoad()
        {
            entryPoint.PreLoad();
        }

        public void Load()
        {
            entryPoint.Load();
        }

        public void PostLoad()
        {
            entryPoint.PostLoad();
        }
    }
    
}
