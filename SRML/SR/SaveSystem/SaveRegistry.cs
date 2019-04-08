using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Registry;
using SRML.SR.SaveSystem.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV06;
namespace SRML.SR.SaveSystem
{
    public static class SaveRegistry
    {
        internal static Dictionary<SRMod,ModSaveInfo> modToSaveInfo = new Dictionary<SRMod, ModSaveInfo>();

        internal static Dictionary<Type,IIDRegistry> moddedIdRegistries = new Dictionary<Type, IIDRegistry>();

        internal static void RegisterIDRegistry(IIDRegistry registry)
        {
            moddedIdRegistries[registry.RegistryType] = registry;
        }

        internal static ModSaveInfo GetSaveInfo(SRMod mod)
        {
            if (!modToSaveInfo.ContainsKey(mod)) modToSaveInfo.Add(mod,new ModSaveInfo());
            return modToSaveInfo[mod];
        }

        internal static ModSaveInfo GetSaveInfo()
        {
            return GetSaveInfo(SRMod.GetCurrentMod());
        }


        public static bool IsFullyModdedData(object data)
        {
            return modToSaveInfo.Any((x) => x.Value.BelongsToMe(data));
        }

        public static bool IsCustom(object data)
        {
            return IsFullyModdedData(data) || HasModdedID(data) || IsModdedID(data);
        }

        public static bool HasModdedID(object data)
        {
            return (data is VanillaActorData actor && IsModdedID((Identifiable.Id)actor.typeId))||
                   (data is VanillaGadgetData gadget && IsModdedID(gadget.gadgetId));
        }

        public static bool IsModdedID(object data)
        {
            return moddedIdRegistries.Any((x) => x.Key == data.GetType() && x.Value.IsModdedID(data));
        }


        internal static SRMod ModForModelType(Type model)
        {
            foreach (var v in modToSaveInfo)
            {
                if (v.Value.IsModelRegistered(model)) return v.Key;
            }

            return null;
        }

        internal static EnumTranslator GenerateEnumTranslator()
        {
            var newTranslator = new EnumTranslator();
            foreach (var v in moddedIdRegistries)
            {
                newTranslator.GenerateTranslationTable(v.Value.GetAllModdedIDs());
            }
            return newTranslator;
        }

        internal static SRMod ModForData(object data)
        {
            if (!IsCustom(data)) return null;
            if (data is IDataRegistryMember model) return ModForModelType(model.GetModelType());
            if (data is VanillaActorData actor) return ModForID((Identifiable.Id) actor.typeId);
            if (data is VanillaGadgetData gadget) return ModForID(gadget.gadgetId); 
            return null;
        }

        internal static SRMod ModForID(object data)
        {
            if (!IsModdedID(data)) return null;

            return moddedIdRegistries.FirstOrDefault((x) => x.Key == data.GetType()).Value?.GetModForID(data);
        }

        public static void RegisterSerializableActorModel<T>(int id) where T : ActorModel, ISerializableModel
        {
            GetSaveInfo().GetRegistryFor<CustomActorData>().AddCustomData<T>(id, () => new BinaryActorData<T>());
        }

        public static void RegisterSerializableGadgetModel<T>(int id) where T : GadgetModel, ISerializableModel
        {
            GetSaveInfo().GetRegistryFor<CustomGadgetData>().AddCustomData<T>(id, () => new BinaryGadgetData<T>());
        }

        public static void RegisterDataParticipant<T>() where T : Component, ExtendedData.Participant
        {
            GetSaveInfo().onExtendedActorDataLoaded += (model, obj, tag) =>
            {
                if (tag.HasPiece(ExtendedDataUtils.GetParticipantName(typeof(T))))
                {
                    obj.AddComponent<T>();
                }
            };
        }

        

    }
}
