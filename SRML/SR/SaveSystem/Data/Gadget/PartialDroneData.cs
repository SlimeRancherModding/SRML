using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using VanillaDroneData = MonomiPark.SlimeRancher.Persist.DroneGadgetV01;
namespace SRML.SR.SaveSystem.Data.Gadget
{
    internal class PartialDroneData : VersionedPartialData<VanillaDroneData>
    {
        private PartialCollection<Identifiable.Id> fashions = new PartialCollection<Identifiable.Id>(ModdedIDRegistry.IsModdedID,
            SerializerPair.GetEnumSerializerPair<Identifiable.Id>());

        public override int LatestVersion => 0;

        public override void Pull(VanillaDroneData data)
        {
            fashions.Pull(data.drone.fashions);
        }

        public override void Push(VanillaDroneData data)
        {
            fashions.Push(data.drone.fashions);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            fashions.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            fashions.Write(writer);
        }

        internal static void RegisterEnumFixer()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, PartialDroneData v) =>
                {
                    translator.FixEnumValues(mode,v.fashions);
                });
        }
    }
}
