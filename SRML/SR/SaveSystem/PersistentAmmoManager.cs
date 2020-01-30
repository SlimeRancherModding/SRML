using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.SR.SaveSystem.Format;
using SRML.SR.SaveSystem.Pipeline;
using SRML.SR.SaveSystem.Registry;
using UnityEngine;
using static PlayerState;

namespace SRML.SR.SaveSystem
{
    public static class PersistentAmmoManager
    {
        internal static readonly Dictionary<AmmoIdentifier, PersistentAmmo> PersistentAmmoData = new Dictionary<AmmoIdentifier, PersistentAmmo>();

        internal static void Pull(ModdedSaveData data)
        {
            Clear();
            foreach (var v in data.ammoDataEntries)
            {
                if (v.model.HasNoData()) continue;
                if (!v.identifier.IsValid()) continue;
                PersistentAmmoData[v.identifier] = new PersistentAmmo(v.identifier, v.model);
            }
        }

        internal static void Clear()
        {
            PersistentAmmoData.Clear();
            AmmoIdentifier.ClearCache();
        }

        internal static void OnAmmoDecrement(AmmoIdentifier id, int slot, int count)
        {
            PersistentAmmoData[id].OnDecrement(slot,count);
            if (PersistentAmmoData[id].DataModel.HasNoData()) PersistentAmmoData.Remove(id);
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
            foreach (var v in PersistentAmmoData.Where((x)=>!x.Value.DataModel.HasNoData()))
            {
                data.ammoDataEntries.Add(new IdentifiableAmmoData(){identifier = v.Key,model = v.Value.DataModel});
            }
        }

        internal static void SyncAll()
        {
            List<AmmoIdentifier> invalidIdentifiers = new List<AmmoIdentifier>();
            foreach(var v in PersistentAmmoData)
            {
                try
                {
                    if (v.Key.ResolveModel() == null) invalidIdentifiers.Add(v.Key);
                }
                catch
                {
                    Debug.LogError($"Error ocurred while attempting to resolve ammo identifier {v.Key.AmmoType} {v.Key.stringIdentifier}-{v.Key.longIdentifier}");
                    invalidIdentifiers.Add(v.Key);
                }
            }
            invalidIdentifiers.ForEach(x => PersistentAmmoData.Remove(x));
            foreach (var v in PersistentAmmoData) v.Value.Sync(true);
        }



    }
    internal class PersistentAmmoDataPipeline : SavePipeline<IdentifiableAmmoData>
    {
        public override string UniqueID => "PersistentAmmoData_pipeline";

        public override int PullPriority => 0;

        public override int LatestVersion => 0;

        public override IEnumerable<IPipelineData> Pull(ModSaveInfo mod, GameV12 data)
        {
            foreach(var v in PersistentAmmoManager.PersistentAmmoData)
            {
                var model = v.Value.DataModel.TearDataForMod(mod.ModID);
                if (model.HasNoData()) continue;
                yield return new IdentifiableAmmoData(this) { identifier = v.Key, model = model };
            }
        }

        public override IdentifiableAmmoData ReadData(BinaryReader reader, ModSaveInfo info)
        {
            var e = new IdentifiableAmmoData(this);
            e.Read(reader);
            return e;
        }

        public override void RemoveExtraModdedData(ModSaveInfo mod, GameV12 data)
        {
        }

        protected override void PushData(ModSaveInfo mod, GameV12 data, IdentifiableAmmoData item)
        {
            PersistentAmmo persistentAmmo;
            if (!PersistentAmmoManager.PersistentAmmoData.TryGetValue(item.identifier, out persistentAmmo))
            {
                persistentAmmo = new PersistentAmmo(item.identifier,item.model);
               
            }
            else
            {
                persistentAmmo.DataModel.CombineData(item.model);
            }
            Debug.Log(item.identifier);
            PersistentAmmoManager.PersistentAmmoData[item.identifier] = persistentAmmo;
        }

        protected override void WriteData(BinaryWriter writer, ModSaveInfo info, IdentifiableAmmoData item)
        {
            item.Write(writer);
        }
    }
}
