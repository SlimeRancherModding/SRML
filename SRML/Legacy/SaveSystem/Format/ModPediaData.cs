using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace SRML.SR.SaveSystem.Format
{
    internal class ModPediaData : VersionedSerializable
    {
        public List<string> unlockedIds = new List<string>();
        public List<string> completedTuts = new List<string>();
        public List<string> popupQueue = new List<string>();

        public override int LatestVersion => 0;

        public override void WriteData(BinaryWriter writer)
        {
            BinaryUtils.WriteList(writer,unlockedIds,(x,y)=>x.Write(y));
            BinaryUtils.WriteList(writer, completedTuts, (x, y) => x.Write(y));
            BinaryUtils.WriteList(writer, popupQueue, (x, y) => x.Write(y));
        }

        public override void ReadData(BinaryReader reader)
        {
            BinaryUtils.ReadList(reader,unlockedIds,(x)=>x.ReadString());
            BinaryUtils.ReadList(reader,completedTuts, (x) => x.ReadString());
            BinaryUtils.ReadList(reader, popupQueue, (x) => x.ReadString());
        }

        public void Pull(PediaDataBuffer data, SRMod ourMod)
        {
            unlockedIds.AddRange(data.unlockedIds.Where((x)=>ModdedIDRegistry.ModForID(x)==ourMod).Select((x)=>Enum.GetName(typeof(PediaDirector.Id),x)));
            completedTuts.AddRange(data.completedTuts.Where((x) => ModdedIDRegistry.ModForID(x) == ourMod).Select((x) => Enum.GetName(typeof(TutorialDirector.Id), x)));
            popupQueue.AddRange(data.popupQueue.Where((x) => ModdedIDRegistry.ModForID(x) == ourMod).Select((x) => Enum.GetName(typeof(TutorialDirector.Id), x)));
        }

        public void Push(PediaV03 data)
        {
            data.unlockedIds.AddRange(unlockedIds.Where((x) => Enum.IsDefined(typeof(PediaDirector.Id),x)&&ModdedIDRegistry.IsValidID(Enum.Parse(typeof(PediaDirector.Id),x))));
            data.completedTuts.AddRange(completedTuts.Where((x) => Enum.IsDefined(typeof(TutorialDirector.Id), x) && ModdedIDRegistry.IsValidID(Enum.Parse(typeof(TutorialDirector.Id), x))));
            data.popupQueue.AddRange(popupQueue.Where((x) => Enum.IsDefined(typeof(TutorialDirector.Id), x) && ModdedIDRegistry.IsValidID(Enum.Parse(typeof(TutorialDirector.Id), x))));
        }

        public static HashSet<SRMod> FindAllModsWithData(PediaDataBuffer data)
        {
            var mods = new HashSet<SRMod>();
            data.unlockedIds.ForEach((x)=>mods.Add(ModdedIDRegistry.ModForID(x)));
            data.completedTuts.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));
            data.popupQueue.ForEach((x) => mods.Add(ModdedIDRegistry.ModForID(x)));

            mods.Remove(null);
            return mods;
        }
    }

    internal class PediaDataBuffer
    {
        public List<PediaDirector.Id> unlockedIds;
        public List<TutorialDirector.Id> completedTuts;
        public List<TutorialDirector.Id> popupQueue;

        public PediaDataBuffer(PediaV03 pediaData)
        {
            unlockedIds = SavedGame.StringsToEnums<PediaDirector.Id>(pediaData.unlockedIds);
            completedTuts = SavedGame.StringsToEnums<TutorialDirector.Id>(pediaData.completedTuts);
            popupQueue = SavedGame.StringsToEnums<TutorialDirector.Id>(pediaData.popupQueue);
        }
    }
}
