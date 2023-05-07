using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.ModLoader
{
    public interface IModLoader<M, E, I>
        where M : IMod<E, I>
        where E : IEntryPoint
        where I : IModInfo
    {
        M DiscoverMod();
    }
}
