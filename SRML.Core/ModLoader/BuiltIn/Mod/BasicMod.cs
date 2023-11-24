using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.ModInfo;
using SRML.Core.ModLoader.DataTypes;
using System;

namespace SRML.Core.ModLoader.BuiltIn.Mod
{
    public class BasicMod : Mod<BasicLoadEntryPoint, FileJSONModInfo>, IPathProvider
    {
        public string Path { get => path; set => path = value; }
        private string path;

        public override void Initialize()
        {
        }
    }
}
