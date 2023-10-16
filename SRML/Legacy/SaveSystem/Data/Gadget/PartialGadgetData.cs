using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using UnityEngine;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV08;
namespace SRML.SR.SaveSystem.Data.Gadget
{
    internal class PartialGadgetData : VersionedPartialData<VanillaGadgetData>
    {
        private Identifiable.Id baitTypeId;
        private Identifiable.Id gordoTypeId;
        private PartialDroneData partialDrone = new PartialDroneData();

        private PartialCollection<Identifiable.Id> fashions =
            new PartialCollection<Identifiable.Id>(ModdedIDRegistry.IsModdedID, idSerializer);
        private static readonly SerializerPair<Identifiable.Id> idSerializer =
            SerializerPair.GetEnumSerializerPair<Identifiable.Id>();

        public override int LatestVersion => 0;

        public override void Pull(VanillaGadgetData data)
        {
            baitTypeId = GiveBackIfModded(data.baitTypeId);
            data.baitTypeId = GiveNoneIfModded(data.baitTypeId);
            gordoTypeId = GiveBackIfModded(data.gordoTypeId);
            data.gordoTypeId = GiveNoneIfModded(data.gordoTypeId);
            if (data.drone!=null) partialDrone.Pull(data.drone);
            fashions.Pull(data.fashions);
        }

        static T GiveBackIfModded<T>(T a)
        {
            return ModdedIDRegistry.IsModdedID(a) ? a : (T)(object) 0;
        }

        static T GiveNoneIfModded<T>(T a)
        {
            return ModdedIDRegistry.IsModdedID(a) ? (T) (object) 0 : a;
        }

        public override void Push(VanillaGadgetData data)
        {
            if (baitTypeId != Identifiable.Id.NONE) data.baitTypeId = baitTypeId;
            if (gordoTypeId != Identifiable.Id.NONE) data.gordoTypeId = gordoTypeId;
            if (data.drone != null) partialDrone.Push(data.drone);
            while (fashions.InternalList.Contains(Identifiable.Id.NONE)) fashions.InternalList.Remove(Identifiable.Id.NONE);
            fashions.Push(data.fashions);
        }

        public override void ReadData(BinaryReader reader)
        {
            baitTypeId = idSerializer.DeserializeGeneric(reader);
            gordoTypeId = idSerializer.DeserializeGeneric(reader);
            partialDrone.Read(reader);
            fashions.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            idSerializer.SerializeGeneric(writer,baitTypeId);
            idSerializer.SerializeGeneric(writer,gordoTypeId);
            partialDrone.Write(writer);
            fashions.Write(writer);
        }

        internal static void RegisterEnumFixer()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, PartialGadgetData v) =>
                {
                    v.baitTypeId = translator.TranslateEnum(mode, v.baitTypeId);
                    v.gordoTypeId = translator.TranslateEnum(mode, v.gordoTypeId);
                    translator.FixEnumValues(mode,v.partialDrone);
                    translator.FixEnumValues(mode,v.fashions);
                });
        }
    }
}
