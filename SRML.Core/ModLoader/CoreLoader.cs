using Doorstop;
using HarmonyLib;
using SRML.Core.ModLoader.DataTypes;
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
        public Dictionary<Type, IModLoader> loaderForEntryType = new Dictionary<Type, IModLoader>();

        internal Stack<(Type, IModInfo)> modStack = new Stack<(Type, IModInfo)>();
        internal bool loadedStack = false;

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
        }

        public void LoadModStack()
        {
            Stack<(Type, IModInfo)> sortedStack = new Stack<(Type, IModInfo)>();
            string[] order = DependencyMetadata.CalculateLoadOrder(modStack.Select(x => x.Item2.Dependencies).ToArray());

            for (int i = order.Length - 1; i >= 0; i--)
                sortedStack.Push(modStack.First(x => x.Item2.Id == order[i]));

            modStack = sortedStack;
            while (modStack.Count > 0)
                LoadModFromStack();

            loadedStack = true;
        }

        public IMod LoadModFromStack()
        {
            (Type, IModInfo) protoMod = modStack.Pop();
            IMod mod = loaderForEntryType[protoMod.Item1].LoadMod(protoMod.Item1, protoMod.Item2);
            mods.Add(mod);
            return mod;
        }

        public void LoadMod(Type entryType)
        {
            Type modType = modTypeForEntryType.FirstOrDefault(x => entryType.IsSubclassOf(x.Key)).Value;
            var loader = loaders.FirstOrDefault(x => x.ModType == modType);
            if (loader == null)
                throw new ArgumentException($"Unable to load entry of type {entryType}; no registered loader loads mod type {entryType.BaseType}");

            loaderForEntryType[entryType] = loader;
            modStack.Push((entryType, loader.LoadModInfo(entryType)));

            if (loadedStack)
                LoadModFromStack();
        }
    }
}
