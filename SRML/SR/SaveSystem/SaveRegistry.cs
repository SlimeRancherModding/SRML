using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Data.LandPlot;
using SRML.SR.SaveSystem.Registry;
using SRML.SR.SaveSystem.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV09;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV08;
using VanillaLandPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
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


        public static bool IsFullyModdedData(object data)
        {
            return modToSaveInfo.Any((x) => x.Value.BelongsToMe(data));
        }

        public static bool IsCustom(object data)
        {
            if (data == null) return false;
            if (data.GetType().IsEnum)
                throw new Exception("IsCustom for enums has been deprecated, use ModdedIDRegistry");
            if (data.GetType().IsValueType) throw new Exception("Custom Value Type " + data.GetType());
            return IsFullyModdedData(data) || ModdedIDRegistry.HasModdedID(data);
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
            foreach (var v in ModdedIDRegistry.moddedIdRegistries)
            {
                newTranslator.GenerateTranslationTable(v.Value.GetAllModdedIDs());
            }
            return newTranslator;
        }

        internal static SRMod ModForData(object data)
        {
            if (!IsCustom(data)) return null;
            if (data is IDataRegistryMember model) return ModForModelType(model.GetModelType());
            if (data is VanillaActorData actor) return ModdedIDRegistry.ModForID((Identifiable.Id) actor.typeId);
            if (data is VanillaGadgetData gadget) return ModdedIDRegistry.ModForID(gadget.gadgetId);
            if (data is VanillaLandPlotData plot) return ModdedIDRegistry.ModForID(plot.typeId);
            return null;
        }

        /// <summary>
        /// Register a serializable <see cref="ActorModel"/>
        /// </summary>
        /// <typeparam name="T">The actor model to register</typeparam>
        /// <param name="id">The mod specific integer ID that the save system will use to refer to this <see cref="ActorModel"/></param>
        public static void RegisterSerializableActorModel<T>(int id) where T : ActorModel, ISerializableModel
        {
            GetSaveInfo().GetRegistryFor<CustomActorData>().AddCustomData<T>(id, () => new BinaryActorData<T>());
        }

        public static void RegisterSerializableGadgetModel<T>(int id) where T : GadgetModel, ISerializableModel
        {
            GetSaveInfo().GetRegistryFor<CustomGadgetData>().AddCustomData<T>(id, () => new BinaryGadgetData<T>());
        }

        public static void RegisterSerializableLandPlotModel<T>(int id) where T : LandPlotModel, ISerializableModel
        {
            GetSaveInfo().GetRegistryFor<CustomLandPlotData>().AddCustomData<T>(id, () => new BinaryLandPlotData<T>());
        }

        /// <summary>
        /// Register a <see cref="Component"/> that will take part in the extended data system
        /// </summary>
        /// <typeparam name="T">Type of the participant to register</typeparam>
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

        public static void RegisterWorldDataPreLoadDelegate(WorldDataPreLoadDelegate del)
        {
            GetSaveInfo(SRMod.GetCurrentMod()).OnDataPreload += del;
        }
        public static void RegisterWorldDataSaveDelegate(WorldDataSaveDelegate del)
        {
            GetSaveInfo(SRMod.GetCurrentMod()).OnWorldSave += del;
        }
        public static void RegisterWorldDataLoadDelegate(WorldDataLoadDelegate del)
        {
            GetSaveInfo(SRMod.GetCurrentMod()).OnDataLoad += del;
        }

    }
}
