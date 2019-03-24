using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
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
            foreach (var v in game.actors.Where((x) => SaveRegistry.IsCustom(x)))
            {
                var segment = data.GetSegmentForMod(SaveRegistry.ModForData(v));
                if (v is CustomActorData<ActorModel> model) segment.customActorData.Add(model);
                else segment.normalActorData.Add(v);
            }
        }

        public static void PushModdedData(GameV09 game)
        {
            foreach (var v in data.segments)
            {
                Debug.Log($"Splicing data from mod {v.modid}, it has {v.customActorData.Count} custom actors");
                game.actors.AddRange(v.customActorData.Select((x)=>(ActorDataV07)x));
                game.actors.AddRange(v.normalActorData.Select((x)=>(ActorDataV07)x));
            }
        }

        public static void LoadModdedSave(AutoSaveDirector director, string savename)
        {
            var storageprovider = director.storageProvider as FileStorageProvider;
            if (storageprovider == null) return;
            var modpath = Path.ChangeExtension(storageprovider.GetFullFilePath(savename), ".mod");
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
            var modpath = Path.ChangeExtension(storageprovider.GetFullFilePath(nextfilename), ".mod");
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
