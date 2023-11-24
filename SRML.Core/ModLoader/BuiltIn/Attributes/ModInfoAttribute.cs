using SRML.Core.ModLoader.Attributes;
using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.BuiltIn.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ModInfoAttribute : BaseModInfoAttribute
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }

        public ModInfoAttribute(Type entryType, string id, string version, string name, string description, string author) :
            this(entryType, id, version, name, description, author, null, null, null) { }

        public ModInfoAttribute(Type entryType, string id, string version, string name, string description, string author,
            string[] loadBefore = null, string[] loadAfter = null, Dictionary<string, string> dependencies = null) : 
            base(entryType, id, version, loadBefore, loadAfter, dependencies)
        {
            Name = name;
            Description = description;
            Author = author;
        }
    }
}
