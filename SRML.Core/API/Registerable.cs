using System.Reflection;

namespace SRML.Core.API
{
    public abstract class Registerable
    {
        public abstract void Register(object originalClass, object[] args);
        public abstract bool IsRegistered();
    }
}
