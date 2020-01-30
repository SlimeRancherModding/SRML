using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static AchievementsDirector;

namespace SRML.SR.SaveSystem.Data.Profile
{
    class PartialProfileAchieveData : VersionedPartialData<PlayerAchievementsV03>
    {
        public override int LatestVersion => 0;

        public PartialAchievementsProgressData progress = new PartialAchievementsProgressData();
        public PartialCollection<Achievement> earnedAchievements = new PartialCollection<Achievement>(ModdedIDRegistry.IsModdedID,SerializerPair.GetEnumSerializerPair<Achievement>(), ModdedIDRegistry.IsModdedID);

        public override void Pull(PlayerAchievementsV03 data)
        {
            var list = data.earnedAchievements.Select(x => (Achievement)x).ToList();
            earnedAchievements.Pull(list);
            data.earnedAchievements = list.Select(x => Convert.ToInt32(x)).ToList();
            progress.Pull(data.progress);
        }

        public override void Push(PlayerAchievementsV03 data)
        {
            var list = new List<Achievement>();
            earnedAchievements.Push(list);
            data.earnedAchievements.AddRange(list.Select(x => (int)x));
            progress.Push(data.progress);
        }

        public override void ReadData(BinaryReader reader)
        {
            earnedAchievements.Read(reader);
            progress.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            earnedAchievements.Write(writer);
            progress.Write(writer);
        }

        static PartialProfileAchieveData()
        {
            PartialData.RegisterPartialData<PlayerAchievementsV03, PartialProfileAchieveData>();
            EnumTranslator.RegisterEnumFixer<PartialProfileAchieveData>((trans, mode, obj) =>
            {
                trans.FixEnumValues(mode, obj.progress);
                trans.FixEnumValues(mode, obj.earnedAchievements);
            });

            CustomChecker.RegisterCustomChecker<PlayerAchievementsV03>(obj =>
            {
                if (obj.earnedAchievements.Any(x=>ModdedIDRegistry.IsModdedID((AchievementsDirector.Achievement)x))) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.GetCustomLevel(obj.progress);
            });

            
        }
        public class PartialAchievementsProgressData : VersionedPartialData<PlayerAchievementProgressV01>
        {
            public override int LatestVersion => 0;
            public PartialDictionary<AchievementsDirector.BoolStat,bool> events = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.BoolStat,bool>(SerializerPair.BOOL);
            public PartialDictionary<AchievementsDirector.IntStat,int> counts = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.IntStat,int>(SerializerPair.INT32);
            public PartialDictionary<AchievementsDirector.EnumStat,List<Enum>> listsFullCustom  = PartialDataUtils.CreatePartialDictionaryWithEnumKey<AchievementsDirector.EnumStat,List<Enum>>(new SerializerPair<List<Enum>>((writer,obj) => BinaryUtils.WriteList(writer,obj,enumSerializer.SerializeGeneric),(reader)=> { var list = new List<Enum>(); BinaryUtils.ReadList(reader, list, enumSerializer.DeserializeGeneric); return list; }));
            public Dictionary<AchievementsDirector.EnumStat, PartialCollection<Enum>> listsPartialCustom = new Dictionary<AchievementsDirector.EnumStat, PartialCollection<Enum>>();
            static SerializerPair<Enum> enumSerializer = new SerializerPair<Enum>(
                        (writer, obj) =>
                        {
                            writer.Write(obj.GetType().AssemblyQualifiedName);
                            writer.Write(Convert.ToInt32(obj));
                        },
                        (reader) =>
                        {
                            var type = Type.GetType(reader.ReadString());
                            return (Enum)Enum.ToObject(type, reader.ReadInt32());
                        }
                        );
            static PartialCollection<Enum> CreatePartialCollection()
            {
                
                return new PartialCollection<Enum>(x => ModdedIDRegistry.IsModdedID(x),
                    enumSerializer,
                    x=> ModdedIDRegistry.IsModdedID(x));
            }
            public override void Pull(PlayerAchievementProgressV01 data)
            {
                events.Pull(data.events.Select(x=>new KeyValuePair<AchievementsDirector.BoolStat,bool>((AchievementsDirector.BoolStat)x.Key,x.Value)).ToDictionary(x=>x.Key,x=>x.Value));
                foreach (var v in events.hoistedValues) data.events.Remove((int)v.Key);
                counts.Pull(data.counts.Select(x => new KeyValuePair<AchievementsDirector.IntStat, int>((AchievementsDirector.IntStat)x.Key, x.Value)).ToDictionary(x => x.Key, x => x.Value));
                foreach (var v in counts.hoistedValues) data.counts.Remove((int)v.Key);
                listsFullCustom.Pull(data.lists.Select(x => new KeyValuePair<AchievementsDirector.EnumStat, List<Enum>>((AchievementsDirector.EnumStat)x.Key, x.Value)).ToDictionary(x => x.Key, x => x.Value));
                foreach (var v in listsFullCustom.hoistedValues) data.lists.Remove((int)v.Key);

                foreach (var v in data.lists)
                {
                    if (v.Value.Any(x => ModdedIDRegistry.IsModdedID(x)))
                    {
                        var list = CreatePartialCollection();
                        list.Pull(v.Value);
                        listsPartialCustom[(AchievementsDirector.EnumStat)v.Key] = list;
                    }
                }

            }

