using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem.Registry
{
    internal class ActorDataRegistry
    {
        public Dictionary<int,Func<CustomActorData>> actorDataIds = new Dictionary<int, Func<CustomActorData>>();

        public Dictionary<Type, int> modelTypeToIds = new Dictionary<Type, int>();

        public void AddCustomActorData<T>(int id, Type dataType) where T : ActorModel
        {
            AddCustomActorData<T>(id, () => ((CustomActorData<T>)Activator.CreateInstance(dataType)));
        }

        public void AddCustomActorData<T>(int id, Func<CustomActorData<T>> creator) where T : ActorModel
        {
            actorDataIds.Add(id,()=>creator());
            modelTypeToIds.Add(typeof(T),id);
        }

        public CustomActorData GetDataForID(int id)
        {
            return actorDataIds[id]();
        }

        public int GetIDForModel(Type model)
        {
            return modelTypeToIds[model];
        }

        public void RegisterSerializableModel<T>(int id) where T : ActorModel, ISerializableModel
        {
            AddCustomActorData(id,()=>new BinaryActorData<T>());
        }
    }
}