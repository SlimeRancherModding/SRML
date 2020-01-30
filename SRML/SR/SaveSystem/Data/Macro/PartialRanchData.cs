using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Macro
{
    public class PartialRanchData : VersionedPartialData<RanchV07>
    {
        public override int LatestVersion => 0;

        public PartialDictionary<string, SwitchHandler.State> accessDoorStates = PartialDataUtils.CreatePartialDictionaryWithStringKey(SerializerPair.GetEnumSerializerPair<SwitchHandler.State>());
        public PartialDictionary<RanchDirector.PaletteType, RanchDirector.Palette> palettes = new PartialDictionary<RanchDirector.PaletteType, RanchDirector.Palette>(ModdedIDRegistry.IsEitherModdedID,SerializerPair.GetEnumSerializerPair<RanchDirector.PaletteType>(),SerializerPair.GetEnumSerializerPair<RanchDirector.Palette>(),checkValueValid:ModdedIDRegistry.IsEitherModdedID);
        public PartialCollection<LandPlotV08> plots = new PartialCollection<LandPlotV08>(x => ModdedStringRegistry.IsValidModdedString(x.id), PartialDataUtils.CreateSerializerForPersistedDataSet<LandPlotV08>(), x => ModdedStringRegistry.IsValidModdedString(x.id));
        public PartialDictionary<string, double> ranchFastForward = PartialDataUtils.CreatePartialDictionaryWithStringKey(SerializerPair.DOUBLE);
        public override void Pull(RanchV07 data)
        {

            accessDoorStates.Pull(data.accessDoorStates);
            palettes.Pull(data.palettes);
            plots.Pull(data.plots);
            ranchFastForward.Pull(data.ranchFastForward);
        }

        public override void Push(RanchV07 data)
        {
            accessDoorStates.Push(data.accessDoorStates);
            palettes.Push(data.palettes);
            plots.Push(data.plots);
            ranchFastForward.Push(data.ranchFastForward);
        }

        public override void ReadData(BinaryReader reader)
        {
            accessDoorStates.Read(reader);
            palettes.Read(reader);
            plots.Read(reader);
            ranchFastForward.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            accessDoorStates.Write(writer);
            palettes.Write(writer);
            plots.Write(writer);
            ranchFastForward.Write(writer);
        }


        static bool GetIsCustom<K, V>(KeyValuePair<K, V> x)
        {
            return typeof(K).IsEnum ? ModdedIDRegistry.IsModdedID(x.Key) : (x.Key is string s ? ModdedStringRegistry.IsValidModdedString(s) : false);
        }
        static PartialRanchData()
        {
            PartialData.RegisterPartialData<RanchV07, PartialRanchData>();
            EnumTranslator.RegisterEnumFixer<PartialRanchData>((trans, mod, obj) =>
            {
                trans.FixEnumValues(mod,obj.accessDoorStates);
                trans.FixEnumValues(mod,obj.palettes);
                trans.FixEnumValues(mod,obj.plots);
                trans.FixEnumValues(mod,obj.ranchFastForward);
            });
            CustomChecker.RegisterCustomChecker<RanchV07>((obj) =>
            {
                if (obj.accessDoorStates.Any(x => GetIsCustom(x)) ||
                obj.palettes.Any(x => ModdedIDRegistry.IsEitherModdedID(x)) ||
                obj.plots.Any(x => ModdedStringRegistry.IsValidModdedString(x.id)) ||
                obj.ranchFastForward.Any(x => GetIsCustom(x))) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
        }
    }
}
