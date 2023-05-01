namespace SRML.Core.ModLoader
{
    public interface IMod
    {
        IEntryPoint Entry { get; set; }
        IModInfo ModInfo { get; set; }

        void Initialize();
    }
}
