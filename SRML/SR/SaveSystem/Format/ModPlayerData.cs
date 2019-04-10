using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;

namespace SRML.SR.SaveSystem.Format
{
    internal class ModPlayerData
    {
        public int version;

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
                });

        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(version);
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
        }

        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
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
        }

        

        public void Pull(PlayerV13 player,SRMod ourMod)
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

        }

        public void Push(PlayerV13 player)
        {
            player.upgrades.AddRange(upgrades);
            player.availUpgrades.AddRange(availUpgrades);
            AddRange(player.upgradeLocks,upgradeLocks);

            AddRange(player.progress,progress);
            AddRange(player.delayedProgress,delayedProgress);

            player.blueprints.AddRange(blueprints);
            player.availBlueprints.AddRange(availBlueprints);
            AddRange(player.blueprintLocks,blueprintLocks);

            AddRange(player.gadgets,gadgets);
            
            AddRange(player.craftMatCounts,craftMatCounts);
        }

        public static HashSet<SRMod> FindAllModsWithData(PlayerV13 player)
        {
            var mods = new HashSet<SRMod>();

            player.upgrades.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            player.availUpgrades.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            foreach (var x in player.upgradeLocks) mods.Add(ModdedIDRegistry.ModForID(x));

            foreach(var x in player.progress) mods.Add(ModdedIDRegistry.ModForID(x.Key));
            foreach (var x in player.delayedProgress) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            player.blueprints.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            player.availBlueprints.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            foreach (var x in player.blueprintLocks) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            foreach (var x in player.gadgets) mods.Add(ModdedIDRegistry.ModForID(x.Key));

            foreach (var x in player.craftMatCounts) mods.Add(ModdedIDRegistry.ModForID(x.Key));

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
