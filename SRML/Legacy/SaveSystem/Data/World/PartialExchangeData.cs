using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.World
{
    internal class PartialExchangeData : VersionedPartialData<ExchangeOfferV04>
    {
        public PartialCollection<RequestedItemEntryV03> requests = new PartialCollection<RequestedItemEntryV03>((x) => ModdedIDRegistry.IsModdedID(x.id) || ModdedIDRegistry.IsModdedID(x.nonIdentReward), new SRML.Utils.SerializerPair<RequestedItemEntryV03>((x, y) => y.Write(x.BaseStream), (x) => { var v = new RequestedItemEntryV03(); v.Load(x.BaseStream); return v; }));
        public PartialCollection<ItemEntryV03> rewards = new PartialCollection<ItemEntryV03>((x) => ModdedIDRegistry.IsModdedID(x.id) || ModdedIDRegistry.IsModdedID(x.nonIdentReward), new SRML.Utils.SerializerPair<ItemEntryV03>((x, y) => y.Write(x.BaseStream), (x) => { var v = new ItemEntryV03(); v.Load(x.BaseStream); return v; }));
        public override int LatestVersion => 0;
        public override void Pull(ExchangeOfferV04 data)
        {
            requests.Pull(data.requests);
            rewards.Pull(data.rewards);
        }

        public override void Push(ExchangeOfferV04 data)
        {
            requests.Push(data.requests);
            rewards.Push(data.rewards);
            data.requests.RemoveAll(x => x.id == Identifiable.Id.NONE);
            data.rewards.RemoveAll(x => x.id == Identifiable.Id.NONE);
        }

        public override void ReadData(BinaryReader reader)
        {
            requests.Read(reader);
            rewards.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            requests.Write(writer);
            rewards.Write(writer);
        }


        static PartialExchangeData()
        {
            RegisterPartialData(() => new PartialExchangeData());
            EnumTranslator.RegisterEnumFixer<PartialExchangeData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.requests);
                translator.FixEnumValues(mode, data.rewards);
            });
            EnumTranslator.RegisterEnumFixer<ItemEntryV03>((translator, mode, data) =>
            {
                data.id = translator.TranslateEnum(mode, data.id);
                data.nonIdentReward = translator.TranslateEnum(mode, data.nonIdentReward);
            });

            EnumTranslator.RegisterEnumFixer<RequestedItemEntryV03>((translator, mode, data) =>
            {
                data.id = translator.TranslateEnum(mode, data.id);
                data.nonIdentReward = translator.TranslateEnum(mode, data.nonIdentReward);
            });

            EnumTranslator.RegisterEnumFixer<ExchangeOfferV04>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.requests);
                translator.FixEnumValues(mode, data.rewards);
            });


            CustomChecker.RegisterCustomChecker<ExchangeOfferV04>((data) =>
            {
                if (ExchangeOfferRegistry.IsCustom(data)) return CustomChecker.CustomLevel.FULL;
                if (data.requests.Any(x => ModdedIDRegistry.IsModdedID(x.id) || ModdedIDRegistry.IsModdedID(x.nonIdentReward))) return CustomChecker.CustomLevel.PARTIAL;
                if (data.rewards.Any(x => ModdedIDRegistry.IsModdedID(x.id) || ModdedIDRegistry.IsModdedID(x.nonIdentReward))) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
        }
    }
}
