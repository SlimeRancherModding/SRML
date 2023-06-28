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

namespace SRML.Core.ModLoader.BuiltIn.ModLoader
{
    public class LegacyModLoader : FileModLoader<BasicMod, LegacyEntryPoint, BasicModInfo>
    {
        public override string Path => @"SRML\Mods";

        public override void Initialize()
        {
        }

        public override IModInfo LoadModInfo(Type entryType)
        {
            throw new NotImplementedException();
        }

        public override IMod LoadMod(Type entryType, IModInfo modInfo)
        {
            BasicMod mod = new BasicMod();
            Assembly asm = entryType.Assembly;
            LegacyEntryPoint entryPoint = new LegacyEntryPoint(modInfo);
            mod.Entry = entryPoint;
            mod.Path = asm.Location;

            BasicModInfo info = new BasicModInfo();
            info.Parse(asm);
            mod.ModInfo = info;

            loadedMods.Add(mod);

            entryPoint.legacyEntry = ModEntryPoint.CreateEntry(entryType, info.Id);
            entryPoint.Initialize();

            return mod;
        }

        public override void DiscoverTypesFromAssembly(Assembly assembly)
        {
            Debug.Log($"attempting to load from {assembly.FullName}");
            Type entryType = assembly.ManifestModule.GetTypes().FirstOrDefault(x => typeof(IModEntryPoint).IsAssignableFrom(x));
            // TODO: actually get modinfo
            if (entryType != null)
                LoadMod(entryType, null);
        }
    }
}
