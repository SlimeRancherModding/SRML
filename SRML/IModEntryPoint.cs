using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using SRML.Console;

namespace SRML
{
    public interface IModEntryPoint
    {
        /// <summary>
        /// Called before <see cref="GameContext.Awake"/><br/>
        /// Used to register things such as enum values, and to do Harmony patching
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Called before <see cref="GameContext.Start"/><br/>
        /// Used to register things that require a loaded GameContext
        /// </summary>
        void Load();

        /// <summary>
        /// Called after every mod's Load has finished (not a registry step)<br/>
        /// Used for editing existing assets in the game
        /// </summary>
        void PostLoad();
    }

    public abstract class ModEntryPoint : IModEntryPoint
    {
        public Harmony HarmonyInstance => harmony;
        internal Harmony harmony;

        public Console.Console.ConsoleInstance ConsoleInstance { get; internal set; }

        public static IModEntryPoint CreateEntry(Type entryType, string id)
        {
            IModEntryPoint entryPoint = (IModEntryPoint)Activator.CreateInstance(entryType);

            if (entryPoint is ModEntryPoint entry)
            {
                Colors col = Colors.lime;
                string name = null;

                foreach (var att in entryType.GetCustomAttributes())
                {
                    if (att.GetType() == typeof(ConsoleAppearance))
                    {
                        ConsoleAppearance app = (ConsoleAppearance)att;
                        col = app.consoleCol;
                        name = app.name;
                    }
                }

                entry.harmony = HarmonyPatcher.SetInstanceForType(entryType, $"net.srml.{id}");
                entry.ConsoleInstance = new Console.Console.ConsoleInstance(name ?? id, col, $"{id}.main");
            }

            return entryPoint;
        }

        public static T Get<T>() where T : IModEntryPoint
        {
            SRMod selectedMod = SRModLoader.Mods.FirstOrDefault(x => x.Value.EntryType == typeof(T)).Value;
            if (selectedMod == null)
                throw new EntryPointNotFoundException();

            return (T)(selectedMod.entryPoint2 ?? selectedMod.entryPoint);
        }

        public static IModEntryPoint Get(string modId)
        {
            SRMod selectedMod = SRModLoader.Mods[modId];
            if (selectedMod == null)
                throw new ArgumentException();

            return selectedMod.entryPoint2 ?? selectedMod.entryPoint;
        }

        public virtual void PreLoad()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void PostLoad()
        {
        }

        public virtual void Reload()
        {
        }

        public virtual void Unload()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }
    }
}
