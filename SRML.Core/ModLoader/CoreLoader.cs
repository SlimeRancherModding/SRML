using System.Collections.Generic;

namespace SRML.Core.ModLoader
{
    internal class CoreLoader
    {
        public List<IMod<IEntryPoint,IModInfo>> mods = new List<IMod<IEntryPoint, IModInfo>>();
    }
}
