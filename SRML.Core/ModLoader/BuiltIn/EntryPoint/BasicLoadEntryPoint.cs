using HarmonyLib;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public abstract class BasicLoadEntryPoint : EventEntryPoint
    {
        internal delegate void GameContextPrefix();
        internal delegate void GameContextPostfix();
        internal static GameContextPrefix GameContextLoad;
        internal static GameContextPostfix GameContextPostLoad;

        public sealed override EntryEvent[] Events() => new EntryEvent[2]
        {
            new EntryEvent(true, AccessTools.Method(typeof(GameContext), "Start"), LoadAll),
            new EntryEvent(false, AccessTools.Method(typeof(GameContext), "Start"), PostLoadAll)
        };

        public sealed override void Initialize()
        {
            base.Initialize();
            GameContextLoad += Load;
            GameContextPostLoad += PostLoad;
            PreLoad();
        }

        internal static void LoadAll() => GameContextLoad();
        internal static void PostLoadAll() => GameContextPostLoad();

        public abstract void PreLoad();
        public abstract void Load();
        public abstract void PostLoad();
    }
}
