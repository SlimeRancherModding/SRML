using HarmonyLib;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public abstract class BasicLoadEntryPoint : EventEntryPoint
    {
        public override EntryEvent[] Events() => new EntryEvent[2]
        {
            new EntryEvent(true, AccessTools.Method(typeof(GameContext), "Start"), Load),
            new EntryEvent(false, AccessTools.Method(typeof(GameContext), "Start"), PostLoad)
        };

        public override void Initialize()
        {
            base.Initialize();
            PreLoad();
        }

        public abstract void PreLoad();
        public abstract void Load();
        public abstract void PostLoad();
    }
}
