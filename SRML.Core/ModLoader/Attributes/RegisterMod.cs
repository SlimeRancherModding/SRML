using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RegisterMod : Attribute
    {
        internal Type entryType;

        public RegisterMod(Type entryType)
        {
            if (entryType.IsInterface | entryType.IsAbstract)
                throw new ArgumentException($"Cannot register mod with entry type {entryType}; not an actual entry");

            this.entryType = entryType;
        }
    }
}
