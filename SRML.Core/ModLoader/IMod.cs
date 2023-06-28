using System;

namespace SRML.Core.ModLoader
{
    /// <summary>
    /// The core of a mod. Contains all required info to load a mod.
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// The type of the required IEntryPoint
        /// </summary>
        Type EntryType { get; }
        /// <summary>
        /// The type of the required IModInfo
        /// </summary>
        Type InfoType { get; }

        /// <summary>
        /// The entry point of the mod. Should be assigned before initialization.
        /// </summary>
        IEntryPoint Entry { get; }
        /// <summary>
        /// The info of the mod. Should be assigned before intialization.
        /// </summary>
        IModInfo ModInfo { get; }

        /// <summary>
        /// Initializes the mod. Should be ran after IMod is initially created and its parameters are assigned.
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// The core building blocks of a mod created via the information in an IModInfo.
    /// </summary>
    /// <typeparam name="E">The type of the entry point used by this mod. Should generally be an abstract type.</typeparam>
    /// <typeparam name="I">The type of the mod info used by this mod.</typeparam>
    public abstract class Mod<E, I> : IMod
        where E : IEntryPoint
        where I : IModInfo
    {
        public Type EntryType => typeof(E);
        public Type InfoType => typeof(I);

        public IEntryPoint Entry { get; set; }
        public IModInfo ModInfo { get; set; }

        /// <summary>
        /// The mod info, cast to <see cref="Mod{E, I}.EntryType"/>.
        /// </summary>
        public E TypeEntry { get => (E)Entry; }
        /// <summary>
        /// The mod info, cast to <see cref="Mod{E, I}.InfoType"/>.
        /// </summary>
        public I TypeInfo { get => (I)ModInfo; }

        public abstract void Initialize();
    }
}
