using Semver;
using SRML.Core.ModLoader.DataTypes;

namespace SRML.Core.ModLoader
{
    public interface IModInfo
    {
        string Id { get; }
        SemVersion Version { get; }
        DependencyMetadata Dependencies { get; }

        void Parse(string json);
    }
}
