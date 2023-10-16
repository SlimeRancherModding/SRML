using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Format;
using SRML.Utils;
using UnityEngine;
using VanillaLandPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
namespace SRML.SR.SaveSystem.Data.LandPlot
{
    internal class PartialLandPlotData : VersionedPartialData<VanillaLandPlotData>
    {
        public override int LatestVersion => 0;


        public SpawnResource.Id attachedId;
        public PartialCollection<global::LandPlot.Upgrade> upgrades = new PartialCollection<global::LandPlot.Upgrade>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<global::LandPlot.Upgrade>());
        public override void Pull(VanillaLandPlotData data)
        {

            attachedId = GiveBackIfModded(data.attachedId);
            data.attachedId = GiveNoneIfModded(data.attachedId);
            upgrades.Pull(data.upgrades);
        }

        static T GiveBackIfModded<T>(T a)
        {
            return ModdedIDRegistry.IsModdedID(a) ? a : (T)(object)0;
        }

        static T GiveNoneIfModded<T>(T a)
        {
            return ModdedIDRegistry.IsModdedID(a) ? (T)(object)0 : a;
        }


        public override void Push(VanillaLandPlotData data)
        {
            if (attachedId != SpawnResource.Id.NONE) data.attachedId = attachedId;
            upgrades.Push(data.upgrades);
        }

        public override void ReadData(BinaryReader reader)
        {
            if (ModdedSaveData.LATEST_READ_VERSION > 3) Version = reader.ReadInt32();
            attachedId = (SpawnResource.Id)reader.ReadInt32();
            upgrades.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write((int)attachedId);
            upgrades.Write(writer);
        }

        public override void Write(BinaryWriter writer)
        {
            WriteData(writer);
        }

        public override void Read(BinaryReader reader)
        {
            ReadData(reader);
        }

        static PartialLandPlotData()
        {
            EnumTranslator.RegisterEnumFixer<PartialLandPlotData>((translator, mode, data) => 
            {
                translator.FixEnumValues(mode, data.upgrades);
                while (data.upgrades.InternalList.Contains(global::LandPlot.Upgrade.NONE)) data.upgrades.InternalList.Remove(global::LandPlot.Upgrade.NONE);
                data.attachedId = translator.TranslateEnum(mode, data.attachedId);
            });
            
            PartialData.RegisterPartialData(() => new PartialLandPlotData());
        }
    }
}
