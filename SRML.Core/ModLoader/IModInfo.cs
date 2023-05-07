namespace SRML.Core.ModLoader
{
    public interface IModInfo
    {
        string Id { get; }

        void Parse(string json);
    }
}
