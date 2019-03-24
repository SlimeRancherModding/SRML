using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
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


        public static bool IsCustom(VanillaActorData data)
        {
            return data is CustomActorData<ActorModel> || IdentifiablePatcher.IsModdedIdentifiable((Identifiable.Id)data.typeId);
        }

        internal static SRMod ModForModelType(Type model)
        {
            foreach (var v in modToSaveInfo)
            {
                if (v.Value.CustomActorDataRegistry.modelTypeToIds.ContainsKey(model)) return v.Key;
            }

            return null;
        }

        internal static SRMod ModForData(VanillaActorData data)
        {
            if (!IsCustom(data)) return null;
            if (data is CustomActorData<ActorModel> model) return ModForModelType(model.GetModelType());
            return IdentifiablePatcher.moddedIdentifiables[(Identifiable.Id) data.typeId];
        }

        public static void RegisterSerializableModel<T>(int id) where T : ActorModel, ISerializableModel
        {
            GetSaveInfo().CustomActorDataRegistry.AddCustomActorData(id, () => new BinaryActorData<T>());
        }

    }
}
