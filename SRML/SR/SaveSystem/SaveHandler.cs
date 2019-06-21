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
namespace SRML.SR.SaveSystem
{
    internal static class SaveHandler
    {
        public static ModdedSaveData data = new ModdedSaveData();
        #region pulling data
        public static void PullModdedData(ModdedSaveData data, GameV11 game)
        {
            data.Clear();

            PullFullData(data,game);
            PullTertiaryData(data,game);
            PullAmmoData(data,game);
            PullPartialData(data,game);

            ExtendedData.Push(data);
            PersistentAmmoManager.SyncAll();
            PersistentAmmoManager.Push(data);
        }

        private static void PullPartialData(ModdedSaveData data, GameV11 game)
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
                Check(g, (v, partialdata) =>
                    data.partialData.Add(new DataIdentifier() { stringID = g.id, Type = IdentifierType.LANDPLOT }, partialdata));
            }
        }

        private static void PullAmmoData(ModdedSaveData data, GameV11 game)
        {
            foreach (var ammo in AmmoDataUtils.GetAllAmmoData(game).Where((x) => AmmoDataUtils.HasCustomData(x)))
            {
                var modsInThis = new HashSet<SRMod>(ammo.Select((x) => ModdedIDRegistry.IsModdedID(x.id) ? ModdedIDRegistry.ModForID(x.id) : null));
                modsInThis.Remove(null);
                foreach (var mod in modsInThis)
                {
                    if (mod == null) continue;
                    if (AmmoIdentifier.TryGetIdentifier(ammo, game, out var identifier))
                    {
                        var segment = data.GetSegmentForMod(mod);
                        segment.customAmmo[identifier] =
                            AmmoDataUtils.RipOutWhere(ammo, (x) => ModdedIDRegistry.ModForID(x.id) == mod, false);
                    }
                    else
                    {
                        Debug.LogError("Unknown ammo identifier, skipping...");
                    }
                }
            }
        }

        private static void PullTertiaryData(ModdedSaveData data, GameV11 game)
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
        }

        private static void PullFullData(ModdedSaveData data, GameV11 game)
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

            foreach (var gadget in game.world.placedGadgets.Where((x) => SaveRegistry.IsCustom(x.Value)))
            {
                var segment = data.GetSegmentForMod(SaveRegistry.ModForData(gadget.Value));
                segment.identifiableData.Add(new IdentifiedData()
                {
                    data = gadget.Value,
                    dataID = new DataIdentifier() { longID = 0, stringID = gadget.Key, Type = IdentifierType.GADGET }
                });

            }
        }
        #endregion

        #region pushing data
        public static void PushAllModdedData(ModdedSaveData data, GameV11 game)
        {
            PushAllSegmentData(data, game);

            ExtendedData.Pull(data);
            PersistentAmmoManager.Pull(data);
            PushAllPartialData(data, game);
        }

        private static void PushAllSegmentData(ModdedSaveData data, GameV11 game)
        {
            foreach (var mod in data.segments)
            {
                PushSegmentFullData(game, mod);

                PushSegmentTertiaryData(game, mod);

                PushSegmentAmmoData(game, mod);
            }
        }

        private static void PushSegmentAmmoData(GameV11 game, ModDataSegment mod)
        {
            foreach (var ammo in mod.customAmmo)
            {
                AmmoDataUtils.SpliceAmmoData(AmmoIdentifier.ResolveToData(ammo.Key, game), ammo.Value);
            }
        }

        private static void PushSegmentTertiaryData(GameV11 game, ModDataSegment mod)
        {
            mod.playerData.Push(game.player);
            mod.pediaData.Push(game.pedia);
        }

        private static void PushSegmentFullData(GameV11 game, ModDataSegment mod)
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
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static void PushAllPartialData(ModdedSaveData data,GameV11 game)
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
                    default:
                        throw new NotImplementedException();

                }
            }
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
            var storageprovider = director.storageProvider as FileStorageProvider;
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
            PushAllModdedData(data,director.savedGame.gameState);

        }

        public static void SaveModdedSave(AutoSaveDirector director, string nextfilename)
        {
            var storageprovider = director.storageProvider as FileStorageProvider;
            if (storageprovider == null) return;
            var modpath = GetModdedPath(storageprovider, nextfilename);
            Debug.Log(modpath + " is our modded path");
            PullModdedData(data,director.savedGame.gameState);
            data.InitializeEnumTranslator();
            data.FixAllEnumValues(EnumTranslator.TranslationMode.TOTRANSLATED);
            if (File.Exists(modpath)) File.Delete(modpath);
            using (var writer = new BinaryWriter(new FileStream(modpath, FileMode.OpenOrCreate)))
            {
                data.Write(writer);
            }

            data.FixAllEnumValues(EnumTranslator.TranslationMode.FROMTRANSLATED);
            PushAllPartialData(data, director.savedGame.gameState); // re-apply the data we took out so we leave the game state relatively untouched
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
