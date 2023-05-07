namespace SRML.Core.ModLoader
{
    public interface IMod<E, I>
        where E : IEntryPoint
        where I : IModInfo
    {
        E Entry { get; set; }
        I ModInfo { get; set; }

        void Initialize();
    }
}
