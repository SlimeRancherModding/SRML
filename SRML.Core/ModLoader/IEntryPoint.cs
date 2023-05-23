using HarmonyLib;
using SRML.Console;
using static ModDirector;

namespace SRML.Core.ModLoader
{
    public interface IEntryPoint
    {
        /// <summary>
        /// Loads the entry point. Should be called on <see cref="IMod.Initialize"/>
        /// </summary>
        void Initialize();
    }

    public abstract class EntryPoint : IEntryPoint
    {
        public abstract void Initialize();

        /// <summary>
        /// The Harmony created for a mod.
        /// </summary>
        public Harmony HarmonyInstance { get; internal set; }
        /// <summary>
        /// A console logger created for a mod based on the mod info.
        /// </summary>
        public Console.Console.ConsoleInstance ConsoleInstance { get; internal set; }

        protected EntryPoint(IModInfo info)
        {
            HarmonyInstance = HarmonyPatcher.SetInstance(info.GetDefaultHarmonyName());
            ConsoleInstance = Console.Console.ConsoleInstance.PopulateConsoleInstanceFromType(GetType(), info.GetDefaultConsoleName());
        }
    }
}
