using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Gadget
{
    internal class PartialGordoData : VersionedPartialData<GordoV01>
    {
        public PartialCollection<Identifiable.Id> fashions = new PartialCollection<Identifiable.Id>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<Identifiable.Id>());

        public override int LatestVersion => 0;

        public override void Pull(GordoV01 data)
        {
            fashions.Pull(data.fashions);
        }

        public override void Push(GordoV01 data)
        {
            fashions.Push(data.fashions);
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

        static PartialGordoData()
        {
            EnumTranslator.RegisterEnumFixer<PartialGordoData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.fashions);
                data.fashions.InternalList.Remove(Identifiable.Id.NONE);
            });

            CustomChecker.RegisterCustomChecker<GordoV01>((x) =>
            {
                if (x.fashions.Any(ModdedIDRegistry.IsModdedID)) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.NONE;
            });
            PartialData.RegisterPartialData(() => new PartialGordoData());
        }
    }
}
