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
    internal class PartialGordoData : PartialData<GordoV01>
    {
        public PartialCollection<Identifiable.Id> fashions = new PartialCollection<Identifiable.Id>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<Identifiable.Id>());
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
            fashions.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            fashions.Write(writer);
        }
    }
}
