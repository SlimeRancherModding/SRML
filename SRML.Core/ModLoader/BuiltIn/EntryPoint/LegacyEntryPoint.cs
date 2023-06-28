using System;
using SRML;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public class LegacyEntryPoint : BasicLoadEntryPoint
    {
        public IModEntryPoint legacyEntry;

        public LegacyEntryPoint(IModInfo info) : base(info)
        {
        }

        public override void PreLoad() => legacyEntry.PreLoad();

        public override void Load() => legacyEntry.Load();

        public override void PostLoad() => legacyEntry.PostLoad();
    }
}
