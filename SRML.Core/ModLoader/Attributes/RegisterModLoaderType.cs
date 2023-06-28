using System;
using System.Collections.Generic;

namespace SRML.Core.ModLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RegisterModLoaderType : Attribute
    {
        internal Type loaderType;
        internal Type modType;

        public RegisterModLoaderType(Type loaderType, Type modType)
        {
            if (!loaderType.IsInstanceOfType(typeof(ModLoader<,,>)))
                throw new ArgumentException($"Cannot register mod loader type {loaderType} that does not inherit from IModLoader.");
            if (CoreLoader.Instance.registeredLoaderTypes.Contains(loaderType))
                throw new ArgumentException($"Cannot register mod loader type {loaderType} as it has already been registered.");

            this.loaderType = loaderType;
            this.modType = modType;
        }
    }
}
