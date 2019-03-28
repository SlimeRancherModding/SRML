using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Format;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static SRML.SR.SaveSystem.Format.ExtendedDataTree;

namespace SRML.SR.SaveSystem
{
    public static class ExtendedData
    {
        internal static Dictionary<long, CompoundDataPiece> extendedActorData =
            new Dictionary<long, CompoundDataPiece>();

        internal static void Pull(ModdedSaveData data)
        {
            extendedActorData.Clear();
            foreach (var mod in data.segments)
            {
                Debug.Log($"mod {mod.modid} has {mod.extendedData.Count} extended actor datas");
                foreach (var extendedDataTree in mod.extendedData)
                {
                    switch (extendedDataTree.idType)
                    {
                        case IdentifierType.ACTOR:
                            var list = GetPieceForMod(mod.modid,GetDataForActor(extendedDataTree.identifier)).DataList;
                            foreach (var h in extendedDataTree.dataPiece.DataList)
                            {
                                list.Add(h);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                            
                    }
                }
            }
        }

        public static void InstantiateActorWithData(GameObject prefab, Vector3 pos, Quaternion rot,
            CompoundDataPiece data)
        {
            long actorId;
            SceneContext.Instance.GameModel.nextActorId = (actorId = SceneContext.Instance.GameModel.nextActorId) + 1L;
            InstantiateActorWithData(actorId,prefab,pos,rot,data);
        }

        public static void InstantiateActorWithData(long actorId, GameObject prefab, Vector3 pos, Quaternion rot,CompoundDataPiece data)
        {
            extendedActorData[actorId] = data;
            SceneContext.Instance.GameModel.InstantiateActor(actorId, prefab, pos, rot, false, false);
        }
            
        internal static void CullMissingModsFromData(CompoundDataPiece piece)
        {
            List<DataPiece> toRemove = new List<DataPiece>();
            foreach (var v in piece.DataList)
            {
                if(SRModLoader.GetMod(v.key) == null) toRemove.Add(v);
            }

            piece.DataList.RemoveWhere((x) => toRemove.Contains(x));
        }

        internal static void OnRegisterActor(GameModel model,long actorId, GameObject gameObject,bool skipNotify)
        {
            if (Identifiable.GetId(gameObject) == Identifiable.Id.NONE) return;

            if (IsRegistered(actorId))
            {
                foreach (var saveInfoPair in SaveRegistry.modToSaveInfo)
                {
                    if(extendedActorData[actorId].HasPiece(saveInfoPair.Key.ModInfo.Id)) saveInfoPair.Value.OnExtendedActorDataLoaded(model.actors[actorId], gameObject, GetPieceForMod(saveInfoPair.Key.ModInfo.Id, extendedActorData[actorId]));
                }

                foreach(var participant in gameObject.GetComponentsInChildren<Participant>())
                    if (!ValidateParticipant(participant, (extendedActorData[actorId])))
                        InitParticipant(participant, (extendedActorData[actorId]));

                foreach (var participant in gameObject.GetComponentsInChildren<Participant>())
                {
                    try
                    {
                        SetParticipant(participant, (extendedActorData[actorId]));
                    }
                    catch(InvalidOperationException)
                    {
                        Debug.Log($"Yipes! seems like {participant.GetType()} isn't initialized!");
                        // a bit gross hack but it'll help when mods add new participants to things that already have actor data stored
                        InitParticipant(participant, (extendedActorData[actorId]));
                        SetParticipant(participant, (extendedActorData[actorId]));
                    }
                }

            }
            else
            {   
                var participants = gameObject.GetComponents<Participant>();
                if (participants != null && participants.Length > 0)
                {
                    RegisterExtendedActorData(actorId, gameObject,skipNotify);
                }
            }
            
            


        }

        internal static void CullIfNotValid(GameModel model)
        {

            List<long> toRemove = new List<long>();
            foreach (var actor in extendedActorData)
            {
                if (!model.actors.ContainsKey(actor.Key))
                {
                    toRemove.Add(actor.Key);
                }
            }

            foreach (var actor in toRemove)
            {
                extendedActorData.Remove(actor);
            }

            if (toRemove.Count > 0)
                Debug.Log($"Culled {toRemove.Count} extended actor data's without any respective actormodel");
        }

        internal static void DestroyActor(GameObject b)
        {
            if (b?.GetComponent<Identifiable>()?.model == null) return;
            long id = Identifiable.GetActorId(b);
            if (IsRegistered(id)) extendedActorData.Remove(id);
        }

        internal static void Push(ModdedSaveData data)
        {
            foreach (var actorData in extendedActorData)
            {
                foreach (CompoundDataPiece modPiece in actorData.Value.DataList)
                {
                    var seg = data.GetSegmentForMod(SRModLoader.GetMod(modPiece.key));
                    var newCompound = new CompoundDataPiece("root");
                    foreach (var dat in modPiece.DataList)
                    {
                        newCompound.DataList.Add(dat);
                    }
                    seg.extendedData.Add(new ExtendedDataTree()
                    {
                        dataPiece = newCompound,
                        identifier = actorData.Key,
                        idType = IdentifierType.ACTOR
                    });
                }
            }
        }

        public static void RegisterExtendedActorData(GameObject obj)
        {
            RegisterExtendedActorData(Identifiable.GetActorId(obj), obj, false);
        }

        public static bool IsRegistered(long id)
        {
            return extendedActorData.ContainsKey(id);
        }

        public static bool IsRegistered(GameObject b)
        {
            return IsRegistered(Identifiable.GetActorId(b));
        }

        public static T AddNewParticipant<T>(GameObject gameObj) where T : Component, Participant
        {
            RegisterExtendedActorData(gameObj);
            var newPart = gameObj.AddComponent<T>();
            var id = Identifiable.GetActorId(gameObj);
            if (!ValidateParticipant(newPart, GetDataForActor(id))) InitParticipant(newPart, GetDataForActor(id));
            SetParticipant(newPart, GetDataForActor(id));
            return newPart;
        }

        public static void RegisterExtendedActorData(long actorId, GameObject obj,bool skipNotify)
        {

            if (IsRegistered(actorId))
            {
                Debug.LogWarning(obj.name + " is already registered!");
                return;
            }


            var tag = GetDataForActor(actorId);
            var participants = obj.GetComponents<Participant>();    
            foreach (var participant in participants)
            {
                if(!ValidateParticipant(participant,tag)) InitParticipant(participant,tag);
            }

            foreach (var participant in participants)
            {
                SetParticipant(participant, tag);
            }
        }

        static CompoundDataPiece GetDataForActor(long actorId)
        {
            if (!IsRegistered(actorId)) extendedActorData.Add(actorId, new CompoundDataPiece("root"));
            return extendedActorData[actorId];
        }

        static bool IsValid(long actorId)
        {
            return SceneContext.Instance.GameModel.actors.ContainsKey(actorId);
        }
        
        static SRMod GetModForParticipant(Participant p)
        {
            return SRModLoader.GetModForAssembly(p.GetType().Assembly);
        }

        static CompoundDataPiece GetPieceForMod(String modid, CompoundDataPiece piece)
        {
            return piece.GetCompoundPiece(modid);
        }

        static void SetParticipant(Participant p, CompoundDataPiece piece)
        {
            var modid = GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            p.SetData(GetPieceForMod(modid, piece));
        }

        static void InitParticipant(Participant p, CompoundDataPiece piece)
        {
            var modid = GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            p.InitData(GetPieceForMod(modid, piece));
        }

        static bool ValidateParticipant(Participant p, CompoundDataPiece piece)
        {
            var modid = GetModForParticipant(p)?.ModInfo.Id ?? "srml";
            return p.IsDataValid(GetPieceForMod(modid, piece));
        }

        public interface Participant
        {
            void InitData(CompoundDataPiece piece);
            void SetData(CompoundDataPiece piece);
            bool IsDataValid(CompoundDataPiece piece);
        }
    }
}
