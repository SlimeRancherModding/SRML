using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Format;
using SRML.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV09;
namespace SRML.SR.SaveSystem.Data.Actor
{
    internal class PartialActorData : VersionedPartialData<VanillaActorData>
    {
        public PartialCollection<Identifiable.Id> partialFashions = new PartialCollection<Identifiable.Id>(ModdedIDRegistry.IsModdedID, SerializerPair.GetEnumSerializerPair<Identifiable.Id>());
        public PartialDictionary<SlimeEmotions.Emotion,float> partialEmotions = new PartialDictionary<SlimeEmotions.Emotion, float>((x)=>ModdedIDRegistry.IsModdedID(x.Key),SerializerPair.GetEnumSerializerPair<SlimeEmotions.Emotion>(),new SerializerPair<float>((x,y)=>x.Write(y),(x)=>x.ReadSingle()));

        public override int LatestVersion => 0;

        public override void Pull(VanillaActorData data)
        {
            partialFashions.Pull(data.fashions);
            partialEmotions.Pull(data.emotions.emotionData);
        }

        public override void Push(VanillaActorData data)
        {
            while (partialFashions.InternalList.Contains(Identifiable.Id.NONE)) partialFashions.InternalList.Remove(Identifiable.Id.NONE);
            partialFashions.Push(data.fashions);
            
            partialEmotions.Push(data.emotions.emotionData);
        }

        public override void ReadData(BinaryReader reader)
        {
            if (ModdedSaveData.LATEST_READ_VERSION > 3) Version = reader.ReadInt32();
            partialFashions.Read(reader);
            partialEmotions.Read(reader);
        }
        
        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(LatestVersion);
           
            partialFashions.Write(writer);
            partialEmotions.Write(writer);
        }

        public override void Write(BinaryWriter writer)
        {
            WriteData(writer);
        }

        public override void Read(BinaryReader reader)
        {
            ReadData(reader);
        }

        static PartialActorData()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, PartialActorData v) =>
                {
                    
                    translator.FixEnumValues(mode,v.partialEmotions);
                    translator.FixEnumValues(mode,v.partialFashions);
                });
        }
    }
}
