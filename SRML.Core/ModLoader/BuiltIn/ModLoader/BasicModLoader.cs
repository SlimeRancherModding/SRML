﻿using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.Mod;
using SRML.Core.ModLoader.BuiltIn.ModInfo;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRML.Core.ModLoader.BuiltIn.ModLoader
{
    public class BasicModLoader : BasicFileModLoader<BasicMod, BasicLoadEntryPoint, IDescriptiveModInfo>
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
                FileJSONModInfo jsonInfo = new FileJSONModInfo();
                jsonInfo.Parse(entryType.Assembly);
                info = jsonInfo;
            }
            catch (MissingModInfoException)
            {
                try
                {
                    AttributeModInfo attInfo = new AttributeModInfo(entryType);
                    attInfo.Parse(entryType.Assembly);
                    info = attInfo;
                }
                catch (MissingModInfoException)
                {
                    try
                    {

                        EmbeddedFileJSONModInfo jsonInfo = new EmbeddedFileJSONModInfo(entryType);
                        jsonInfo.Parse(entryType.Assembly);
                        info = jsonInfo;
                    }
                    catch (MissingModInfoException)
                    {
                        throw new MissingModInfoException("EntryPoint does not have an associated mod info");
                    }
                }
            }
            
            return info;
        }

        public override IMod LoadMod(Type entryType, IModInfo modInfo)
        {
            BasicMod mod = new BasicMod();
            Assembly asm = entryType.Assembly;
            
            BasicLoadEntryPoint entryPoint = (BasicLoadEntryPoint)Activator.CreateInstance(entryType, modInfo);
            
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