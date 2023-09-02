using System;

namespace SRML.Core.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class RegistryAttribute : Attribute
    {
        public abstract void Register(IRegistryNew toRegister);
    }
}
