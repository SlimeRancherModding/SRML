using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Format;
using SRML.SR.SaveSystem.Utils;
using SRML.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV09;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV08;
using VanillaPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
using Game = MonomiPark.SlimeRancher.Persist.GameV12;
namespace SRML.SR.SaveSystem
{
    internal static class SaveHandler
    {
        public static ModdedSaveData data = new ModdedSaveData();
        #region pulling data
        public static void PullModdedData(ModdedSaveData data, Game game)
        {
            data.Clear();

            PullFullData(data, game);
            PullTertiaryData(data, game);
            PullAmmoData(data, game);
            PullPartialData(data, game);

            ExtendedData.Push(data);
            PersistentAmmoManager.SyncAll();
            PersistentAmmoManager.Push(data);
        }

        private static void PullPartialData(ModdedSaveData data, Game game)
        {
            void Check<T>(T v, Action
                            <T, PartialData> onSuccess)
            {
                var level = CustomChecker.GetCustomLevel(v);
                if (level == CustomChecker.CustomLevel.PARTIAL)
                {
                    var partialdata = PartialData.GetPartialData(v.GetType(), true);
                    partialdata.Pull(v);

                    onSuccess(v, partialdata);
                }
            }

            foreach (var g in game.actors)
            {
                Check(g, (v, partialdata) =>
                    data.partialData.Add(new DataIdentifier() { longID = v.actorId, Type = IdentifierType.ACTOR }, partialdata));

            }

            foreach (var g in game.world.placedGadgets)
            {
                var currentString = g.Key;
                Check(g.Value, (v, partialdata) =>
                {
                    data.partialData.Add(new DataIdentifier() { stringID = currentString, Type = IdentifierType.GADGET },
                        partialdata);
                });
            }

            foreach (var g in game.ranch.plots)
            {
                var currentString = g.id;
                Check(g, (v, partialdata) =>
                    data.partialData.Add(new DataIdentifier() { stringID = currentString, Type = IdentifierType.LANDPLOT }, partialdata));
            }

            foreach(var g in game.world.gordos)
            {
                Check(g.Value, (v, partialData) => data.partialData.Add(new DataIdentifier() { Type = IdentifierType.GORDO, stringID = g.Key }, partialData));
            }

            foreach(var t in game.world.treasurePods)
            {
                Check(t.Value, (v, partialData) => data.partialData.Add(new DataIdentifier() { Type = IdentifierType.TREASUREPOD, stringID = t.Key }, partialData));
            }

            foreach(var t in game.world.offers)
            {
                var cur = t.Key;
                Check(t.Value, (v, partialData) => data.partialData.Add(new DataIdentifier() { Type = IdentifierType.EXCHANGEOFFER, longID = (int)cur },partialData));
            }

            data.appearancesData.Pull(game.appearances);
        }

        private static void PullAmmoData(ModdedSaveData data, Game game)
        {
            foreach (var ammo in AmmoDataUtils.GetAllAmmoData(game))
            {
                var modsInThis = new HashSet<SRMod>(ammo.Select((x) => ModdedIDRegistry.IsModdedID(x.id) ? ModdedIDRegistry.ModForID(x.id) : null));
                var belongingMod = AmmoIdentifier.TryGetIdentifier(ammo, game, out var id) ? AmmoIdentifier.GetModForIdentifier(id) : null;
                modsInThis.Add(belongingMod);
                modsInThis.Remove(null);
                foreach (var mod in modsInThis)
                {
                    if (mod == null) continue;
                    if (AmmoIdentifier.TryGetIdentifier(ammo, game, out var identifier))
                    {
                        var segment = data.GetSegmentForMod(mod);
                        segment.customAmmo[identifier] =
                            AmmoDataUtils.RipOutWhere(ammo, (x) => mod==belongingMod?ModdedIDRegistry.ModForID(x.id)==null:ModdedIDRegistry.ModForID(x.id) == mod, false);
                    }
                    else
                    {
                        Debug.LogError("Unknown ammo identifier, skipping...");
                    }
                }
            }
        }

        private static void PullTertiaryData(ModdedSaveData data, Game game)
        {
            foreach (var mod in ModPlayerData.FindAllModsWithData(game.player))
            {

                var segment = data.GetSegmentForMod(mod);

                segment.playerData.Pull(game.player, mod);
            }

            PediaDataBuffer buf = new PediaDataBuffer(game.pedia);
            foreach (var mod in ModPediaData.FindAllModsWithData(buf))
            {
                var segment = data.GetSegmentForMod(mod);
                segment.pediaData.Pull(buf, mod);
            }

            foreach(var mod in ModWorldData.FindAllModsWithData(game.world))
            {
                var segment = data.GetSegmentForMod(mod);
                segment.worldData.Pull(game.world,mod);
            }
        }

