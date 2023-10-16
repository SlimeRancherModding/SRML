using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using UnityEngine;

namespace SRML.SR.SaveSystem.Format
{
    internal class ModPlayerData : VersionedSerializable
    {

        public List<PlayerState.Upgrade> upgrades = new List<PlayerState.Upgrade>();
        public List<PlayerState.Upgrade> availUpgrades = new List<PlayerState.Upgrade>();
        public Dictionary<PlayerState.Upgrade,PlayerState.UpgradeLockData> upgradeLocks=new Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLockData>();

        public Dictionary<ProgressDirector.ProgressType, int> progress =
            new Dictionary<ProgressDirector.ProgressType, int>();
        public Dictionary<ProgressDirector.ProgressTrackerId, double> delayedProgress = new Dictionary<ProgressDirector.ProgressTrackerId, double>(); 

        public List<Gadget.Id> blueprints = new List<Gadget.Id>();
        public List<Gadget.Id> availBlueprints = new List<Gadget.Id>();
        public Dictionary<Gadget.Id,GadgetDirector.BlueprintLockData> blueprintLocks = new Dictionary<Gadget.Id, GadgetDirector.BlueprintLockData>();
        
        public Dictionary<Gadget.Id,int> gadgets = new Dictionary<Gadget.Id, int>();

        public Dictionary<Identifiable.Id,int> craftMatCounts = new Dictionary<Identifiable.Id, int>();

        public List<MailV02> mail = new List<MailV02>();

        public List<ZoneDirector.Zone> unlockedZoneMaps = new List<ZoneDirector.Zone>();

        public override int LatestVersion => 1;

