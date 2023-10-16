using System;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Registry;
using VanillaLandPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;

namespace SRML.SR.SaveSystem.Data.LandPlot
{
    internal abstract class CustomLandPlotData<T> : CustomLandPlotData where T : LandPlotModel
    {

        public override Type GetModelType()
        {
            return typeof(T);
        }


        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public sealed override void PushCustomModel(LandPlotModel model)
        {
            this.PushCustomModel((T)model);
        }

        public sealed override void PullCustomModel(LandPlotModel model)
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

    public abstract class CustomLandPlotData : VanillaLandPlotData, IDataRegistryMember
    {
        public abstract void PullCustomModel(LandPlotModel model);

        public abstract void PushCustomModel(LandPlotModel model);

        public abstract void WriteCustomData(BinaryWriter writer);

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract Type GetModelType();

        static CustomLandPlotData()
        {
            CustomChecker.RegisterCustomChecker<VanillaLandPlotData>(x =>
            {
                if (ModdedIDRegistry.IsModdedID(x.typeId)) return CustomChecker.CustomLevel.FULL;
                if (x.upgrades.Any(ModdedIDRegistry.IsModdedID)) return CustomChecker.CustomLevel.PARTIAL;
                if (ModdedIDRegistry.IsModdedID(x.attachedId)) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
            EnumTranslator.RegisterEnumFixer<VanillaLandPlotData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.upgrades);
                data.upgrades.RemoveAll(x => x == global::LandPlot.Upgrade.NONE); ;
                data.attachedId = translator.TranslateEnum<SpawnResource.Id>(mode, data.attachedId);
            });

        }
    }
}
