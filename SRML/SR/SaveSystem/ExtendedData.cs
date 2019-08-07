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
    public static class ExtendedData
    {
        internal static Dictionary<DataIdentifier, CompoundDataPiece> extendedData =
            new Dictionary<DataIdentifier, CompoundDataPiece>();
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
                            var list = ExtendedDataUtils.GetPieceForMod(mod.modid, GetDataForActor(extendedDataTree.longIdentifier)).DataList;
                            foreach (var h in extendedDataTree.dataPiece.DataList)
                            {
                                list.Add(h);
                            }
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
        }

        public static CompoundDataPiece GetWorldSaveData()
        {
            return worldSaveData.Get(SRMod.GetCurrentMod());
        }

        internal static void Clear()
        {
            extendedData.Clear();
            preparedData.Clear();
            worldSaveData.Clear();
        }
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
            PreparedData pdata = new PreparedData();
            if (IsRegistered(actorId) || preparedData.TryGetValue(DataIdentifier.GetActorIdentifier(actorId), out pdata))
            {
                if (pdata.Data != null) extendedData[DataIdentifier.GetActorIdentifier(actorId)] = pdata.Data;
                foreach (var saveInfoPair in SaveRegistry.modToSaveInfo)
                {
                    if (extendedData[actorIdentifier].HasPiece(saveInfoPair.Key.ModInfo.Id)) saveInfoPair.Value.OnExtendedActorDataLoaded(model.actors[actorId], gameObject, ExtendedDataUtils.GetPieceForMod(saveInfoPair.Key.ModInfo.Id, extendedData[actorIdentifier]));
                }

                foreach (var participant in gameObject.GetComponentsInChildren<ExtendedData.Participant>())
                    if (!ValidateParticipant(participant, (extendedData[actorIdentifier])))
                        InitParticipant(participant, (extendedData[actorIdentifier]));

                foreach (var participant in gameObject.GetComponentsInChildren<ExtendedData.Participant>())
                {
                    try
                    {
                        SetParticipant(participant, (extendedData[actorIdentifier]));
                    }
                    catch (InvalidOperationException)
                    {
                        Debug.Log($"Yipes! seems like {participant.GetType()} isn't initialized!");
                        // a bit gross hack but it'll help when mods add new participants to things that already have actor data stored
                        InitParticipant(participant, (extendedData[actorIdentifier]));
                        SetParticipant(participant, (extendedData[actorIdentifier]));
                    }
                }

            }
            else
            {
                var participants = gameObject.GetComponents<ExtendedData.Participant>();
                if (participants != null && participants.Length > 0)
                {
                    RegisterExtendedActorData(actorId, gameObject, skipNotify);
                }
            }




        }

        internal static void CullIfNotValid(GameModel model)
        {

            List<DataIdentifier> toRemove = new List<DataIdentifier>();
            foreach (var actor in extendedData)
            {
                if (!model.actors.ContainsKey(actor.Key.longID) || actor.Value.DataList.Count == 0)
                {
                    toRemove.Add(actor.Key);
                }
            }

            foreach (var actor in toRemove)
            {
                extendedData.Remove(actor);
            }

            if (toRemove.Count > 0)
                Debug.Log($"Culled {toRemove.Count} invalid actor datas");
        }

        internal static void DestroyActor(GameObject b)
        {
            if (b?.GetComponent<Identifiable>()?.model == null) return;
            long id = Identifiable.GetActorId(b);
            if (IsRegistered(id)) extendedData.Remove(DataIdentifier.GetActorIdentifier(id));
        }


        public static void UnregisterActor(GameObject b, bool removeParticipants = true)
        {
            if (b?.GetComponent<Identifiable>()?.model == null) return;
            long id = Identifiable.GetActorId(b);
            if (IsRegistered(id)) extendedData.Remove(DataIdentifier.GetActorIdentifier(id));
            if (!removeParticipants) return;
            foreach (Component participant in b.GetComponents<ExtendedData.Participant>())
            {
                MonoBehaviour.Destroy(participant);
            }
        }

        public static void RemoveParticipant<T>(GameObject b) where T : ExtendedData.Participant
        {
            if (b?.GetComponent<Identifiable>()?.model == null) return;
            long id = Identifiable.GetActorId(b);
            if (IsRegistered(id))
            {
                var part = b.GetComponent<T>();
                if (part != null)
                {
                    var piece = GetDataForActor(id);
                    var modPiece = ExtendedDataUtils.GetPieceForMod(ExtendedDataUtils.GetModForParticipant(part)?.ModInfo.Id, piece);
                    var participantPiece = ExtendedDataUtils.GetPieceForParticipant<T>(modPiece);
                    MonoBehaviour.Destroy(part as UnityEngine.Object);
                    modPiece.DataList.Remove(participantPiece);
                    if (ExtendedDataUtils.GetParticipantCount(modPiece) == 0) piece.DataList.Remove(modPiece);
                    if (ExtendedDataUtils.GetModPieceCount(piece) == 0) UnregisterActor(b);
                }
            }
        }

        internal static void Push(ModdedSaveData data)
        {
            foreach (var actorData in extendedData)
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
                if (pair.Value.DataList.Count > 0)
                {
                    SaveRegistry.GetSaveInfo(pair.Key).WorldDataSave(pair.Value);
                    data.GetSegmentForMod(pair.Key).extendedWorldData = pair.Value;
                }   
            }
        }

        internal static CompoundDataPiece GetDataForCurrentMod(CompoundDataPiece piece)
        {
            var strin = SRMod.GetCurrentMod().ModInfo.Id;
            return ExtendedDataUtils.GetPieceForMod(strin, piece);
        }

        public static void RegisterExtendedActorData(GameObject obj)
        {
            RegisterExtendedActorData(Identifiable.GetActorId(obj), obj, false);
        }

        public static bool IsRegistered(long id)
        {
            return extendedData.ContainsKey(DataIdentifier.GetActorIdentifier(id));
        }

        public static bool IsRegistered(GameObject b)
        {
            return IsRegistered(Identifiable.GetActorId(b));
        }

        public static T AddNewParticipant<T>(GameObject gameObj) where T : Component, ExtendedData.Participant
        {
            if (!IsRegistered(gameObj)) RegisterExtendedActorData(gameObj);
            var newPart = gameObj.AddComponent<T>();
            var id = Identifiable.GetActorId(gameObj);
            if (!ValidateParticipant(newPart, GetDataForActor(id))) InitParticipant(newPart, GetDataForActor(id));
            SetParticipant(newPart, GetDataForActor(id));
            return newPart;
        }

        public static void RegisterExtendedActorData(long actorId, GameObject obj, bool skipNotify)
        {

            if (IsRegistered(actorId))
            {
                Debug.LogWarning(obj.name + " is already registered!");
                return;
            }


            var tag = GetDataForActor(actorId);
            var participants = obj.GetComponents<ExtendedData.Participant>();
            foreach (var participant in participants)
            {
                if (!ValidateParticipant(participant, tag)) InitParticipant(participant, tag);
            }

            foreach (var participant in participants)
            {
                SetParticipant(participant, tag);
            }
        }

        static CompoundDataPiece GetDataForActor(long actorId)
        {
            if (!IsRegistered(actorId)) extendedData.Add(DataIdentifier.GetActorIdentifier(actorId), new CompoundDataPiece("root"));
            return extendedData[DataIdentifier.GetActorIdentifier(actorId)];
        }

        static bool IsValid(long actorId)
        {
            return SceneContext.Instance.GameModel.actors.ContainsKey(actorId);
        }


        static void SetParticipant(ExtendedData.Participant p, CompoundDataPiece piece)
        {
            var modid = ExtendedDataUtils.GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            p.SetData(ExtendedDataUtils.GetPieceForParticipantFromRoot(modid, p, piece));
        }

        static void InitParticipant(ExtendedData.Participant p, CompoundDataPiece piece)
        {
            var modid = ExtendedDataUtils.GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            p.InitData(ExtendedDataUtils.GetPieceForParticipantFromRoot(modid, p, piece));
        }

        static bool ValidateParticipant(ExtendedData.Participant p, CompoundDataPiece piece)
        {
            var modid = ExtendedDataUtils.GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            return p.IsDataValid(ExtendedDataUtils.GetPieceForParticipantFromRoot(modid, p, piece));
        }

        public interface Participant
        {
            void InitData(CompoundDataPiece piece);
            void SetData(CompoundDataPiece piece);
            bool IsDataValid(CompoundDataPiece piece);
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
