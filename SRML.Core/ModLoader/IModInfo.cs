using Semver;
using SRML.Core.ModLoader.DataTypes;
using System.Reflection;

namespace SRML.Core.ModLoader
{
    public interface IModInfo
    {
        /// <summary>
        /// The unique id of the mod. If any other loaded mod has this, then loading will be aborted.
        /// </summary>
        string ID { get; }
        /// <summary>
        /// The version of the mod in SemVer specification.
        /// </summary>
        SemVersion Version { get; }
        /// <summary>
        /// The mods required for this mod to load, as well as 
        /// </summary>
        DependencyMetadata Dependencies { get; }

        /// <summary>
        /// Parses the mod info based off of the mod's assembly. Should be called in <see cref="IModLoader.LoadModInfo(System.Type)"/>
        /// </summary>
        /// <param name="modAssembly">The assembly of the mod to load the info from.</param>
        void Parse(Assembly modAssembly);

        /// <summary>
        /// Create a Harmony name based off of the info within the mod info.
        /// </summary>
        /// <returns>The default Harmony name.</returns>
        string GetDefaultHarmonyName();

        /// <summary>
        /// Create a ConsoleInstance name based off of the info within the mod info.
        /// </summary>
        /// <returns>The default Console name.</returns>
        string GetDefaultConsoleName();
    }
}
