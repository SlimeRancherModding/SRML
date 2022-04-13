using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Format;
using SRML.SR.SaveSystem.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR.SaveSystem
{

    /// <summary>
    /// Class allowing for the addition of arbitrary data to Actors, similar to minecrafts nbt system
    /// </summary>
    public static class ExtendedData
    {
        internal static Dictionary<DataIdentifier, PreparedData> preparedData = new Dictionary<DataIdentifier, PreparedData>();

        internal static Dictionary<SRMod, CompoundDataPiece> worldSaveData = new Dictionary<SRMod, CompoundDataPiece>();

        internal static void Pull(ModdedSaveData data)
        {
            Clear();
            foreach (var mod in data.segments)
            {
                Debug.Log($"mod {mod.modid} has {mod.extendedData.Count} extended actor datas");
                foreach (var extendedDataTree in mod.extendedData)
                {
                    switch (extendedDataTree.idType)
                    {
                        case ExtendedDataTree.IdentifierType.ACTOR:
                            var identifier = DataIdentifier.GetActorIdentifier(extendedDataTree.longIdentifier);
                            PreparedData pdata;
                            if (!preparedData.TryGetValue(identifier, out pdata)) {
                                pdata = new PreparedData() { Data = new CompoundDataPiece("root"), SourceType = PreparedData.PreparationSource.SPAWN };
                                preparedData[identifier] = pdata;
                            }
                            extendedDataTree.dataPiece.DataList.Do((x) => pdata.Data.GetCompoundPiece(mod.modid).DataList.Add(x));
                            break;
                        default:
                            throw new NotImplementedException();

                    }
                }
                var actualMod = SRModLoader.GetMod(mod.modid);
                if (actualMod == null) continue;
                worldSaveData.Add(actualMod, mod.extendedWorldData);
                SaveRegistry.GetSaveInfo(actualMod).WorldDataPreLoad(mod.extendedWorldData);
            }
            foreach (var v in SRModLoader.GetMods())
            {
                if (!worldSaveData.ContainsKey(v))
                {
                    var newData = new CompoundDataPiece("root");
                    worldSaveData.Add(v, newData);
                    SaveRegistry.GetSaveInfo(v).WorldDataPreLoad(newData);
                }
            }
        }

        public static CompoundDataPiece GetWorldSaveData()
        {
            return worldSaveData.Get(SRMod.GetCurrentMod());
        }

        internal static void Clear()
        {
            preparedData.Clear();
            worldSaveData.Clear();
        }

        /// <summary>
        /// Instantiate an an Actor with the given data
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GameObject InstantiateActorWithData(GameObject prefab, RegionRegistry.RegionSetId id, Vector3 pos, Quaternion rot,
            CompoundDataPiece data)
        {
            long actorId;
            SceneContext.Instance.GameModel.nextActorId = (actorId = SceneContext.Instance.GameModel.nextActorId) + 1L;
            return InstantiateActorWithData(actorId, prefab, id, pos, rot, data);
        }

        public static GameObject InstantiateActorWithData(long actorId, GameObject prefab, RegionRegistry.RegionSetId id, Vector3 pos, Quaternion rot, CompoundDataPiece data)
        {
            preparedData.Add(DataIdentifier.GetActorIdentifier(actorId), new PreparedData() { Data = data, SourceType = PreparedData.PreparationSource.SPAWN });
            return SceneContext.Instance.GameModel.InstantiateActor(actorId, prefab, id, pos, rot, false, false);
        }

        internal static void CullMissingModsFromData(CompoundDataPiece piece)
        {
            List<DataPiece> toRemove = new List<DataPiece>();
            foreach (var v in piece.DataList)
            {
                if (SRModLoader.GetMod(v.key) == null) toRemove.Add(v);
            }

            foreach (var v in toRemove)
            {
                piece.DataList.Remove(v);
            }
        }

        internal static void OnRegisterActor(GameModel model, long actorId, GameObject gameObject, bool skipNotify)
        {
            if (Identifiable.GetId(gameObject) == Identifiable.Id.NONE) return;

            var actorIdentifier = DataIdentifier.GetActorIdentifier(actorId);
            if (preparedData.TryGetValue(actorIdentifier, out var pdata))
            {
                var data = pdata.Data;
                foreach (var saveInfoPair in SaveRegistry.modToSaveInfo)
                {
                    if (data.HasPiece(saveInfoPair.Key.ModInfo.Id)) saveInfoPair.Value.OnExtendedActorDataLoaded(model.actors[actorId], gameObject, ExtendedDataUtils.GetPieceForMod(saveInfoPair.Key.ModInfo.Id, data));
                }
                ApplyDataToGameObject(gameObject, data);
                preparedData.Remove(actorIdentifier);
            }




        }

        public static bool HasExtendedData(GameObject obj) => obj.GetComponents<ExtendedData.Participant>().Length > 0;
        
        internal static void Push(ModdedSaveData data)
        {
            foreach (var p in SRModLoader.Mods)
                if (SaveRegistry.GetSaveInfo(p.Value).OnWorldSave != null && !ExtendedData.worldSaveData.ContainsKey(p.Value))
                    ExtendedData.worldSaveData.Add(p.Value, new CompoundDataPiece(p.Key));

            foreach (var actorData in GetAllData(SceneContext.Instance.GameModel))
            {
                foreach (CompoundDataPiece modPiece in actorData.Value.DataList)
                {
                    var mod = SRModLoader.GetMod(modPiece.key);
                    var seg = data.GetSegmentForMod(mod);
                    var newCompound = new CompoundDataPiece("root");
                    foreach (var dat in modPiece.DataList)
                    {
                        newCompound.DataList.Add(dat);
                    }
                    seg.extendedData.Add(new ExtendedDataTree()
                    {
                        dataPiece = newCompound,
                        longIdentifier = actorData.Key.longID,
                        stringIdentifier = actorData.Key.stringID ?? "",
                        idType = ExtendedDataTree.IdentifierType.ACTOR
                    });
                }
            }
            foreach(var pair in worldSaveData)
            {
                SaveRegistry.GetSaveInfo(pair.Key).WorldDataSave(pair.Value);
                if (pair.Value.DataList.Count > 0)
                {

                    data.GetSegmentForMod(pair.Key).extendedWorldData = pair.Value;
                }   
            }
        }

        internal static CompoundDataPiece GetDataForCurrentMod(CompoundDataPiece piece)
        {
            var strin = SRMod.GetCurrentMod().ModInfo.Id;
            return ExtendedDataUtils.GetPieceForMod(strin, piece);
        }

        static CompoundDataPiece GetDataForActor(long actorId)
        {
            return SceneContext.Instance.GameModel.actors.TryGetValue(actorId, out var model) ? ReadDataFromGameObject(model.transform.gameObject) : null;
        }

        static bool IsValid(long actorId)
        {
            return SceneContext.Instance.GameModel.actors.ContainsKey(actorId);
        }

        static ExtendedData()
        {
            SRCallbacks.OnSaveGameLoaded += (s) =>
            {
                foreach (var v in worldSaveData)
                {
                    SaveRegistry.GetSaveInfo(v.Key)?.WorldDataLoad(v.Value);
                }
            };
        }

        static bool HasValidDataForParticipant(Participant p, CompoundDataPiece piece)
        {
            var modid = ExtendedDataUtils.GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            return piece.HasPiece(modid) && piece.GetCompoundPiece(modid).HasPiece(ExtendedDataUtils.GetParticipantName(p));
        }
            
        public static void ApplyDataToGameObject(GameObject obj, CompoundDataPiece piece)
        {
            foreach(var participant in obj.GetComponents<Participant>())
            {
                if (HasValidDataForParticipant(participant, piece)) participant.ReadData(ExtendedDataUtils.GetPieceForParticipantFromRoot(participant, piece));
            }
        }

        public static CompoundDataPiece ReadDataFromGameObject(GameObject obj)
        {
            var newCompound = new CompoundDataPiece("root");
            foreach(var participant in obj.GetComponents<Participant>())
            {
                participant.WriteData(ExtendedDataUtils.GetPieceForParticipantFromRoot(participant,newCompound));
            }

            return newCompound;
        }



        static IEnumerable<KeyValuePair<DataIdentifier,CompoundDataPiece>> GetAllData(GameModel model)
        {
            foreach(var v in model.actors)
            {
                if (v.Value?.transform == null) continue;
                yield return new KeyValuePair<DataIdentifier, CompoundDataPiece>(DataIdentifier.GetActorIdentifier(v.Key), ReadDataFromGameObject(v.Value.transform.gameObject));
            }
        }
        /// <summary>
        /// A participant of the data system
        /// </summary>
        public interface Participant
        {
            void ReadData(CompoundDataPiece piece);
            void WriteData(CompoundDataPiece piece);
        }

        internal struct PreparedData
        {
            public CompoundDataPiece Data;
            public PreparationSource SourceType;
            public object Source;
            public enum PreparationSource
            {
                UNKNOWN,
                AMMO,
                SPAWN
            }

        }
    }
}
