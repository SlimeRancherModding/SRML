﻿using SRML.Core.ModLoader.Attributes;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SRML.Core.ModLoader
{
    public class CoreLoader
    {
        public static CoreLoader Instance { get; private set; }

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
        internal Dictionary<IEntryPoint, IModInfo> infoForEntry = new Dictionary<IEntryPoint, IModInfo>();
        internal Dictionary<Type, IMod> modForEntryType = new Dictionary<Type, IMod>();
        internal Dictionary<Assembly, List<IMod>> modsForAssembly = new Dictionary<Assembly, List<IMod>>();
        internal Dictionary<IMod, Assembly> assembliesForMod = new Dictionary<IMod, Assembly>();

        internal Stack<(Type, IModInfo)> modStack = new Stack<(Type, IModInfo)>();
        internal bool loadedStack = false;

        public delegate void ModProcessor(IMod mod);
        public delegate void ModPreProcessor(Type type, IModInfo info);
        /// <summary>
        /// Event that runs before a mod is loaded.
        /// </summary>
        public event ModPreProcessor PreProcessMods;
        /// <summary>
        /// Event that runs after a mod is loaded.
        /// </summary>
        public event ModProcessor ProcessMods;

        internal IMod forceMod;

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
            try
            {
                if (!typeof(IMod).IsAssignableFrom(modType))
                    throw new ArgumentException("Mod type must inherit from IMod");
                if (!typeof(IEntryPoint).IsAssignableFrom(entryType))
                    throw new ArgumentException("Entry type must inherit from IEntryPoint");

                modTypeForEntryType.Add(entryType, modType);
                registeredModTypes.Add(modType);
            }
            catch (Exception ex)
            {
                ErrorGUI.errors.Add(modType.Name, (ErrorGUI.ErrorType.RegisterModType, ex));
                modTypeForEntryType.Remove(entryType);
                registeredModTypes.Remove(modType);
            }
        }

        /// <summary>
        /// Registers a mod loader that can load a specified mod type.
        /// </summary>
        /// <param name="loaderType">The type of the mod loader</param>
        public void RegisterModLoader(Type loaderType)
        {
            try
            {
                if (!typeof(IModLoader).IsAssignableFrom(loaderType))
                    throw new ArgumentException("Loader type must inherit from IModLoader");

                var loader = (IModLoader)Activator.CreateInstance(loaderType);
                loaders.Add(loader);
                registeredLoaderTypes.Add(loaderType);

                loader.Initialize();
                loader.DiscoverMods();
            }
            catch (Exception ex)
            {
                ErrorGUI.errors.Add(loaderType.Name, (ErrorGUI.ErrorType.RegisterModLoader, ex));
                loaders.Remove(loaders.FirstOrDefault(x => x.GetType() == loaderType));
                registeredLoaderTypes.Remove(loaderType);
            }
        }

        internal void LoadFromDefaultPath()
        {
            Assembly[] potentialDlls = Directory.GetFiles(Path.GetFullPath(Core.Main.MODS_PATH), "*.dll", SearchOption.AllDirectories).Select(x => Assembly.LoadFrom(x)).ToArray();

            Assembly FindAssembly(object obj, ResolveEventArgs args)
            {
                var name = new AssemblyName(args.Name);
                return potentialDlls.FirstOrDefault(x => x.GetName() == name);
            }

            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;

            try
            {
                foreach (Assembly dll in potentialDlls)
                {
                    foreach (RegisterModLoaderTypeAttribute att in dll.GetCustomAttributes<RegisterModLoaderTypeAttribute>())
                        RegisterModLoader(att.loaderType);
                    foreach (RegisterModTypeAttribute att in dll.GetCustomAttributes<RegisterModTypeAttribute>())
                        RegisterModType(att.modType, att.entryType);
                }
            }
            finally { AppDomain.CurrentDomain.AssemblyResolve -= FindAssembly; }
        }

        internal void LoadModStack()
        {
            Stack<(Type, IModInfo)> sortedStack = new Stack<(Type, IModInfo)>();
            string[] order = DependencyMetadata.CalculateLoadOrder(modStack.Select(x => x.Item2.Dependencies).ToArray());

            for (int i = order.Length - 1; i >= 0; i--)
                sortedStack.Push(modStack.First(x => x.Item2.ID == order[i]));

            modStack = sortedStack;
            while (modStack.Count > 0)
                LoadModFromStack();

            loadedStack = true;
        }

        internal IMod LoadModFromStack()
        {
            (Type, IModInfo) protoMod = modStack.Pop();
            try
            {
                PreProcessMods?.Invoke(protoMod.Item1, protoMod.Item2);
                IMod mod = loaderForEntryType[protoMod.Item1].LoadMod(protoMod.Item1, protoMod.Item2);

                if (mod.Entry != null)
                    infoForEntry.Add(mod.Entry, protoMod.Item2);
                mods.Add(mod);

                if (!modsForAssembly.ContainsKey(protoMod.Item1.Assembly))
                    modsForAssembly[protoMod.Item1.Assembly] = new List<IMod>();
                modsForAssembly[protoMod.Item1.Assembly].Add(mod);
                assembliesForMod.Add(mod, protoMod.Item1.Assembly);

                modForEntryType[protoMod.Item1] = mod;

                ProcessMods?.Invoke(mod);

                return mod;
            }
            catch (Exception ex)
            {
                ErrorGUI.errors.Add(protoMod.Item2.ID, (ErrorGUI.ErrorType.LoadMod, ex));
            }
            return null;
        }

        /// <summary>
        /// Loads a mod.
        /// </summary>
        /// <param name="entryType">The type of the mod's entry point.</param>
        public void LoadMod(Type entryType)
        {
            try
            {
                Type modType = modTypeForEntryType.FirstOrDefault(x => entryType.IsSubclassOf(x.Key) || x.Key.IsAssignableFrom(entryType)).Value;
                var loader = loaders.FirstOrDefault(x => x.ModType == modType);
                if (loader == null)
                    throw new ArgumentException($"Unable to load entry of type {entryType}; no registered loader loads mod type {entryType.BaseType}");

                loaderForEntryType[entryType] = loader;
                modStack.Push((entryType, loader.LoadModInfo(entryType)));

                if (loadedStack)
                    LoadModFromStack();
            }
            catch (Exception ex)
            {
                ErrorGUI.AddError($"from assembly {entryType.Assembly.GetName().Name}", ErrorGUI.ErrorType.LoadMod, ex);
            }
        }

        public List<IMod> GetModsFromAssembly(Assembly assembly)
        {
            if (modsForAssembly.TryGetValue(assembly, out List<IMod> mod))
                return mod;

            return null;
        }

        public IMod GetExecutingModContext()
        {
            if (forceMod != null)
                return forceMod;

            // TODO: This is terrible and awful, try to find a better way.
            // Unfortunately, C# has probably no implemented way to do this recursively. It can only find the direct calling method.
            int recursionCheck = 0;
            foreach (StackFrame frame in new StackTrace().GetFrames())
            {
                if (recursionCheck >= 100)
                    break;

                if (modForEntryType.TryGetValue(frame.GetMethod().DeclaringType, out IMod found))
                    return found;
                recursionCheck++;
            }
            
            // TODO: PLEASE FIND BETTER WAY
            // THIS IS AWFUL
            recursionCheck = 0;
            foreach (StackFrame frame in new StackTrace().GetFrames())
            {
                if (recursionCheck >= 100)
                    break;

                if (modsForAssembly.TryGetValue(frame.GetMethod().DeclaringType.Assembly, out List<IMod> found))
                    return found.First();
                recursionCheck++;
            }

            return null;
        }

        public void ForceModContext(IMod mod) => forceMod = mod;

        public void ClearModContext() => forceMod = null;

        public IMod GetMod(string id) => Mods.FirstOrDefault(x => x.ModInfo.ID == id);

        internal CoreLoader() => Instance = this;
    }
}