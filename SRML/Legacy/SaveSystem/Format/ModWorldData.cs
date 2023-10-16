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
        public override int LatestVersion => 1;

        Dictionary<Identifiable.Id,float> econSaturations = new Dictionary<Identifiable.Id, float>();
        List<string> lastOfferRancherIds = new List<string>();
        List<string> pendingOfferRancherIds = new List<string>();
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
            if (Version == 0) return;
            BinaryUtils.ReadList(reader, lastOfferRancherIds,x=>x.ReadString());
            BinaryUtils.ReadList(reader, pendingOfferRancherIds, x => x.ReadString());
        }

        public override void WriteData(BinaryWriter writer)
        {
            BinaryUtils.WriteDictionary(writer, econSaturations, (x, y) => x.Write((int)y), (x, y) => x.Write(y));
            BinaryUtils.WriteList(writer, lastOfferRancherIds, (x, y) => x.Write(y));
            BinaryUtils.WriteList(writer, pendingOfferRancherIds, (x, y) => x.Write(y));
        }

        public void Pull(VanillaWorldData data,SRMod mod)
        {
            foreach(var v in data.econSaturations)
            {
                if (ModdedIDRegistry.ModForID(v.Key)==mod) econSaturations.Add(v.Key, v.Value);
            }
            pendingOfferRancherIds.AddRange(data.pendingOfferRancherIds.Where((x)=>ExchangeOfferRegistry.GetModForID(x)==mod));
            lastOfferRancherIds.AddRange(data.lastOfferRancherIds.Where((x) => ExchangeOfferRegistry.GetModForID(x) == mod));
        }

        public void Push(VanillaWorldData data)
        {
            foreach(var v in econSaturations)
            {
                if (ModdedIDRegistry.IsValidID(v.Key)) data.econSaturations[v.Key] = v.Value;
            }
            data.pendingOfferRancherIds.AddRange(pendingOfferRancherIds.Where(ExchangeOfferRegistry.IsCustom));
            data.lastOfferRancherIds.AddRange(lastOfferRancherIds.Where(ExchangeOfferRegistry.IsCustom));
        }

        public static HashSet<SRMod> FindAllModsWithData(VanillaWorldData data)
        {
            var set = new HashSet<SRMod>();

            foreach(var v in data.econSaturations)
            {
                set.Add(ModdedIDRegistry.ModForID(v.Key));
            }

            foreach (var v in data.lastOfferRancherIds.Select(ExchangeOfferRegistry.GetModForID)) set.Add(v);
            foreach (var v in data.pendingOfferRancherIds.Select(ExchangeOfferRegistry.GetModForID)) set.Add(v);
            set.Remove(null);
            return set;
        }
    }
}
