using HarmonyLib;
using SRML.Console;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.Mod;
using SRML.Core.ModLoader.BuiltIn.ModInfo;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

#pragma warning disable CS0612
namespace SRML.Core.ModLoader.BuiltIn.ModLoader
{
    public class LegacyModLoader : FileModLoader<LegacyMod, LegacyEntryPoint, DescriptiveJSONModInfo>
    {
        public override string Path => @"SRML\Mods";

        public override void Initialize()
        {
        }

        public override IModInfo LoadModInfo(Type entryType)
        {
            DescriptiveJSONModInfo info = new DescriptiveJSONModInfo();
            info.Parse(entryType.Assembly);
            return info;
        }

        public override IMod LoadMod(Type entryType, IModInfo modInfo)
        {
            LegacyMod mod = new LegacyMod();
            Assembly asm = entryType.Assembly;
            LegacyEntryPoint entryPoint = new LegacyEntryPoint(modInfo);
            mod.Entry = entryPoint;
            mod.Path = asm.Location;

            mod.ModInfo = modInfo;

            loadedMods.Add(mod);

            entryPoint.legacyEntry = ModEntryPoint.CreateEntry(entryType, mod.ModInfo.ID);
            entryPoint.Initialize();
            mod.Initialize();

            return mod;
        }

        public override void DiscoverTypesFromAssembly(Assembly assembly)
        {
            Type entryType = assembly.ManifestModule.GetTypes().FirstOrDefault((x) => (typeof(IModEntryPoint).IsAssignableFrom(x)));
            if (entryType != null)
                CoreLoader.Instance.LoadMod(entryType);
        }
    }
}
#pragma warning restore CS0612