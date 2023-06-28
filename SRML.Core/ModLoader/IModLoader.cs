using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader
{
    public interface IModLoader
    {
        /// <summary>
        /// The base type of mod to load. Should inherit from <see cref="IMod"/>
        /// </summary>
        Type ModType { get; }
        /// <summary>
        /// An array of loaded mods. Mods loaded during <see cref="IModLoader.LoadMod(Type, IModInfo)"/> should be added to this.
        /// </summary>
        IMod[] LoadedMods { get; }

        /// <summary>
        /// Initializes the mod loader. Called upon registration of the loader.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Discover any mod types from somewhere. Called directly after initialization.
        /// </summary>
        void DiscoverMods();
        /// <summary>
        /// Loads the mod info based on the entry type. Should be loaded before the main mod class.
        /// </summary>
        /// <param name="entryType">The type of a mod's entry point.</param>
        /// <returns>The loaded mod info.</returns>
        IModInfo LoadModInfo(Type entryType);
        /// <summary>
        /// Loads a mod based on the entry type and previously-loaded mod info.
        /// </summary>
        /// <param name="entryType">The type of a mod's entry point.</param>
        /// <param name="modInfo">The previously-loaded mod info for this mod.</param>
        /// <returns>The loaded mod</returns>
        IMod LoadMod(Type entryType, IModInfo modInfo);
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

        public abstract IModInfo LoadModInfo(Type entryType);

        public abstract IMod LoadMod(Type entryType, IModInfo info);
    }
}
