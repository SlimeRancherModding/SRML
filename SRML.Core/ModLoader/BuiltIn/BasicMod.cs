using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.ModLoader.BuiltIn
{
    public class BasicMod : IMod
    {
        public IEntryPoint Entry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IModInfo ModInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize()
        {
        }
    }
}
