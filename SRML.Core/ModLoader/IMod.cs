using System;

namespace SRML.Core.ModLoader
{
    public interface IMod
    {
        Type EntryType { get; }
        Type InfoType { get; }

        IEntryPoint Entry { get; }
        IModInfo ModInfo { get; }

        void Initialize();
    }

    public abstract class Mod<E, I> : IMod
        where E : IEntryPoint
        where I : IModInfo
    {
        public Type EntryType => typeof(E);
        public Type InfoType => typeof(I);

        public IEntryPoint Entry { get; set; }
        public IModInfo ModInfo { get; set; }

        public abstract E TypeEntry { get; set; }
        public abstract I TypeInfo { get; set; }

        public abstract void Initialize();
    }
}
