using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Pipeline;
using SRML.SR.SaveSystem.Registry;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace SRML.SR.SaveSystem.Data.Macro
{
    class PartialPediaData : VersionedPartialData<PediaV03>
    {
        public override int LatestVersion => 0;
        public PartialCollection<PediaDirector.Id> unlockedIds = new PartialCollection<PediaDirector.Id>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<PediaDirector.Id>(), ModdedIDRegistry.IsModdedID);
        private static readonly SerializerPair<TutorialDirector.Id> tutorialPair = SerializerPair.GetEnumSerializerPair<TutorialDirector.Id>();
        public PartialCollection<TutorialDirector.Id> completedTuts = new PartialCollection<TutorialDirector.Id>(ModdedIDRegistry.IsModdedID, tutorialPair, ModdedIDRegistry.IsModdedID);
        public PartialCollection<TutorialDirector.Id> popupQueue = new PartialCollection<TutorialDirector.Id>(ModdedIDRegistry.IsModdedID, tutorialPair,ModdedIDRegistry.IsModdedID);
        public override void Pull(PediaV03 data)
        {
            var buffer = new PediaDataBuffer(data);
            unlockedIds.Pull(buffer.unlockedIds); data.unlockedIds.RemoveAll(x => { foreach (var y in unlockedIds.InternalList) if (y.ToString() == x) return true; return false; });
            completedTuts.Pull(buffer.completedTuts); data.completedTuts.RemoveAll(x => { foreach (var y in completedTuts.InternalList) if (y.ToString() == x) return true; return false; });
            popupQueue.Pull(buffer.popupQueue); data.popupQueue.RemoveAll(x => { foreach (var y in popupQueue.InternalList) if (y.ToString() == x) return true; return false; });
        }

        void PushIntoList<T>(List<string> toPush, PartialCollection<T> col)
        {
            var list = new List<T>();
            col.Push(list);
            toPush.AddRange(list.Select(x => x.ToString()));
        }

        public override void Push(PediaV03 data)
        {
            PushIntoList(data.unlockedIds, unlockedIds);
            PushIntoList(data.completedTuts, completedTuts);
            PushIntoList(data.popupQueue, popupQueue);
        }

        public override void ReadData(BinaryReader reader)
        {
            unlockedIds.Read(reader);
            completedTuts.Read(reader);
            popupQueue.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            unlockedIds.Write(writer);
            completedTuts.Write(writer);
            popupQueue.Write(writer);
        }

        static PartialPediaData()
        {
            PartialData.RegisterPartialData<PediaV03, PartialPediaData>();
            EnumTranslator.RegisterEnumFixer<PartialPediaData>((trans, mode, obj) =>
            {
                trans.FixEnumValues(mode, obj.completedTuts);
                trans.FixEnumValues(mode, obj.popupQueue);
                trans.FixEnumValues(mode, obj.unlockedIds);
            });

            CustomChecker.RegisterCustomChecker<PediaV03>((y) =>
            {
                if (y.unlockedIds.Any(x => GetIsCustom<PediaDirector.Id>(x)) ||
                y.completedTuts.Any(x => GetIsCustom<TutorialDirector.Id>(x)) ||
                y.popupQueue.Any(x => GetIsCustom<TutorialDirector.Id>(x))) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
        }

        static bool GetIsCustom<T>(string t)
        {
            return Enum.IsDefined(typeof(T), t) && ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(T), t));
        }
    }
    public class PediaDataBuffer
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
