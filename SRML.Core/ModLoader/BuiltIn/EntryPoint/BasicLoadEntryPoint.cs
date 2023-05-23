using HarmonyLib;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public abstract class BasicLoadEntryPoint : Core.ModLoader.EntryPoint
    {
        internal delegate void GameContextPrefix();
        internal delegate void GameContextPostfix();
        internal static GameContextPrefix GameContextLoad;
        internal static GameContextPostfix GameContextPostLoad;

        protected BasicLoadEntryPoint(IModInfo info) : base(info)
        {
        }

        public sealed override void Initialize()
        {
            GameContextLoad += Load;
            GameContextPostLoad += PostLoad;
            PreLoad();
        }

        public abstract void PreLoad();
        public abstract void Load();
        public abstract void PostLoad();
    }
}