        private static void PullFullData(ModdedSaveData data, Game game)
        {
            foreach (var actor in game.actors.Where((x) => SaveRegistry.IsCustom(x)))
            {
                var segment = data.GetSegmentForMod(SaveRegistry.ModForData(actor));
                segment.identifiableData.Add(new IdentifiedData()
                {
                    data = actor,
                    dataID = new DataIdentifier() { longID = actor.actorId, stringID = "", Type = IdentifierType.ACTOR }
                });
            }


            foreach(var plot in game.ranch.plots.Where(x => ModdedStringRegistry.IsValidString(x.id) && (SaveRegistry.IsCustom(x) || ModdedStringRegistry.IsModdedString(x.id))))
            {
                var segment = data.GetSegmentForMod(SaveRegistry.ModForData(plot) is SRMod mod ? mod : ModdedStringRegistry.GetModForModdedString(plot.id));
                segment.identifiableData.Add(new IdentifiedData()
                {
                    data = plot,
                    dataID = new DataIdentifier() { longID = 0, stringID = plot.id, Type = IdentifierType.LANDPLOT }
                });
            }

            void GetStringIndexedModdedData<T>(Dictionary<string, T> source, Func<KeyValuePair<string, T>, DataIdentifier> dataIdentifier) where T : PersistedDataSet
            {
                foreach (var pair in source.Where(x =>ModdedStringRegistry.IsValidString(x.Key) && (SaveRegistry.IsCustom(x.Value) || ModdedStringRegistry.IsModdedString(x.Key))))
                {
                    var segment = data.GetSegmentForMod(SaveRegistry.ModForData(pair.Value) ?? ModdedStringRegistry.GetModForModdedString(pair.Key));
                    segment.identifiableData.Add(new IdentifiedData()
                    {
                        data = pair.Value,
                        dataID = dataIdentifier(pair)
                    });
                }
            }


            GetStringIndexedModdedData(game.world.placedGadgets, (gadget) => new DataIdentifier() { longID = 0, stringID = gadget.Key, Type = IdentifierType.GADGET }); 
            GetStringIndexedModdedData(game.world.gordos, (gordo) => new DataIdentifier() { longID = 0, stringID = gordo.Key, Type = IdentifierType.GORDO });
            GetStringIndexedModdedData(game.world.treasurePods, (pod) => new DataIdentifier() { longID = 0, stringID = pod.Key, Type = IdentifierType.TREASUREPOD });

            foreach (var v in game.world.offers.Where(x=>ModdedIDRegistry.IsModdedID(x.Key)||ExchangeOfferRegistry.IsCustom(x.Value)))
            {
                var segment = data.GetSegmentForMod(SaveRegistry.ModForData(v.Value) ?? ExchangeOfferRegistry.GetModForData(v.Value));
                segment.identifiableData.Add(new IdentifiedData()
                {
                    data = v.Value,
                    dataID = new DataIdentifier() { Type = IdentifierType.EXCHANGEOFFER, longID = (int)v.Key }
                });
            }
        }
        #endregion

        #region pushing data
        public static void PushAllModdedData(ModdedSaveData data, Game game)
        {


            
            ExtendedData.Pull(data);
            PushAllSegmentData(data, game);


            PersistentAmmoManager.Pull(data);
            PushAllPartialData(data, game);
        }

        private static void PushAllSegmentData(ModdedSaveData data, Game game)
        {
            foreach (var mod in data.segments)
            {
                PushSegmentFullData(game, mod);

                PushSegmentTertiaryData(game, mod);

                PushSegmentAmmoData(game, mod);
            }
        }

        private static void PushSegmentAmmoData(Game game, ModDataSegment mod)
        {
            foreach (var ammo in mod.customAmmo)
            {
                if (!ammo.Key.IsValid()) continue;
                AmmoDataUtils.SpliceAmmoData(AmmoIdentifier.ResolveToData(ammo.Key, game), ammo.Value);
            }
        }

        private static void PushSegmentTertiaryData(Game game, ModDataSegment mod)
        {
            mod.playerData.Push(game.player);
            mod.pediaData.Push(game.pedia);
            mod.worldData.Push(game.world);
        }

