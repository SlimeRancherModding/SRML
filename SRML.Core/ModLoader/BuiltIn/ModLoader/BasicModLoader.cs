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
    public class BasicModLoader : BasicFileModLoader<BasicMod, BasicLoadEntryPoint, BasicModInfo>
    {
        public override string Path => @"SRML\NewMods";

        public override void Initialize()
        {
        }

        public override IModInfo LoadModInfo(Type entryType)
        {
            BasicModInfo info = new BasicModInfo();
            Assembly asm = entryType.Assembly;

            if (asm.GetManifestResourceNames().FirstOrDefault((x) => x.EndsWith("modinfo.json")) is string fileName)
            {
                using (StreamReader reader = new StreamReader(asm.GetManifestResourceStream(fileName)))
                    info.Parse(reader.ReadToEnd());
            }

            return info;
        }

        public override IMod LoadMod(Type entryType, IModInfo modInfo)
        {
            BasicMod mod = new BasicMod();
            Assembly asm = entryType.Assembly;
            
            BasicLoadEntryPoint entryPoint = (BasicLoadEntryPoint)Activator.CreateInstance(entryType);
            
            mod.Entry = entryPoint;
            mod.Path = asm.Location;

            mod.ModInfo = modInfo;

            loadedMods.Add(mod);

            entryPoint.HarmonyInstance = HarmonyPatcher.SetInstance(((BasicModInfo)modInfo).GetDefaultHarmonyName());
            entryPoint.ConsoleInstance = Console.Console.ConsoleInstance.PopulateConsoleInstanceFromType(entryType, modInfo.Id);
            entryPoint.Initialize();
            
            return mod;
        }
    }
}