            static void PushToIntDict<T,K>(PartialDictionary<T,K> partialDict,Dictionary<int,K> real)
            {
                var dict = new Dictionary<T, K>();
                partialDict.Push(dict);
                foreach (var v in dict) real[((int)(object)v.Key)] = v.Value;
            } 

            public override void Push(PlayerAchievementProgressV01 data)
            {
                PushToIntDict(events, data.events);
                PushToIntDict(counts, data.counts);
                PushToIntDict(listsFullCustom, data.lists);

                
                foreach(var v in listsPartialCustom)
                {
                    var intKey = Convert.ToInt32(v.Key);
                    if (!data.lists.ContainsKey(intKey)) data.lists[intKey] = new List<Enum>();
                    v.Value.Push(data.lists[intKey]);
                }
            }

            public override void ReadData(BinaryReader reader)
            {
                events.Read(reader);
                counts.Read(reader);
                listsFullCustom.Read(reader);

                int count = reader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    var part = CreatePartialCollection();
                    listsPartialCustom[(AchievementsDirector.EnumStat)reader.ReadInt32()] = part;
                    part.Read(reader);

                }
            }

            public override void WriteData(BinaryWriter writer)
            {
                events.Write(writer);
                counts.Write(writer);
                listsFullCustom.Write(writer);

                int count = listsPartialCustom.Count;
                writer.Write(count);
                foreach(var v in listsPartialCustom)
                {
                    writer.Write(Convert.ToInt32(v.Key));
                    v.Value.Write(writer);
                }
            }

            static bool GetIsCustom<T>(int t)
            {
                var obj = Enum.ToObject(typeof(T), t);
                return ModdedIDRegistry.IsModdedID(obj);
            }

            static PartialAchievementsProgressData()
            {
                PartialData.RegisterPartialData<PlayerAchievementProgressV01, PartialAchievementsProgressData>();
                EnumTranslator.RegisterEnumFixer<PartialAchievementsProgressData>((trans, mode, obj) =>
                {
                    trans.FixEnumValues(mode, obj.counts);
                    trans.FixEnumValues(mode, obj.events);
                    trans.FixEnumValues(mode, obj.listsFullCustom);
                    trans.FixEnumValues(mode, obj.listsPartialCustom);
                });

                CustomChecker.RegisterCustomChecker<PlayerAchievementProgressV01>((obj) =>
                {
                    if (obj.events.Any(x => GetIsCustom<BoolStat>(x.Key)) ||
                    obj.counts.Any(x => GetIsCustom<IntStat>(x.Key)) ||
                    obj.lists.Any(x => GetIsCustom<EnumStat>(x.Key) || x.Value.Any(ModdedIDRegistry.IsModdedID))) return CustomChecker.CustomLevel.PARTIAL;
                    return CustomChecker.CustomLevel.VANILLA;

                });
            }
        }
    }
}
