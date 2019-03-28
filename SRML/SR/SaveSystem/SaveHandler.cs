using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Format;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
namespace SRML.SR.SaveSystem
{
    internal static class SaveHandler
    {
        public static ModdedSaveData data = new ModdedSaveData();

        public static void PullModdedData(GameV09 game)
        {
            data.segments.Clear();
            data.ammoDataEntries.Clear();
            foreach (var actor in game.actors.Where((x) => SaveRegistry.IsCustom(x)))
            {
                var segment = data.GetSegmentForMod(SaveRegistry.ModForData(actor));
                if (actor is CustomActorData model) segment.customActorData.Add(model);
                else segment.normalActorData.Add(actor);
            }

            ExtendedData.Push(data);
            PersistentAmmoManager.Push(data);
        }

        public static void PushModdedData(GameV09 game)
        {
            foreach (var mod in data.segments)
            {
                Debug.Log($"Splicing data from mod {mod.modid}, it has {mod.customActorData.Count} custom actors and {mod.normalActorData.Count} normal actors with custom ID's");
                game.actors.AddRange(mod.customActorData.Select((x)=>(ActorDataV07)x));
                game.actors.AddRange(mod.normalActorData.Select((x)=>(ActorDataV07)x));
            }

            ExtendedData.Pull(data);
            PersistentAmmoManager.Pull(data);
        }

        public static string GetModdedPath(FileStorageProvider provider, string savename)
        {
            return Path.ChangeExtension(provider.GetFullFilePath(savename), ".mod");
        }

        public static void LoadModdedSave(AutoSaveDirector director, string savename)
        {
            var storageprovider = director.storageProvider as FileStorageProvider;
            if (storageprovider == null) return;
            var modpath = GetModdedPath(storageprovider, savename);
            Debug.Log(modpath+" is our modded path");
            if (!File.Exists(modpath)) return;
            using (var reader = new BinaryReader(new FileStream(modpath, FileMode.Open)))
            {
                data.Read(reader);
            }

            PushModdedData(director.savedGame.gameState);

        }

        public static void SaveModdedSave(AutoSaveDirector director, string nextfilename)
        {
            var storageprovider = director.storageProvider as FileStorageProvider;
            if (storageprovider == null) return;
            var modpath = GetModdedPath(storageprovider, nextfilename);
            Debug.Log(modpath + " is our modded path");
            PullModdedData(director.savedGame.gameState);
            if (File.Exists(modpath)) File.Delete(modpath);
            using (var writer = new BinaryWriter(new FileStream(modpath, FileMode.OpenOrCreate)))
            {
                data.Write(writer);
            }

            
        }
    }
}
