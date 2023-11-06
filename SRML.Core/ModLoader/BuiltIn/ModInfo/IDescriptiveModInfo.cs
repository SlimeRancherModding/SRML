using Semver;
using SRML.Core.ModLoader.DataTypes;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public interface IDescriptiveModInfo : IModInfo
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
    }
}
