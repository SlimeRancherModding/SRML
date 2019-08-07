using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VanillaWorldData = MonomiPark.SlimeRancher.Persist.WorldV22;

namespace SRML.SR.SaveSystem.Format
{
    internal class ModWorldData : VersionedSerializable
    {
        public override int LatestVersion => 0;

        Dictionary<Identifiable.Id,float> econSaturations = new Dictionary<Identifiable.Id, float>();

        static ModWorldData()
        {
            EnumTranslator.RegisterEnumFixer<ModWorldData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.econSaturations);
            });
        }

        public override void ReadData(BinaryReader reader)
        {
            BinaryUtils.ReadDictionary(reader, econSaturations, (x) => (Identifiable.Id)x.ReadInt32(), (x) => x.ReadSingle());
        }

        public override void WriteData(BinaryWriter writer)
        {
            BinaryUtils.WriteDictionary(writer, econSaturations, (x, y) => x.Write((int)y), (x, y) => x.Write(y));
        }

        public void Pull(VanillaWorldData data,SRMod mod)
        {
            foreach(var v in data.econSaturations)
            {
                if (ModdedIDRegistry.ModForID(v.Key)==mod) econSaturations.Add(v.Key, v.Value);
            }
        }

        public void Push(VanillaWorldData data)
        {
            foreach(var v in econSaturations)
            {
                if (ModdedIDRegistry.IsValidID(v.Key)) data.econSaturations[v.Key] = v.Value;
            }
        }

        public static HashSet<SRMod> FindAllModsWithData(VanillaWorldData data)
        {
            var set = new HashSet<SRMod>();

            foreach(var v in data.econSaturations)
            {
                set.Add(ModdedIDRegistry.ModForID(v.Key));
            }

            set.Remove(null);
            return set;
        }
    }
}
