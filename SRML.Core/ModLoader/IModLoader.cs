using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader
{
    public interface IModLoader
    {
        Type ModType { get; }
        IMod[] LoadedMods { get; }

        void Initialize();
        void DiscoverMods();
        IMod LoadMod(Type entryType);
    }

    public abstract class ModLoader<M, E, I> : IModLoader
        where M : IMod
        where E : IEntryPoint
        where I : IModInfo
    {
        public Type ModType => typeof(M);

        public IMod[] LoadedMods => loadedMods.ToArray();
        internal List<IMod> loadedMods = new List<IMod>();

        public abstract void DiscoverMods();

        public abstract void Initialize();

        public abstract IMod LoadMod(Type entryType);
    }
}