        static ModPlayerData()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, ModPlayerData data) =>
                {

                    EnumTranslator.FixEnumValues(translator,mode,data.upgrades);
                    EnumTranslator.FixEnumValues(translator, mode,data.availBlueprints);
                    EnumTranslator.FixEnumValues(translator, mode, data.blueprints);
                    EnumTranslator.FixEnumValues(translator, mode, data.blueprintLocks);
                    EnumTranslator.FixEnumValues(translator, mode, data.availUpgrades);
                    EnumTranslator.FixEnumValues(translator, mode, data.upgradeLocks);
                    EnumTranslator.FixEnumValues(translator, mode, data.progress);
                    EnumTranslator.FixEnumValues(translator, mode, data.craftMatCounts);
                    EnumTranslator.FixEnumValues(translator, mode,data.delayedProgress);
                    EnumTranslator.FixEnumValues(translator, mode, data.gadgets);
                    EnumTranslator.FixEnumValues(translator,mode,data.mail);
                    EnumTranslator.FixEnumValues(translator,mode,data.unlockedZoneMaps);
                });
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, MailV02 data) =>
                {
                    data.mailType=EnumTranslator.TranslateEnum(translator, mode, data.mailType);
                }
            );

        }

        public override void WriteData(BinaryWriter writer)
        {
            BinaryUtils.WriteList(writer,upgrades,(x,y)=>x.Write((int)y));
            BinaryUtils.WriteList(writer, availUpgrades, (x, y) => x.Write((int)y));
            BinaryUtils.WriteDictionary(writer, upgradeLocks, (x, y) => x.Write((int)y),(x,y)=> {x.Write(y.timedLock);x.Write(y.lockedUntil);});

            BinaryUtils.WriteDictionary(writer,progress,(x,y)=>x.Write((int)y),(x,y)=>x.Write(y));
            BinaryUtils.WriteDictionary(writer, delayedProgress, (x, y) => x.Write((int)y), (x, y) => x.Write(y));
                
            BinaryUtils.WriteList(writer, blueprints,(x,y)=>x.Write((int)y));
            BinaryUtils.WriteList(writer, availBlueprints, (x, y) => x.Write((int)y));
            BinaryUtils.WriteDictionary(writer, blueprintLocks, (x, y) => x.Write((int)y), (x, y) => { x.Write(y.timedLock); x.Write(y.lockedUntil); });

            BinaryUtils.WriteDictionary(writer, gadgets, (x, y) => x.Write((int)y), (x, y) => x.Write(y));

            BinaryUtils.WriteDictionary(writer, craftMatCounts, (x, y) => x.Write((int)y), (x, y) => x.Write(y));

            BinaryUtils.WriteList(writer,mail,(x,y)=>y.WriteData(x));

            var enumser = SerializerPair.GetEnumSerializerPair<ZoneDirector.Zone>();
            BinaryUtils.WriteList(writer,unlockedZoneMaps,(x,y)=>enumser.Serialize(x,y));
        }

        public override void ReadData(BinaryReader reader)
        {
            BinaryUtils.ReadList(reader,upgrades,(x)=>(PlayerState.Upgrade)x.ReadInt32());
            BinaryUtils.ReadList(reader, availUpgrades, (x) => (PlayerState.Upgrade)x.ReadInt32());
            BinaryUtils.ReadDictionary(reader,upgradeLocks,(x)=> (PlayerState.Upgrade)x.ReadInt32(),(x)=>new PlayerState.UpgradeLockData(x.ReadBoolean(),x.ReadDouble()));

            BinaryUtils.ReadDictionary(reader,progress,(x)=>(ProgressDirector.ProgressType)x.ReadInt32(),(x)=>x.ReadInt32());
            BinaryUtils.ReadDictionary(reader, delayedProgress, (x) => (ProgressDirector.ProgressTrackerId)x.ReadDouble(), (x) => x.ReadDouble());

            BinaryUtils.ReadList(reader,blueprints,(x)=>(Gadget.Id)x.ReadInt32());
            BinaryUtils.ReadList(reader, availBlueprints, (x) => (Gadget.Id) x.ReadInt32());
            BinaryUtils.ReadDictionary(reader,blueprintLocks,(x)=>(Gadget.Id)x.ReadInt32(),(x)=>new GadgetDirector.BlueprintLockData(x.ReadBoolean(),x.ReadDouble()));

            BinaryUtils.ReadDictionary(reader,gadgets, (x) => (Gadget.Id)x.ReadInt32(), (x) => x.ReadInt32());

            BinaryUtils.ReadDictionary(reader, craftMatCounts, (x) => (Identifiable.Id)x.ReadInt32(), (x) => x.ReadInt32());

            if (Version == 0) return;

            BinaryUtils.ReadList(reader,mail, (x) =>
            {
                var v = new MailV02();
                v.LoadData(reader);
                return v;
            });
            var enumser = SerializerPair.GetEnumSerializerPair<ZoneDirector.Zone>();
            BinaryUtils.ReadList(reader,unlockedZoneMaps,(x)=>(ZoneDirector.Zone)enumser.Deserialize(x));
        }

        

        public void Pull(PlayerV14 player,SRMod ourMod)
        {
            upgrades.AddRange(player.upgrades.Where((x)=>ModdedIDRegistry.ModForID(x)==ourMod));
            availUpgrades.AddRange(player.availUpgrades.Where((x) => ModdedIDRegistry.ModForID(x) == ourMod));
            AddRange(upgradeLocks,player.upgradeLocks.Where((x)=> ModdedIDRegistry.ModForID(x.Key) == ourMod));

            AddRange(progress,player.progress.Where((x)=> ModdedIDRegistry.ModForID(x.Key) == ourMod));
            AddRange(delayedProgress, player.delayedProgress.Where((x) => ModdedIDRegistry.ModForID(x.Key) == ourMod));

            blueprints.AddRange(player.blueprints.Where((x)=> ModdedIDRegistry.ModForID(x) == ourMod));
            availBlueprints.AddRange(player.availBlueprints.Where((x) => ModdedIDRegistry.ModForID(x) == ourMod));
            AddRange(blueprintLocks,player.blueprintLocks.Where((x)=> ModdedIDRegistry.ModForID(x.Key) == ourMod));

            AddRange(gadgets,player.gadgets.Where((x)=> ModdedIDRegistry.ModForID(x.Key) == ourMod));

            AddRange(craftMatCounts, player.craftMatCounts.Where((x) => ModdedIDRegistry.ModForID(x.Key) == ourMod));

            unlockedZoneMaps.AddRange(player.unlockedZoneMaps.Where((x)=>ModdedIDRegistry.ModForID(x)==ourMod));

            mail.AddRange(player.mail.Where((x)=>MailRegistry.GetModForMail(x.messageKey)==ourMod));
        }

        public void Push(PlayerV14 player)
        {
            player.upgrades.AddRange(upgrades.Where((x) => ModdedIDRegistry.IsValidID(x)));
            player.availUpgrades.AddRange(availUpgrades.Where((x)=>ModdedIDRegistry.IsValidID(x)));
            AddRange(player.upgradeLocks,upgradeLocks.Where((x) => ModdedIDRegistry.IsValidID(x.Key)));

            AddRange(player.progress,progress.Where((x) => ModdedIDRegistry.IsValidID(x.Key)));
            AddRange(player.delayedProgress,delayedProgress.Where((x) => ModdedIDRegistry.IsValidID(x.Key)));

            player.blueprints.AddRange(blueprints.Where((x) => ModdedIDRegistry.IsValidID(x)));
            player.availBlueprints.AddRange(availBlueprints.Where((x) => ModdedIDRegistry.IsValidID(x)));
            AddRange(player.blueprintLocks,blueprintLocks.Where((x) => ModdedIDRegistry.IsValidID(x.Key)));

            AddRange(player.gadgets,gadgets.Where((x) => ModdedIDRegistry.IsValidID(x.Key)));
            
            AddRange(player.craftMatCounts,craftMatCounts.Where((x) => ModdedIDRegistry.IsValidID(x.Key)));
            player.unlockedZoneMaps.AddRange(unlockedZoneMaps.Where((x)=>ModdedIDRegistry.IsValidID(unlockedZoneMaps)));
            player.mail.AddRange(mail.Where((x)=>MailRegistry.GetModForMail(x.messageKey)!=null));
        }

        public static HashSet<SRMod> FindAllModsWithData(PlayerV14 player)
        {
            var mods = new HashSet<SRMod>();

            player.upgrades.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            player.availUpgrades.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            foreach (var x in player.upgradeLocks) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            foreach(var x in player.progress) mods.Add(ModdedIDRegistry.ModForID(x.Key));
            foreach (var x in player.delayedProgress) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            player.blueprints.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            player.availBlueprints.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            foreach (var x in player.blueprintLocks) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            foreach (var x in player.gadgets) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            foreach (var x in player.craftMatCounts) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            player.mail.ForEach((x)=>mods.Add(MailRegistry.GetModForMail(x.messageKey)));

            player.unlockedZoneMaps.ForEach((x)=>mods.Add(ModdedIDRegistry.ModForID(x)));

            mods.Remove(null);
            return mods;
        }

        static void AddRange<K, V>(Dictionary<K, V> originalDict, IEnumerable<KeyValuePair<K, V>> enumerable)
        {
            foreach (var pair in enumerable)
            {
                originalDict[pair.Key] = pair.Value;
            }
        }
    }
}
