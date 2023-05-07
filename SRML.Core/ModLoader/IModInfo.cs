using Semver;

namespace SRML.Core.ModLoader
{
    public interface IModInfo
    {
        string Id { get; }
        SemVersion Version { get; }

        void Parse(string json);
    }
}
