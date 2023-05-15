using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.ModInfo;
using SRML.Core.ModLoader.DataTypes;
using System;

namespace SRML.Core.ModLoader.BuiltIn.Mod
{
    public class BasicMod : Mod<BasicLoadEntryPoint, BasicModInfo>, IPathProvider
    {
        public override BasicLoadEntryPoint TypeEntry { get => entry; set => entry = value; }
        public override BasicModInfo TypeInfo { get => modInfo; set => modInfo = value; }
        public string Path { get => path; set => path = value; }

        private BasicLoadEntryPoint entry;
        private BasicModInfo modInfo;
        private string path;

        public override void Initialize()
        {
        }
    }
}
