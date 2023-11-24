using Semver;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BaseModInfoAttribute : Attribute
    {
        public string ID { get; }
        public SemVersion Version { get; }
        public DependencyMetadata Dependencies { get; }
        public Type EntryType { get; }

        public BaseModInfoAttribute(Type entryType, string id, string version) : this(entryType, id, version, null, null, null) { }

        public BaseModInfoAttribute(Type entryType, string id, string version, string[] loadBefore = null, string[] loadAfter = null,
            Dictionary<string, string> dependencies = null)
        {
            EntryType = entryType;
            ID = id;
            Version = SemVersion.Parse(version, SemVersionStyles.OptionalPatch);
            Dependencies = new DependencyMetadata(id, new SemVersion(1), loadBefore ?? new string[0], loadAfter ?? new string[0],
                dependencies?.ToDictionary(x => x.Key, y => SemVersion.Parse(y.Value, SemVersionStyles.OptionalPatch)) ??
                    new Dictionary<string, SemVersion>());
        }
    }
}
