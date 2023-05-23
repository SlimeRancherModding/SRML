using Semver;
using SRML.Core.ModLoader.DataTypes;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.ModInfo
{
    public abstract class JSONModInfo : IModInfo
    {
        public abstract string Id { get; }

        public abstract SemVersion Version { get; }

        public abstract DependencyMetadata Dependencies { get; }

        public void Parse(Assembly modAssembly) => ParseFromJSON(GetJSON(modAssembly));

        public abstract string GetDefaultHarmonyName();
        public abstract string GetDefaultConsoleName();
        public abstract string GetJSON(Assembly modAssembly);
        public abstract void ParseFromJSON(string json);
    }
}
