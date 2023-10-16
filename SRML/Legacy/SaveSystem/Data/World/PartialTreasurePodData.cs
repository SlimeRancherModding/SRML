using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.World
{
    internal class PartialTreasurePodData : PartialData<TreasurePodV01>
    {
        PartialCollection<Identifiable.Id> spawnQueue = new PartialCollection<Identifiable.Id>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<Identifiable.Id>());
        public override void Pull(TreasurePodV01 data)
        {
            spawnQueue.Pull(data.spawnQueue);
            
        }

        public override void Push(TreasurePodV01 data)
        {
            spawnQueue.Push(data.spawnQueue);
            data.spawnQueue.RemoveAll(x => x == Identifiable.Id.NONE);
        }

        public override void Read(BinaryReader reader)
        {
            spawnQueue.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            spawnQueue.Write(writer);
        }

        static PartialTreasurePodData()
        {
            CustomChecker.RegisterCustomChecker<TreasurePodV01>(x =>
            {
                if (x.spawnQueue.Any(ModdedIDRegistry.IsModdedID)) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.NONE;
            });

            EnumTranslator.RegisterEnumFixer<PartialTreasurePodData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.spawnQueue);
            });

            PartialData.RegisterPartialData(() => new PartialTreasurePodData());
        }
    }
}
