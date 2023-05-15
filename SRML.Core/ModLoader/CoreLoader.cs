using Doorstop;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.Core.ModLoader
{
    internal class CoreLoader
    {
        public List<IMod> mods = new List<IMod>();
        public Dictionary<Type, Type> modTypeForEntryType = new Dictionary<Type, Type>();

        public List<Type> registeredModTypes = new List<Type>();
        public List<Type> registeredLoaderTypes = new List<Type>();

        public List<IModLoader> loaders = new List<IModLoader>();

        public readonly string[] FORBIDDEN_IDS = new string[]
        {
            "srml",
            "unity",
            "internal",
        };

        public void RegisterModType(Type modType, Type entryType)
        {
            modTypeForEntryType.Add(entryType, modType);
            registeredModTypes.Add(modType);
        }

        public void RegisterModLoader(Type loaderType)
        {
            var loader = (IModLoader)Activator.CreateInstance(loaderType);
            loaders.Add(loader);
            registeredLoaderTypes.Add(loaderType);

            loader.Initialize();
            loader.DiscoverMods();

            mods.AddRange(loader.LoadedMods);
            Debug.Log($"loader found mod {string.Join(", ", loader.LoadedMods.Select(x => x.ModInfo.Id))}");
        }

        public IMod LoadMod(Type entryType)
        {
            Type modType = modTypeForEntryType.FirstOrDefault(x => entryType.IsSubclassOf(x.Key)).Value;
            var loader = loaders.FirstOrDefault(x => x.ModType == modType);
            if (loader == null)
                throw new ArgumentException($"Unable to load entry of type {entryType}; no registered loader loads mod type {entryType.BaseType}");

            return loader.LoadMod(entryType);
        }
    }
}
