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

            foreach (RegisterModLoaderType att in assembly.GetCustomAttributes<RegisterModLoaderType>())
                loader.RegisterModLoader(att.loaderType);
            foreach (RegisterModType att in assembly.GetCustomAttributes<RegisterModType>())
                loader.RegisterModType(att.modType, att.entryType);
            foreach (RegisterMod att in assembly.GetCustomAttributes<RegisterMod>())
                loader.LoadMod(att.entryType);
        }
    }
}
