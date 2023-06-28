using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RegisterModType : Attribute
    {
        internal Type modType;
        internal Type entryType;

        public RegisterModType(Type modType, Type entryType)
        {
            if (!modType.IsInstanceOfType(typeof(Mod<,>)))
                throw new ArgumentException($"Cannot register mod type {modType} that does not inherit from IMod");
            if (CoreLoader.Instance.registeredModTypes.Contains(modType))
                throw new ArgumentException($"Cannot register mod type {modType} as it has already been registered.");

            this.modType = modType;
            this.entryType = entryType;
        }
    }
}
