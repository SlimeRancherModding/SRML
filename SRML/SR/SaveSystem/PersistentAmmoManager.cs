using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.SR.SaveSystem.Format;
using UnityEngine;
using static PlayerState;

namespace SRML.SR.SaveSystem
{
    public static class PersistentAmmoManager
    {
        internal static readonly Dictionary<AmmoIdentifier, PersistentAmmo> PersistentAmmoData = new Dictionary<AmmoIdentifier, PersistentAmmo>();

        internal static void Pull(ModdedSaveData data)
        {
            PersistentAmmoData.Clear();
            AmmoIdentifier.ClearCache();
            foreach (var v in data.ammoDataEntries)
            {
                if (v.model.HasNoData()) continue;
                PersistentAmmoData[v.identifier] = new PersistentAmmo(v.identifier, v.model);
            }
        }

        internal static CompoundDataPiece GetPotentialDataTag(GameObject obj)
        {
            var id = Identifiable.GetId(obj);
            return GetPotentialDataTag(id);
        }

        internal static PersistentAmmo GetPersistentAmmoForAmmo(AmmoModel model)
        {
            var identifier = AmmoIdentifier.GetIdentifier(model);
            if (identifier.AmmoType == AmmoType.NONE) return null;
            if (!PersistentAmmoData.ContainsKey(identifier))
            {
                var newData = new PersistentAmmo(identifier, new PersistentAmmoModel(model));
                PersistentAmmoData.Add(identifier,newData);
                newData.Sync();
            }

            return PersistentAmmoData[identifier];
        }

        internal static bool HasPersistentAmmo(AmmoIdentifier id)
        {
            return PersistentAmmoData.ContainsKey(id);
        }

        internal static CompoundDataPiece GetPotentialDataTag(Identifiable.Id id)
        {
            foreach (var ammo in PersistentAmmoData)
            {
                if (ammo.Value.potentialId == id)
                {
                    var piece = ammo.Value.potentialTag;
                    ammo.Value.ClearSelected();
                    return piece;
                }
            }

            return null;
        }

        internal static void Push(ModdedSaveData data)
        {
            foreach (var v in PersistentAmmoData)
            {
                data.ammoDataEntries.Add(new IdentifiableAmmoData(){identifier = v.Key,model = v.Value.DataModel});
            }
        }

        internal static void SyncAll()
        {
            foreach (var v in PersistentAmmoData) v.Value.Sync();
        }



    }
}
