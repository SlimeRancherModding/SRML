using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Macro
{
    public class PartialGameAchieveData : VersionedPartialData<GameAchieveV03>
    {
        public override int LatestVersion => 0;

        public PartialDictionary<AchievementsDirector.GameDoubleStat, double> gameDoubleStatDict = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.GameDoubleStat, double>(SerializerPair.DOUBLE);
        public PartialDictionary<AchievementsDirector.GameFloatStat, float> gameFloatStatDict = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.GameFloatStat, float>(SerializerPair.FLOAT);
        public PartialDictionary<AchievementsDirector.GameIntStat, int> gameIntStatDict = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.GameIntStat, int>(SerializerPair.INT32);
        static SerializerPair<Identifiable.Id> idSerializer = SerializerPair.GetEnumSerializerPair<Identifiable.Id>();
        
        public PartialDictionary<AchievementsDirector.GameIdDictStat, Dictionary<Identifiable.Id, int>> gameIdDictStatDictFullCustom = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.GameIdDictStat, Dictionary<Identifiable.Id, int>>(new SerializerPair<Dictionary<Identifiable.Id, int>>((x,y)=>BinaryUtils.WriteDictionary(x,y,idSerializer.SerializeGeneric,SerializerPair.INT32.SerializeGeneric),(x)=> { var v = new Dictionary<Identifiable.Id, int>(); BinaryUtils.ReadDictionary<Identifiable.Id, int>(x, v, idSerializer.DeserializeGeneric, SerializerPair.INT32.DeserializeGeneric); return v; }));
        public Dictionary<AchievementsDirector.GameIdDictStat, PartialDictionary<Identifiable.Id, int>> gameIdDictStatDictPartialCustom = new Dictionary<AchievementsDirector.GameIdDictStat, PartialDictionary<Identifiable.Id, int>>();

        static PartialDictionary<Identifiable.Id, int> CreateDict() => PartialDataUtils.CreatePartialDictionaryWithEnumKey<Identifiable.Id,int>(SerializerPair.INT32);
        public override void Pull(GameAchieveV03 data)
        {
            gameDoubleStatDict.Pull(data.gameDoubleStatDict);
            gameFloatStatDict.Pull(data.gameFloatStatDict);
            gameIntStatDict.Pull(data.gameIntStatDict);
            gameIdDictStatDictFullCustom.Pull(data.gameIdDictStatDict);

            foreach (var v in data.gameIdDictStatDict)
            {
                if (v.Value.Any(x => ModdedIDRegistry.IsModdedID(x.Key)))
                {
                    var dict = CreateDict();
                    dict.Pull(v.Value);
                    gameIdDictStatDictPartialCustom[v.Key] = dict;
                }
            }
        }

        public override void Push(GameAchieveV03 data)
        {
            gameDoubleStatDict.Pull(data.gameDoubleStatDict);
            gameFloatStatDict.Pull(data.gameFloatStatDict);
            gameIntStatDict.Pull(data.gameIntStatDict);
            gameIdDictStatDictFullCustom.Pull(data.gameIdDictStatDict);

            foreach(var v in gameIdDictStatDictPartialCustom)
            {
                if (!data.gameIdDictStatDict.ContainsKey(v.Key)) data.gameIdDictStatDict[v.Key] = new Dictionary<Identifiable.Id, int>();
                v.Value.Push(data.gameIdDictStatDict[v.Key]);
            }

        }

        static SerializerPair<AchievementsDirector.GameIdDictStat> dictPair = SerializerPair.GetEnumSerializerPair<AchievementsDirector.GameIdDictStat>();
        static SerializerPair<Dictionary<AchievementsDirector.GameIdDictStat, PartialDictionary<Identifiable.Id, int>>> dictSerializer = new SerializerPair<Dictionary<AchievementsDirector.GameIdDictStat, PartialDictionary<Identifiable.Id, int>>>((reader, obj) =>
          {

              BinaryUtils.WriteDictionary(reader, obj, dictPair.SerializeGeneric, (x, y) => { y.Write(x); });
          },
          (reader) =>
          {
              var dict = new Dictionary<AchievementsDirector.GameIdDictStat, PartialDictionary<Identifiable.Id, int>>();
              BinaryUtils.ReadDictionary(reader, dict, dictPair.DeserializeGeneric, (reader1) =>
              {
                  var p = CreateDict();
                  p.Read(reader1);
                  return p;
              });
              return dict;
          }
          );

        public override void ReadData(BinaryReader reader)
        {
            gameDoubleStatDict.Read(reader);
            gameFloatStatDict.Read(reader);
            gameIntStatDict.Read(reader);
            gameIdDictStatDictFullCustom.Read(reader);

            gameIdDictStatDictPartialCustom = dictSerializer.DeserializeGeneric(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            gameDoubleStatDict.Write(writer);
            gameFloatStatDict.Write(writer);
            gameIntStatDict.Write(writer);
            gameIdDictStatDictFullCustom.Write(writer);

            dictSerializer.SerializeGeneric(writer,gameIdDictStatDictPartialCustom);
        }

        static PartialGameAchieveData()
        {
            PartialData.RegisterPartialData<GameAchieveV03, PartialGameAchieveData>();
            EnumTranslator.RegisterEnumFixer<PartialGameAchieveData>((trans, mode, obj) =>
            {
                trans.FixEnumValues(mode, obj.gameDoubleStatDict);
                trans.FixEnumValues(mode, obj.gameFloatStatDict);
                trans.FixEnumValues(mode, obj.gameIntStatDict);
                trans.FixEnumValues(mode, obj.gameIdDictStatDictPartialCustom);
                trans.FixEnumValues(mode, obj.gameIdDictStatDictFullCustom);


            });

            CustomChecker.RegisterCustomChecker<GameAchieveV03>((obj) =>
            {
                if (obj.gameDoubleStatDict.Any(x => ModdedIDRegistry.IsModdedID(x.Key)) ||
                obj.gameFloatStatDict.Any(x => ModdedIDRegistry.IsModdedID(x.Key)) ||
                obj.gameIntStatDict.Any(x => ModdedIDRegistry.IsModdedID(x.Key)) ||
                obj.gameIdDictStatDict.Any(x => ModdedIDRegistry.IsModdedID(x.Key) || x.Value.Any(y => ModdedIDRegistry.IsModdedID(y.Key)))) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
        }
    }
}
