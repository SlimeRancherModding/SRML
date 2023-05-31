using Doorstop;
using SRML.Core.ModLoader.Attributes;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.ModLoader.BuiltIn.ModLoader
{
    public abstract class BasicFileModLoader<M, E, I> : FileModLoader<M, E, I>
        where M : IMod, IPathProvider
        where E : IEntryPoint
        where I : IModInfo
    {
        public sealed override void DiscoverTypesFromAssembly(Assembly assembly)
        {
            CoreLoader loader = CoreLoader.Main;

            foreach (RegisterMod att in assembly.GetCustomAttributes<RegisterMod>())
            {
                if (typeof(E).IsAssignableFrom(att.entryType))
                    loader.LoadMod(att.entryType);
            }
        }
    }
}
