namespace SRML.Core.API
{
    public class CoreAPI : ClassSingleton<CoreAPI>
    {
        public delegate void APIProcessor(IRegistry registry);
        public event APIProcessor ProcessAPIs;

        public void RegisterRegistry(IRegistry registry)
        {
            registry.Initialize();
            ProcessAPIs?.Invoke(registry);
        }
    }
}