        private static void PushSegmentFullData(Game game, ModDataSegment mod)
        {
            Debug.Log($"Splicing data from mod {mod.modid} which has {mod.identifiableData.Count} pieces of identifiable data");
            foreach (var customData in mod.identifiableData)
            {
                switch (customData.dataID.Type)
                {
                    case IdentifierType.ACTOR:
                        game.actors.Add((VanillaActorData)customData.data);
                        break;
                    case IdentifierType.GADGET:
                        game.world.placedGadgets[customData.dataID.stringID] = (VanillaGadgetData)customData.data;
                        break;
                    case IdentifierType.LANDPLOT:
                        game.ranch.plots.Add((VanillaPlotData)customData.data);
                        break;
                    case IdentifierType.GORDO:
                        game.world.gordos[customData.dataID.stringID] = (GordoV01)customData.data;
                        break;
                    case IdentifierType.TREASUREPOD:
                        game.world.treasurePods[customData.dataID.stringID] = (TreasurePodV01)customData.data;
                        break;
                    case IdentifierType.EXCHANGEOFFER:
                        var offertype = (ExchangeDirector.OfferType)(int)customData.dataID.longID;
                        if (Enum.IsDefined(typeof(ExchangeDirector.OfferType), offertype)) game.world.offers[offertype] = (ExchangeOfferV04)customData.data;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static void PushAllPartialData(ModdedSaveData data, Game game)
        {
            foreach (var partial in data.partialData)
            {
                switch (partial.Key.Type)
                {
                    case IdentifierType.ACTOR:
                        if(game.actors.FirstOrDefault((x)=>x.actorId==partial.Key.longID) is VanillaActorData dat) partial.Value.Push(dat);
                        break;
                    case IdentifierType.GADGET:
                        if (game.world.placedGadgets.ContainsKey(partial.Key.stringID)) partial.Value.Push(game.world.placedGadgets[partial.Key.stringID]);
                        break;
                    case IdentifierType.LANDPLOT:       
                        if(game.ranch.plots.FirstOrDefault((x)=>x.id==partial.Key.stringID) is VanillaPlotData plot) partial.Value.Push(plot);
                        break;
                    case IdentifierType.GORDO:
                        if (game.world.gordos.TryGetValue(partial.Key.stringID,out var gordo)) partial.Value.Push(gordo);
                        break;
                    case IdentifierType.TREASUREPOD:
                        if (game.world.treasurePods.TryGetValue(partial.Key.stringID, out var treasurepod)) partial.Value.Push(treasurepod);
                        break;
                    case IdentifierType.EXCHANGEOFFER:
                        if (game.world.offers.TryGetValue((ExchangeDirector.OfferType)partial.Key.longID, out var offer)) partial.Value.Push(offer);
                        break;
                    default:
                        throw new NotImplementedException();

                }
            }
            data.appearancesData.Push(game.appearances);
        }
        #endregion

        public static string GetModdedPath(FileStorageProvider provider, string savename)
        {
            return Path.ChangeExtension(provider.GetFullFilePath(savename), ".mod");
        }

        static void ClearAllNonData()
        {
            ExtendedData.Clear();
            PersistentAmmoManager.Clear();
        }

        public static void LoadModdedSave(AutoSaveDirector director, string savename)
        {   
            var storageprovider = director.StorageProvider as FileStorageProvider;
            if (storageprovider == null) return;
            var modpath = GetModdedPath(storageprovider, savename);
            Debug.Log(modpath+" is our modded path");
            ClearAllNonData();
            if (!File.Exists(modpath)) return;

            using (var reader = new BinaryReader(new FileStream(modpath, FileMode.Open)))
            {
                data.Read(reader);
            }

            data.enumTranslator?.FixMissingEnumValues();
            data.FixAllEnumValues(EnumTranslator.TranslationMode.FROMTRANSLATED);
            PushAllModdedData(data,director.SavedGame.gameState);
        }

        public static void SaveModdedSave(AutoSaveDirector director, string nextfilename)
        {
            var storageprovider = director.StorageProvider as FileStorageProvider;
            if (storageprovider == null) return;
            var modpath = GetModdedPath(storageprovider, nextfilename);
            Debug.Log(modpath + " is our modded path");
            PullModdedData(data,director.SavedGame.gameState);
            data.InitializeEnumTranslator();
            data.FixAllEnumValues(EnumTranslator.TranslationMode.TOTRANSLATED);
            if (File.Exists(modpath)) File.Delete(modpath);
            using (var writer = new BinaryWriter(new FileStream(modpath, FileMode.OpenOrCreate)))
            {
                data.Write(writer);
            }

            data.FixAllEnumValues(EnumTranslator.TranslationMode.FROMTRANSLATED);
            PushAllPartialData(data, director.SavedGame.gameState); // re-apply the data we took out so we leave the game state relatively untouched
        }

        static SaveHandler()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, AmmoDataV02 data) =>
                {
                    data.id = translator.TranslateEnum(mode, data.id);
                    translator.FixEnumValues(mode,data.emotionData.emotionData);
                });
            CustomGadgetData.RegisterGadgetThings();
        }
    }
}
