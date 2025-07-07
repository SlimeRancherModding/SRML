using System;
using System.Linq;

namespace SRML
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ModInfoAttribute : Attribute
    {
        private readonly Type entryType;
        private readonly string id;
        private readonly string name;
        private readonly string author;
        private readonly string version;
        private readonly string description;
        private readonly string[] loadAfter;
        private readonly string[] loadBefore;
        private readonly string[] dependencies;

        internal SRModLoader.ProtoMod Parse() => new SRModLoader.ProtoMod()
        {
            entryType = entryType,
            id = id,
            name = name,
            author = author,
            version = version,
            description = description,
            load_after = loadAfter ?? new string[0],
            load_before = loadBefore ?? new string[0],
            parsedDependencies = dependencies?.Select(x =>
            {
                string[] pieces = x.Split(':');

                if (pieces.Length != 2)
                    throw new ArgumentException($"Malformed dependency for {id ?? "<unknown mod>"}: {x}");

                return new DependencyChecker.Dependency(pieces[0], pieces[1]);
            }).ToArray() ?? new DependencyChecker.Dependency[0]
        };

        /// <summary>
        /// Mod info for a mod
        /// </summary>
        /// <param name="entryType">Type of the entry point</param>
        /// <param name="id">ID for the mod</param>
        /// <param name="name">Name of the mod</param>
        /// <param name="author">Author of the mod</param>
        /// <param name="version">Version of the mod in SemVar specification</param>
        /// <param name="description">Description of the mod</param>
        /// <param name="loadAfter">Mods to load after this mod</param>
        /// <param name="loadBefore">Mods to load before this mod</param>
        /// <param name="dependencies">Mods that this mod depends on, in the format "ID:VERSION"</param>
        public ModInfoAttribute(Type entryType, string id, string name, string author, string version, 
            string description = null, string[] loadAfter = null, string[] loadBefore = null, string[] dependencies = null)
        {
            this.entryType = entryType;
            this.id = id;
            this.name = name;
            this.author = author;
            this.version = version;
            this.description = description;
            this.loadAfter = loadAfter;
            this.loadBefore = loadBefore;
            this.dependencies = dependencies;
        }
    }
}
