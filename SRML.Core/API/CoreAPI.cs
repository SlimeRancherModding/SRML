namespace SRML.Core.API
{
    public class CoreAPI
    {
        public static CoreAPI Main { get; internal set; }

        public void RegisterRegistry(IRegistry registry)
        {
            registry.Initialize();
        }
    }
}
