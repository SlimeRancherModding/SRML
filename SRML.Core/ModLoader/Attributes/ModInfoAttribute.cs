using Semver;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModInfoAttribute : Attribute
    {
        public string ID { get; }
        public SemVersion Version { get; }
        public DependencyMetadata Dependencies { get; }
        public Type EntryType { get; }

        public ModInfoAttribute(string id, string version) : this(id, version, null, null, null) { }

        public ModInfoAttribute(string id, string version, string[] loadBefore = null, string[] loadAfter = null, 
            Dictionary<string, string> dependencies = null)
        {
            ID = id;
            Version = SemVersion.Parse(version, SemVersionStyles.OptionalPatch);
            Dependencies = new DependencyMetadata(id, Version, loadBefore ?? new string[0], loadAfter ?? new string[0],
                dependencies.ToDictionary(x => x.Key, y => SemVersion.Parse(y.Value, SemVersionStyles.OptionalPatch)) ?? 
                    new Dictionary<string, SemVersion>());
        }
    }
}
