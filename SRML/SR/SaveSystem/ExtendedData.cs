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
                foreach (var v in mod.extendedData)
                {
                    switch (v.idType)
                    {
                        case ExtendedDataTree.IdentifierType.ACTOR:
                            var list = GetPieceForMod(mod.modid,GetDataForActor(v.identifier)).dataList;
                            foreach (var h in v.dataPiece.dataList)
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

        internal static void OnRegisterActor(GameModel model,long actorId, GameObject gameObject,bool skipNotify)
        {
            if (Identifiable.GetId(gameObject) == Identifiable.Id.NONE) return;

            if (IsRegistered(actorId))
            {
                foreach (var v in SaveRegistry.modToSaveInfo)
                {
                    if(extendedActorData[actorId].HasPiece(v.Key.ModInfo.Id)) v.Value.OnExtendedActorDataLoaded(model.actors[actorId], gameObject, GetPieceForMod(v.Key.ModInfo.Id, extendedActorData[actorId]));
                }

                
                foreach (var p in gameObject.GetComponentsInChildren<Participant>())
                {
                    try
                    {
                        SetParticipant(p, (extendedActorData[actorId]));
                    }
                    catch(InvalidOperationException e)
                    {
                        Debug.Log($"Yipes! seems like {p.GetType()} isn't initialized!");
                        // a bit gross hack but it'll help when mods add new participants to things that already have actor data stored
                        InitParticipant(p, (extendedActorData[actorId]));
                        SetParticipant(p, (extendedActorData[actorId]));
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

            foreach (var v in toRemove)
            {
                extendedActorData.Remove(v);
            }

            if (toRemove.Count > 0)
                Debug.Log($"Culled {toRemove.Count} extended actor data's without any respective actormodel");
        }

        internal static void DestroyActor(GameObject b)
        {
            long id = Identifiable.GetActorId(b);
            if (IsRegistered(id)) extendedActorData.Remove(id);
        }

        internal static void Push(ModdedSaveData data)
        {
            foreach (var v in extendedActorData)
            {
                foreach (CompoundDataPiece h in v.Value.dataList)
                {
                    var seg = data.GetSegmentForMod(SRModLoader.GetMod(h.key));
                    var newCompound = new CompoundDataPiece("root");
                    foreach (var dat in h.dataList)
                    {
                        newCompound.dataList.Add(dat);
                    }
                    seg.extendedData.Add(new ExtendedDataTree()
                    {
                        dataPiece = newCompound,
                        identifier = v.Key,
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

        public static T AddNewParticipant<T>(GameObject b) where T : Component, Participant
        {
            RegisterExtendedActorData(b);
            var newPart = b.AddComponent<T>();
            var id = Identifiable.GetActorId(b);
            InitParticipant(newPart, GetDataForActor(id));
            SetParticipant(newPart, GetDataForActor(id));
            return newPart;
        }

        public static void RegisterExtendedActorData(long actorId, GameObject obj,bool skipNotify)
        {
            if (IsRegistered(actorId)) return;
            var tag = GetDataForActor(actorId);
            var participants = obj.GetComponents<Participant>();
            foreach (var v in participants)
            {
                InitParticipant(v,tag);
            }

            if (skipNotify) return;
            foreach (var v in participants)
            {
                SetParticipant(v, tag);
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

        public interface Participant
        {
            void InitData(CompoundDataPiece piece);
            void SetData(CompoundDataPiece piece);
        }
    }
}
