using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.SR.SaveSystem.Data.Partial;
using UnityEngine;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV08;
using VanillaDroneData = MonomiPark.SlimeRancher.Persist.DroneGadgetV01;
namespace SRML.SR.SaveSystem.Data.Gadget
{
    internal abstract class CustomGadgetData<T> : CustomGadgetData where T : GadgetModel
    {

        public override Type GetModelType()
        {
            return typeof(T);
        }


        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public sealed override void PushCustomModel(GadgetModel model)
        {
            this.PushCustomModel((T)model);
        }

        public sealed override void PullCustomModel(GadgetModel model)
        {
            this.PullCustomModel((T)model);
        }

        public sealed override void Load(Stream stream, bool skipPayloadEnd)
        {
            base.Load(stream, false);
            var reader = new BinaryReader(stream);
            LoadCustomData(reader);
            if (!skipPayloadEnd) ReadDataPayloadEnd(reader);
        }

        public sealed override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            WriteDataPayloadEnd(writer);
            WriteCustomData(writer);
        }

    }
    public abstract class CustomGadgetData : VanillaGadgetData, IDataRegistryMember
    {
        public abstract void PullCustomModel(GadgetModel model);

        public abstract void PushCustomModel(GadgetModel model);

        public abstract void WriteCustomData(BinaryWriter writer);

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract Type GetModelType();

        internal static void RegisterGadgetThings()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, VanillaGadgetData v) =>
                {
                    v.gadgetId = translator.TranslateEnum(mode, v.gadgetId);
                    translator.FixEnumValues(mode, v.fashions);
                    v.fashions.RemoveAll(x => x == Identifiable.Id.NONE);
                    v.baitTypeId = translator.TranslateEnum(mode, v.baitTypeId);
                    v.gordoTypeId = translator.TranslateEnum(mode, v.gordoTypeId);
                    if(v.drone!=null) translator.FixEnumValues(mode,v.drone);
                });

            EnumTranslator.RegisterEnumFixer((EnumTranslator translator, EnumTranslator.TranslationMode mode,
                VanillaDroneData v) =>
            {
                translator.FixEnumValues(mode,v.drone.fashions);
                v.drone.fashions.RemoveAll(x => x == Identifiable.Id.NONE);
            });
            PartialGadgetData.RegisterEnumFixer();
            PartialDroneData.RegisterEnumFixer();
            PartialData.RegisterPartialData(() => new PartialGadgetData());
            PartialData.RegisterPartialData(() => new PartialDroneData());
            CustomChecker.RegisterCustomChecker((VanillaGadgetData data) =>
            {
                if (ModdedIDRegistry.IsModdedID(data.gadgetId)) return CustomChecker.CustomLevel.FULL;
                if (ModdedIDRegistry.IsModdedID(data.baitTypeId) || ModdedIDRegistry.IsModdedID(data.gordoTypeId))
                    return CustomChecker.CustomLevel.PARTIAL;
                if (data.fashions.Any(ModdedIDRegistry.IsModdedID)) return CustomChecker.CustomLevel.PARTIAL;
                return data.drone==null? CustomChecker.CustomLevel.VANILLA : CustomChecker.GetCustomLevel(data.drone);
            });
            CustomChecker.RegisterCustomChecker((VanillaDroneData data) =>
            {
                if (data.drone.fashions.Any(ModdedIDRegistry.IsModdedID)) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
        }
    }
}
