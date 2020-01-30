using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Macro
{
    public class PartialPlayerData : VersionedPartialData<PlayerV14>
    {
        public override int LatestVersion => 0;


        static PartialCollection<T> CreateDiscriminatedEnumList<T>() => new PartialCollection<T>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<T>());

        public PartialCollection<global::Gadget.Id> availBlueprints = CreateDiscriminatedEnumList<global::Gadget.Id>();
        public PartialCollection<PlayerState.Upgrade> availUpgrades = CreateDiscriminatedEnumList<PlayerState.Upgrade>();
        private static readonly SerializerPair<global::Gadget.Id> gadgetSerializer = SerializerPair.GetEnumSerializerPair<global::Gadget.Id>();
        public PartialDictionary<global::Gadget.Id, GadgetDirector.BlueprintLockData> blueprintLocks = new PartialDictionary<global::Gadget.Id,GadgetDirector.BlueprintLockData>(x=>ModdedIDRegistry.IsModdedID(x.Key), gadgetSerializer, new SerializerPair<GadgetDirector.BlueprintLockData>((x,y)=> { x.Write(y.timedLock);x.Write(y.lockedUntil); },(x)=>new GadgetDirector.BlueprintLockData(x.ReadBoolean(),x.ReadDouble())),checkValueValid:x=>ModdedIDRegistry.IsModdedID(x.Key));
        public PartialCollection<global::Gadget.Id> blueprints = CreateDiscriminatedEnumList<global::Gadget.Id>();
        public PartialDictionary<Identifiable.Id, int> craftMatCounts = new PartialDictionary<Identifiable.Id, int>(x => ModdedIDRegistry.IsModdedID(x.Key), SerializerPair.GetEnumSerializerPair<Identifiable.Id>(), SerializerPair.INT32, checkValueValid:x=>ModdedIDRegistry.IsModdedID(x.Key));
        public PartialDecorizerData decorizer = new PartialDecorizerData();
        public PartialDictionary<ProgressDirector.ProgressTrackerId, double> delayedProgress = new PartialDictionary<ProgressDirector.ProgressTrackerId, double>(x => ModdedIDRegistry.IsModdedID(x.Key), SerializerPair.GetEnumSerializerPair<ProgressDirector.ProgressTrackerId>(), SerializerPair.DOUBLE, checkValueValid:x => ModdedIDRegistry.IsModdedID(x.Key));
        public PartialDictionary<global::Gadget.Id, int> gadgets = new PartialDictionary<global::Gadget.Id, int>(x => ModdedIDRegistry.IsModdedID(x.Key), gadgetSerializer, SerializerPair.INT32, checkValueValid: x => ModdedIDRegistry.IsModdedID(x.Key));
        public PartialDictionary<ProgressDirector.ProgressType, int> progress = new PartialDictionary<ProgressDirector.ProgressType, int>(x => ModdedIDRegistry.IsModdedID(x.Key), SerializerPair.GetEnumSerializerPair<ProgressDirector.ProgressType>(), SerializerPair.INT32, checkValueValid: x => ModdedIDRegistry.IsModdedID(x.Key));
        public PartialCollection<MailV02> mail = new PartialCollection<MailV02>(x => SRMod.IsContextMod(MailRegistry.GetModForMail(x.messageKey)) && ModdedIDRegistry.IsModdedID(x.mailType), PartialDataUtils.CreateSerializerForPersistedDataSet<MailV02>(), x => SRMod.IsContextMod(MailRegistry.GetModForMail(x.messageKey)) && ModdedIDRegistry.IsModdedID(x.mailType));
        static readonly SerializerPair<RegionRegistry.RegionSetId> regionSetIdSerializer = SerializerPair.GetEnumSerializerPair<RegionRegistry.RegionSetId>();
        public RegionRegistry.RegionSetId regionSetId;
        public PartialCollection<ZoneDirector.Zone> unlockedZoneMaps = new PartialCollection<ZoneDirector.Zone>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<ZoneDirector.Zone>(), ModdedIDRegistry.IsModdedID);
        public PartialCollection<PlayerState.Upgrade> upgrades  = new PartialCollection<PlayerState.Upgrade>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<PlayerState.Upgrade>(), ModdedIDRegistry.IsModdedID);
        public PartialDictionary<PlayerState.Upgrade, PlayerState.UpgradeLockData> upgradeLocks = new PartialDictionary<PlayerState.Upgrade, PlayerState.UpgradeLockData>(x => ModdedIDRegistry.IsModdedID(x.Key), SerializerPair.GetEnumSerializerPair<PlayerState.Upgrade>(), new SerializerPair<PlayerState.UpgradeLockData>((x, y) => { x.Write(y.timedLock); x.Write(y.lockedUntil); }, (x) => new PlayerState.UpgradeLockData(x.ReadBoolean(), x.ReadDouble())), checkValueValid:x => ModdedIDRegistry.IsModdedID(x.Key));
        public override void Pull(PlayerV14 data)
        {
            regionSetId = ModdedIDRegistry.IsModdedID(data.regionSetId) ? data.regionSetId : RegionRegistry.RegionSetId.UNSET;
            decorizer.Pull(data.decorizer);
            upgrades.Pull(data.upgrades);
            upgradeLocks.Pull(data.upgradeLocks);
            craftMatCounts.Pull(data.craftMatCounts);
            delayedProgress.Pull(data.delayedProgress);
            mail.Pull(data.mail);
            availUpgrades.Pull(data.availUpgrades);
            availBlueprints.Pull(data.availBlueprints);
            gadgets.Pull(data.gadgets);
            unlockedZoneMaps.Pull(data.unlockedZoneMaps);
            blueprints.Pull(data.blueprints);
            blueprintLocks.Pull(data.blueprintLocks);
            progress.Pull(data.progress);
        }

        public override void Push(PlayerV14 data)
        {
            data.regionSetId = ModdedIDRegistry.IsModdedID(regionSetId) ? regionSetId : data.regionSetId;
            decorizer.Push(data.decorizer);
            upgrades.Push(data.upgrades);
            upgradeLocks.Push(data.upgradeLocks);
            craftMatCounts.Push(data.craftMatCounts);
            delayedProgress.Push(data.delayedProgress);
            mail.Push(data.mail);
            availUpgrades.Push(data.availUpgrades);
            availBlueprints.Push(data.availBlueprints);
            gadgets.Push(data.gadgets);
            unlockedZoneMaps.Push(data.unlockedZoneMaps);
            blueprints.Push(data.blueprints);
            blueprintLocks.Push(data.blueprintLocks);
            progress.Push(data.progress);
        }

        public override void ReadData(BinaryReader reader)
        {
            regionSetId = regionSetIdSerializer.DeserializeGeneric(reader);
            decorizer.Read(reader);
            upgrades.Read(reader);
            upgradeLocks.Read(reader);
            craftMatCounts.Read(reader);
            delayedProgress.Read(reader);
            mail.Read(reader);
            availUpgrades.Read(reader);
            availBlueprints.Read(reader);
            gadgets.Read(reader);
            unlockedZoneMaps.Read(reader);
            blueprints.Read(reader);
            blueprintLocks.Read(reader);
            progress.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            regionSetIdSerializer.Serialize(writer, regionSetId);
            decorizer.Write(writer);
            upgrades.Write(writer);
            upgradeLocks.Write(writer);
            craftMatCounts.Write(writer);
            delayedProgress.Write(writer);
            mail.Write(writer);
            availUpgrades.Write(writer);
            availBlueprints.Write(writer);
            gadgets.Write(writer);
            unlockedZoneMaps.Write(writer);
            blueprints.Write(writer);
            blueprintLocks.Write(writer);
            progress.Write(writer);
        }


        static bool GetIsCustom<K,V>(KeyValuePair<K,V> x)
        {
            return typeof(K).IsEnum ? ModdedIDRegistry.IsModdedID(x.Key) : (x.Key is string s  ? ModdedStringRegistry.IsValidModdedString(s) : false) ;
        }

        static bool GetIsCustom<T>(T obj)
        {

            if (typeof(T).IsEnum) return ModdedIDRegistry.IsModdedID(obj);
            switch (obj)
            {
                case MailV02 mail:
                    return SRMod.IsContextMod(MailRegistry.GetModForMail(mail.messageKey)) || ModdedIDRegistry.IsModdedID(mail.mailType);
                
            }
            //LogUtils.Log(obj);
            return false;
        }
        static PartialPlayerData()
        {
            PartialData.RegisterPartialData<PlayerV14,PartialPlayerData>();
            PartialData.RegisterPartialData<DecorizerV01, PartialDecorizerData>();
            EnumTranslator.RegisterEnumFixer<PlayerV14>((trans, mode, obj) =>
            {

                trans.FixEnumValues(mode, obj.decorizer);
                trans.FixEnumValues(mode, obj.upgrades);
                trans.FixEnumValues(mode, obj.upgradeLocks);
                trans.FixEnumValues(mode, obj.craftMatCounts);
                trans.FixEnumValues(mode, obj.delayedProgress);
                trans.FixEnumValues(mode, obj.mail);
                trans.FixEnumValues(mode, obj.availUpgrades);
                trans.FixEnumValues(mode, obj.availBlueprints);
                trans.FixEnumValues(mode, obj.gadgets);
                trans.FixEnumValues(mode, obj.unlockedZoneMaps);
                trans.FixEnumValues(mode, obj.blueprints);
                trans.FixEnumValues(mode, obj.blueprintLocks);
                trans.FixEnumValues(mode, obj.progress);
                obj.regionSetId = trans.TranslateEnum<RegionRegistry.RegionSetId>(mode,obj.regionSetId);
            });
            CustomChecker.RegisterCustomChecker<PlayerV14>(obj =>
            {
                try
                {
                    if (CustomChecker.GetCustomLevel(obj.decorizer) == CustomChecker.CustomLevel.PARTIAL ||
                    obj.upgrades.Any(x => GetIsCustom(x)) ||
                    obj.upgradeLocks.Any(x => GetIsCustom(x)) ||
                    obj.craftMatCounts.Any(x => GetIsCustom(x)) ||
                    obj.delayedProgress.Any(x => GetIsCustom(x)) ||
                    obj.mail.Any(x => GetIsCustom(x)) ||
                    obj.availUpgrades.Any(x => GetIsCustom(x)) ||
                    obj.availBlueprints.Any(x => GetIsCustom(x)) ||
                    obj.gadgets.Any(x => GetIsCustom(x)) ||
                    obj.unlockedZoneMaps.Any(x => GetIsCustom(x)) ||
                    obj.blueprints.Any(x => GetIsCustom(x)) ||
                    obj.blueprintLocks.Any(x => GetIsCustom(x)) ||
                    obj.progress.Any(x => GetIsCustom(x)) ||
                    ModdedIDRegistry.IsModdedID(obj.regionSetId)) return CustomChecker.CustomLevel.PARTIAL;
                    return CustomChecker.CustomLevel.VANILLA;
                }
                finally
                {
                }
            });
        }

        public class PartialDecorizerData : VersionedPartialData<DecorizerV01>
        {
            public override int LatestVersion => 0;
            public PartialDictionary<Identifiable.Id, int> contents = new PartialDictionary<Identifiable.Id, int>(x=>ModdedIDRegistry.IsModdedID(x.Key),SerializerPair.GetEnumSerializerPair<Identifiable.Id>(),SerializerPair.INT32,checkValueValid: x => ModdedIDRegistry.IsModdedID(x.Key));
            public PartialDictionary<string, DecorizerSettingsV01> settings = PartialDataUtils.CreatePartialDictionaryWithStringKey<DecorizerSettingsV01>();
            public override void Pull(DecorizerV01 data)
            {
                contents.Pull(data.contents);
                settings.Pull(data.settings);
            }

            public override void Push(DecorizerV01 data)
            {
                contents.Push(data.contents);
                settings.Push(data.settings);
            }

            public override void ReadData(BinaryReader reader)
            {
                contents.Read(reader);
                settings.Read(reader);
            }

            public override void WriteData(BinaryWriter writer)
            {
                contents.Write(writer);
                settings.Write(writer);
            }

            static PartialDecorizerData()
            {
                EnumTranslator.RegisterEnumFixer<PartialDecorizerData>((trans, mode, obj) =>
                {
                    trans.FixEnumValues(mode, obj.settings);
                    trans.FixEnumValues(mode, obj.contents);
                });

                CustomChecker.RegisterCustomChecker<DecorizerV01>(y =>
                {
                    if (
                    y.contents.Any(x => ModdedIDRegistry.IsModdedID(x.Key)) ||
                    y.settings.Any(x => ModdedStringRegistry.IsValidModdedString(x.Key))) return CustomChecker.CustomLevel.PARTIAL;
                    return CustomChecker.CustomLevel.VANILLA;
                });
            }
        }
    }
}
