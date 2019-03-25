using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Registry;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
namespace SRML.SR.SaveSystem
{
    public static class SaveRegistry
    {
        internal static Dictionary<SRMod,ModSaveInfo> modToSaveInfo = new Dictionary<SRMod, ModSaveInfo>();

        internal static ModSaveInfo GetSaveInfo(SRMod mod)
        {
            if (!modToSaveInfo.ContainsKey(mod)) modToSaveInfo.Add(mod,new ModSaveInfo());
            return modToSaveInfo[mod];
        }

        internal static ModSaveInfo GetSaveInfo()
        {
            return GetSaveInfo(SRMod.GetCurrentMod());
        }


        public static bool IsCustom(object data)
        {
            return modToSaveInfo.Any((x)=>x.Value.BelongsToMe(data)) || HasModdedID(data);
        }

        public static bool HasModdedID(object data)
        {
            return (data is VanillaActorData dat && IdentifiablePatcher.IsModdedIdentifiable((Identifiable.Id)dat.typeId));
        }


        internal static SRMod ModForModelType(Type model)
        {
            foreach (var v in modToSaveInfo)
            {
                if (v.Value.IsModelRegistered(model)) return v.Key;
            }

            return null;
        }

        internal static SRMod ModForData(object data)
        {
            if (!IsCustom(data)) return null;
            if (data is IDataRegistryMember model) return ModForModelType(model.GetModelType());
            if(data is VanillaActorData dat) return IdentifiablePatcher.moddedIdentifiables[(Identifiable.Id) dat.typeId];
            return null;
        }

        public static void RegisterSerializableModel<T>(int id) where T : ActorModel, ISerializableModel
        {
            GetSaveInfo().GetRegistryFor<CustomActorData>().AddCustomData<T>(id, () => new BinaryActorData<T>());
        }

    }
}
