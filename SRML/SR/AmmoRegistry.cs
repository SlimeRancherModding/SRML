using SRML.SR.SaveSystem.Data.Ammo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class AmmoRegistry
    {
        internal static Dictionary<PlayerState.AmmoMode, HashSet<Identifiable.Id>> inventoryPrefabsToPatch = new Dictionary<PlayerState.AmmoMode, HashSet<Identifiable.Id>>();
        internal static List<Identifiable.Id> customRefineryResources = new List<Identifiable.Id>();
        internal static Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>> siloPrefabs = new Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>>();

        public static void RegisterAmmoPrefab(PlayerState.AmmoMode mode, GameObject prefab)
        {
            RegisterPlayerAmmo(mode, Identifiable.GetId(prefab));
        }

        public static void RegisterPlayerAmmo(PlayerState.AmmoMode mode, Identifiable.Id id)
        {
            if (!inventoryPrefabsToPatch.ContainsKey(mode))
            {
                inventoryPrefabsToPatch[mode] = new HashSet<Identifiable.Id>();
            }
            inventoryPrefabsToPatch[mode].Add(id);
        }

        public static void RegisterSiloAmmo(SiloStorage.StorageType typeId, Identifiable.Id id)
        {

            if (!siloPrefabs.ContainsKey(typeId))
            {
                siloPrefabs[typeId] = new HashSet<Identifiable.Id>();
            }
            siloPrefabs[typeId].Add(id);
        }

        public static void RegisterSiloAmmo(Predicate<SiloStorage.StorageType> pred, Identifiable.Id id)
        {
            foreach(SiloStorage.StorageType v in Enum.GetValues(typeof(SiloStorage.StorageType)))
            {
                if (pred(v)) RegisterSiloAmmo(v, id);
            }
        }

        public static void RegisterRefineryResource(Identifiable.Id id)
        {
            customRefineryResources.Add(id);
        }

    }
}
