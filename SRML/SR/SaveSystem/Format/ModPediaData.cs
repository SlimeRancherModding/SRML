using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using UnityEngine.Rendering;

namespace SRML.SR.SaveSystem.Format
{
    internal class ModPediaData
    {
        public int version;
        public List<string> unlockedIds = new List<string>();
        public List<string> completedTuts = new List<string>();
        public List<string> popupQueue = new List<string>();


        public void Write(BinaryWriter writer)
        {
            writer.Write(version);
            BinaryUtils.WriteList(writer,unlockedIds,(x,y)=>x.Write(y));
            BinaryUtils.WriteList(writer, completedTuts, (x, y) => x.Write(y));
            BinaryUtils.WriteList(writer, popupQueue, (x, y) => x.Write(y));
        }

        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
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
            data.unlockedIds.AddRange(unlockedIds);
            data.completedTuts.AddRange(completedTuts);
            data.popupQueue.AddRange(popupQueue);
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
