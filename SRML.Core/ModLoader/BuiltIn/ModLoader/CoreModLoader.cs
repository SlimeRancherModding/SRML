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
    public class CoreModLoader : BasicFileModLoader<CoreMod, CoreModEntryPoint, IDescriptiveModInfo>
    {
        public override string Path => @"SRML\NewMods";

        public override void Initialize()
        {
        }

        public override IModInfo LoadModInfo(Type entryType)
        {
            IDescriptiveModInfo info;

            try
            {
                DescriptiveJSONModInfo jsonInfo = new DescriptiveJSONModInfo();
                jsonInfo.Parse(entryType.Assembly);
                info = jsonInfo;
            }
            catch
            {
                DescriptiveAttributeModInfo attInfo = new DescriptiveAttributeModInfo(entryType);
                attInfo.Parse(entryType.Assembly);
                info = attInfo;
            }

            return info;
        }

        public override IMod LoadMod(Type entryType, IModInfo modInfo)
        {
            CoreMod mod = new CoreMod();
            Assembly asm = entryType.Assembly;

            CoreModEntryPoint entryPoint = (CoreModEntryPoint)Activator.CreateInstance(entryType, modInfo);
            
            mod.Entry = entryPoint;
            mod.Path = asm.Location;

            mod.ModInfo = modInfo;

            loadedMods.Add(mod);

            entryPoint.Initialize();

            mod.Initialize();
            
            return mod;
        }
    }
}
