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
            ProcessAPIs?.Invoke(registry);
        }

        internal CoreAPI() => Instance = this;
    }
}
