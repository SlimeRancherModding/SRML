using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterModAttribute : Attribute
    {
        internal Type entryType;

        public RegisterModAttribute(Type entryType)
        {
            if (entryType.IsInterface | entryType.IsAbstract)
                throw new ArgumentException($"Cannot register mod with entry type {entryType}; not an actual entry");

            this.entryType = entryType;
        }
    }
}
