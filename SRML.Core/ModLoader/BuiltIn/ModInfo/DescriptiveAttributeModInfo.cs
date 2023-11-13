using Semver;
using SRML.Core.ModLoader.BuiltIn.Attributes;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public class DescriptiveAttributeModInfo : IDescriptiveModInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public string ID { get; private set; }
        public SemVersion Version { get; private set; }
        public DependencyMetadata Dependencies { get; private set; }

        private readonly Type entryType;

        public string GetDefaultHarmonyName() => $"net.{(Author == null || Author.Length == 0 ? "srml" : Regex.Replace(Author, @"\s+", ""))}.{ID}";
        public string GetDefaultConsoleName() => Name;

        public void Parse(Assembly modAssembly)
        {
            Attribute att = entryType.GetCustomAttribute<DescriptiveModInfoAttribute>();
            if (att == null)
                throw new MissingModInfoException("Entry type does not have a DescriptiveModInfoAttribute");

            DescriptiveModInfoAttribute descAtt = (DescriptiveModInfoAttribute)att;

            Name = descAtt.Name;
            Description = descAtt.Description;
            Author = descAtt.Author;
            ID = descAtt.ID;
            Version = descAtt.Version;
            Dependencies = descAtt.Dependencies;
        }

        public DescriptiveAttributeModInfo(Type entryType) => this.entryType = entryType;
    }
}
