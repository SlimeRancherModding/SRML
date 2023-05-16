using HarmonyLib;
using SRML.Console;

namespace SRML.Core.ModLoader
{
    public interface IEntryPoint
    {
        void Initialize();
    }

    public abstract class EntryPoint : IEntryPoint
    {
        public abstract void Initialize();

        public Harmony HarmonyInstance { get; internal set; }
        public Console.Console.ConsoleInstance ConsoleInstance { get; internal set; }
    }
}
