using System.Reflection;

namespace SRML.Core.API
{
    public class CoreAPI
    {
        public delegate void APIProcessor(IRegistry registry);
        public event APIProcessor ProcessAPIs;

        public static CoreAPI Instance { get; private set; }

        public void RegisterRegistry(IRegistry registry)
        {
            registry.Initialize();

            // TODO: once I replace old registry system with this, this should be changed to always run
            if (registry is IRegistryNew reg)
            {
                foreach (RegistryAttribute att in registry.GetType().GetCustomAttributes<RegistryAttribute>())
                    att.Register(reg);
            }

            ProcessAPIs?.Invoke(registry);
        }

        internal CoreAPI() => Instance = this;
    }
}
