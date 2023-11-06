using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterModTypeAttribute : Attribute
    {
        internal Type modType;
        internal Type entryType;

        public RegisterModTypeAttribute(Type modType, Type entryType)
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
