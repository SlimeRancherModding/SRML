using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.ModInfo;
using System;

namespace SRML.Core.ModLoader.BuiltIn
{
    public class BasicMod : IMod<BasicLoadEntryPoint, BasicModInfo>
    {
        public BasicLoadEntryPoint Entry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public BasicModInfo ModInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize()
        {
        }
    }
}
