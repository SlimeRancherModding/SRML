using SRML.Core.ModLoader.Attributes;
using SRML.Core.ModLoader.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.ModLoader
{
    public abstract class FileModLoader<M, E, I> : ModLoader<M, E, I>, IPathProvider
        where M : IMod, IPathProvider
        where E : IEntryPoint
        where I : IModInfo
    {
        public abstract string Path { get; }

        public override void DiscoverMods()
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
                return;
            }

            Assembly[] potentialDlls = Directory.GetFiles(System.IO.Path.GetFullPath(Path), "*.dll", SearchOption.AllDirectories).Select(x => Assembly.LoadFrom(x)).ToArray();
            
            Assembly FindAssembly(object obj, ResolveEventArgs args)
            {
                var name = new AssemblyName(args.Name);
                return potentialDlls.FirstOrDefault(x => x.GetName() == name);
            }

            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;

            try
            {
                CoreLoader loader = Main.loader;

                foreach (Assembly dll in potentialDlls)
                {
                    foreach (RegisterModLoaderType att in dll.GetCustomAttributes<RegisterModLoaderType>())
                        loader.RegisterModLoader(att.loaderType);
                    foreach (RegisterModType att in dll.GetCustomAttributes<RegisterModType>())
                        loader.RegisterModType(att.modType, att.entryType);
                    foreach (RegisterMod att in dll.GetCustomAttributes<RegisterMod>())
                        loader.LoadMod(att.entryType);
                }
            }
            finally { AppDomain.CurrentDomain.AssemblyResolve -= FindAssembly; }
        }
    }
}
