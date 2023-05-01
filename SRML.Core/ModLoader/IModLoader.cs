using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.ModLoader
{
    public interface IModLoader<M>
        where M : IMod
    {
        M DiscoverMod();
    }
}
