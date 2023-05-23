using Doorstop;
using HarmonyLib;
using SRML.Console;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.Core.ModLoader
{
    public class CoreLoader
    {
        /// <summary>
        /// All loaded mods
        /// </summary>
        public IMod[] Mods { get => mods.ToArray(); }

        internal List<IMod> mods = new List<IMod>();
        internal Dictionary<Type, Type> modTypeForEntryType = new Dictionary<Type, Type>();

        internal List<Type> registeredModTypes = new List<Type>();
        internal List<Type> registeredLoaderTypes = new List<Type>();

        internal List<IModLoader> loaders = new List<IModLoader>();
        internal Dictionary<Type, IModLoader> loaderForEntryType = new Dictionary<Type, IModLoader>();

        internal Stack<(Type, IModInfo)> modStack = new Stack<(Type, IModInfo)>();
        internal bool loadedStack = false;

        public readonly string[] FORBIDDEN_IDS = new string[]
        {
            "srml",
            "unity",
            "internal",
        };

        /// <summary>
        /// Registers a type of mod that can be loaded from a mod loader.
        /// </summary>
        /// <param name="modType">The type of the mod</param>
        /// <param name="entryType">The entry type of the mod</param>
        public void RegisterModType(Type modType, Type entryType)
        {
            if (!typeof(IMod).IsAssignableFrom(modType))
                throw new ArgumentException("Mod type must inherit from IMod");
            if (!typeof(IEntryPoint).IsAssignableFrom(entryType))
                throw new ArgumentException("Entry type must inherit from IEntryPoint");

            modTypeForEntryType.Add(entryType, modType);
            registeredModTypes.Add(modType);
        }

        /// <summary>
        /// Registers a mod loader that can load a specified mod type.
        /// </summary>
        /// <param name="loaderType">The type of the mod loader</param>
        public void RegisterModLoader(Type loaderType)
        {
            if (!typeof(IModLoader).IsAssignableFrom(loaderType))
                throw new ArgumentException("Loader type must inherit from IModLoader");

            var loader = (IModLoader)Activator.CreateInstance(loaderType);
            loaders.Add(loader);
            registeredLoaderTypes.Add(loaderType);

            loader.Initialize();
            loader.DiscoverMods();
        }

        internal void LoadModStack()
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

        internal IMod LoadModFromStack()
        {
            (Type, IModInfo) protoMod = modStack.Pop();
            IMod mod = loaderForEntryType[protoMod.Item1].LoadMod(protoMod.Item1, protoMod.Item2);
            mods.Add(mod);
            return mod;
        }

        /// <summary>
        /// Loads a mod.
        /// </summary>
        /// <param name="entryType">The type of the mod's entry point.</param>
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
