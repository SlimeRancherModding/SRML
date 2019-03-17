using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace SRML
{
    public class SRModInfo
    {
        public SRModInfo(string name, string author, ModVersion version)
        {
            Name = name;
            Author = author;
            Version = version;
        }

        public String Name { get; private set; }
        public String Author { get; private set; }
        public ModVersion Version { get; private set; }

        public struct ModVersion
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
        }
    }

    internal class SRMod
    {
        public SRModInfo ModInfo { get; private set; }
        public String Assembly { get; private set; }
        public Type EntryType { get; private set; }
        private HarmonyInstance _harmonyInstance;

        private ModEntryPoint entryPoint;
        

        public HarmonyInstance HarmonyInstance
        {
            get
            {
                if (_harmonyInstance == null)
                {
                    CreateHarmonyInstance(GetHarmonyName());
                }

                return _harmonyInstance;
            }
            private set { _harmonyInstance = value; }
        }

        void CreateHarmonyInstance(String name)
        {
            HarmonyInstance = HarmonyInstance.Create(name);
        }

        public String GetHarmonyName()
        {
            return $"net.{ModInfo.Author}.{ModInfo.Name}";
        }

        SRMod(SRModInfo info)
        {
            this.ModInfo = info;
        }

        
        
        public static SRMod CreateFromEntryPoint(ModEntryPoint entryPoint)
        {
            SRMod newMod = new SRMod(entryPoint.GetModInfo());
            newMod.EntryType = entryPoint.GetType();
            newMod.Assembly = newMod.EntryType.Assembly.GetName().Name + ".dll";
            newMod.entryPoint = entryPoint;

            return newMod;
        }

    

    }
    
}
