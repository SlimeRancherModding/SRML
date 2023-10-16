using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRML.SR.SaveSystem
{
    internal static class DronePersistentAmmoManager
    {
        public static (CompoundDataPiece, Identifiable.Id) recentlyRemoved;
        public static (GameObject, long) recentlySpawnedWithoutData;
        public static (Ammo, int) seeking;
        public static DroneNetwork.StorageMetadata currMetadata;

        private static List<DroneNetwork.StorageMetadata> storages = new List<DroneNetwork.StorageMetadata>();
        private static Dictionary<Ammo, DroneNetwork.StorageMetadata> storageForAmmo = new Dictionary<Ammo, DroneNetwork.StorageMetadata>();

        public static void RegisterStorage(DroneNetwork.StorageMetadata storage) => storages.AddIfDoesNotContain(storage);

        public static bool SameStorage(Ammo ammo)
        {
            if (storageForAmmo.ContainsKey(ammo))
                return true;

            var storage = storages.FirstOrDefault(x => x.ammo == ammo);
            if (storage != null)
            {
                storageForAmmo.Add(ammo, storage);
                return true;
            }

            return false;
        }

        public static void OnIncrement(Ammo ammo, int slot, bool repeat)
        {
            seeking = (ammo, slot);
            Associate(repeat);
        }

        public static void OnDecrement(CompoundDataPiece removedPiece, Identifiable.Id id)
        {
            recentlyRemoved = (removedPiece, id);
            Associate(false);
        }

        public static void OnActorSpawned(GameObject go, long actorId)
        {
            recentlySpawnedWithoutData = (go, actorId);
            Associate(false);
        }

        public static void Associate(bool repeat)
        {
            if (recentlyRemoved.Item1 == null)
                return;

            if (seeking.Item1 != null)
            {
                if (seeking.Item1.Slots[seeking.Item2].id != recentlyRemoved.Item2)
                    return;

                PersistentAmmo ammo = PersistentAmmoManager.GetPersistentAmmoForAmmo(seeking.Item1.ammoModel);
                ammo.DataModel.PushDataForSlot(seeking.Item2, recentlyRemoved.Item1);
                ammo.Sync();

                if (!repeat)
                    recentlyRemoved = (null, Identifiable.Id.NONE);
                seeking = (null, -1);
            }
            else if (recentlySpawnedWithoutData.Item1 != null)
            {
                if (Identifiable.GetId(recentlySpawnedWithoutData.Item1) != recentlyRemoved.Item2)
                    return;

                ExtendedData.preparedData[DataIdentifier.GetActorIdentifier(recentlySpawnedWithoutData.Item2)] = new ExtendedData.PreparedData() 
                { 
                    SourceType = ExtendedData.PreparedData.PreparationSource.AMMO, 
                    Data = recentlyRemoved.Item1 
                };

                recentlyRemoved = (null, Identifiable.Id.NONE);
                recentlySpawnedWithoutData = (null, 0);
            }
        }
    }
}
