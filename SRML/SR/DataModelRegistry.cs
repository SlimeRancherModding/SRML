using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

namespace SRML.SR
{
    public static class DataModelRegistry
    {
        public delegate ActorModel CreateActorDelegate(long actorid, Identifiable.Id ident, GameObject gameObj);
        internal static Dictionary<Predicate<Identifiable.Id>,CreateActorDelegate> actorOverrideMapping = new Dictionary<Predicate<Identifiable.Id>, CreateActorDelegate>();

        public static void RegisterActorModelOverride(Predicate<Identifiable.Id> pred, CreateActorDelegate creator)
        {
            actorOverrideMapping.Add(pred,creator);
        }

        public static void RegisterCustomActorModel(Identifiable.Id id, CreateActorDelegate del)
        {
            RegisterActorModelOverride((x)=>x==id,del);
        }

        public static void RegisterCustomActorModel(Identifiable.Id id, Type actorType)
        {
            if (!typeof(ActorModel).IsAssignableFrom(actorType))
                throw new Exception("Given type is not a valid ActorModel!");
            RegisterCustomActorModel(id,(x,y,z)=>(ActorModel)Activator.CreateInstance(actorType,x,y,z.transform));
        }
    }
}
