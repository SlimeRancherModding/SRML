using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class SpawnResourceRegistry
    {
        internal static IDRegistry<SpawnResource.Id> moddedSpawnResources = new IDRegistry<SpawnResource.Id>();

        static SpawnResourceRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedSpawnResources);
            EnumPatcher.RegisterAlternate(typeof(SpawnResource.Id), (obj, name) => CreateSpawnResourceId(obj, name));
        }

        /// <summary>
        /// Creates a <see cref="SpawnResource.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="SpawnResource.Id"/>.</param>
        /// <param name="name">The name of the <see cref="SpawnResource.Id"/>.</param>
        /// <returns>The created <see cref="SpawnResource.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static SpawnResource.Id CreateSpawnResourceId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register landplot upgrades outside of the PreLoad step");
            return moddedSpawnResources.RegisterValueWithEnum((SpawnResource.Id)value, name);
        }
    }
}
