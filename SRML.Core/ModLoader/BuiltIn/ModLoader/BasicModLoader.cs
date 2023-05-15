using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.Mod;
using SRML.Core.ModLoader.BuiltIn.ModInfo;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRML.Core.ModLoader.BuiltIn.ModLoader
{
    public class BasicModLoader : FileModLoader<BasicMod, BasicLoadEntryPoint, BasicModInfo>
    {
        public override string Path => @"SRML\NewMods";

        public override void Initialize()
        {
        }

        public override IMod LoadMod(Type entryType)
        {
            BasicMod mod = new BasicMod();
            Assembly asm = entryType.Assembly;
            mod.Entry = (BasicLoadEntryPoint)Activator.CreateInstance(entryType);
            mod.Path = asm.Location;

            BasicModInfo info = new BasicModInfo();
            if (asm.GetManifestResourceNames().FirstOrDefault((x) => x.EndsWith("modinfo.json")) is string fileName)
            {
                using (StreamReader reader = new StreamReader(asm.GetManifestResourceStream(fileName)))
                    info.Parse(reader.ReadToEnd());
            }
            mod.ModInfo = info;

            loadedMods.Add(mod);
            mod.Entry.Initialize();
            return mod;
        }
    }
}
