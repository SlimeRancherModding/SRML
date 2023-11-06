using SRML.Core.ModLoader.Attributes;
using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.BuiltIn.Attributes
{
    public class DescriptiveModInfoAttribute : ModInfoAttribute
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }

        public DescriptiveModInfoAttribute(string id, string version, string name, string description, string author) :
            this(id, version, name, description, author, null, null, null) { }

        public DescriptiveModInfoAttribute(string id, string version, string name, string description, string author,
            string[] loadBefore = null, string[] loadAfter = null, Dictionary<string, string> dependencies = null) : 
            base(id, version, loadBefore, loadAfter, dependencies)
        {
            Name = name;
            Description = description;
            Author = author;
        }
    }
}
