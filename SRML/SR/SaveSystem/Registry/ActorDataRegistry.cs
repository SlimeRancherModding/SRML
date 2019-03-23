using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem.Registry
{
    internal class ActorDataRegistry
    {
        public Dictionary<int,Func<ICustomActorData<ActorModel>>> actorDataIds = new Dictionary<int, Func<ICustomActorData<ActorModel>>>();

        public void AddCustomActorData<T>(int id, Type dataType) where T : ActorModel
        {
            AddCustomActorData<T>(id, () => ((ICustomActorData<T>)Activator.CreateInstance(dataType)));
        }

        public void AddCustomActorData<T>(int id, Func<ICustomActorData<T>> creator) where T : ActorModel
        {
            actorDataIds.Add(id,()=>new ActorDataWrapper<T>(creator()));
        }

        public ICustomActorData<ActorModel> GetDataForID(int id)
        {
            return actorDataIds[id]();
        }

        public void RegisterSerializableModel<T>(int id) where T : ActorModel, ISerializableModel
        {
            AddCustomActorData(id,()=>new BinaryActorData<T>());
        }
    }
}