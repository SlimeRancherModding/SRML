using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Actor;
using UnityEngine;

namespace SRML.SR.SaveSystem.Registry
{
    internal class ModSaveInfo 
    {
        DataRegistry<CustomActorData> CustomActorDataRegistry = new DataRegistry<CustomActorData>();
        public readonly HashSet<DataRegistry> Registries = new HashSet<DataRegistry>();

        public delegate void ExtendedActorDataLoaded(ActorModel actor, GameObject obj, CompoundDataPiece data);


        public ExtendedActorDataLoaded onExtendedActorDataLoaded;

        public void OnExtendedActorDataLoaded(ActorModel model, GameObject obj, CompoundDataPiece piece)
        {
            onExtendedActorDataLoaded?.Invoke(model, obj, piece);
        }


        public ModSaveInfo()
        {
            Registries.Add(CustomActorDataRegistry);
        }

        public bool BelongsToMe(object b)
        {
            return Registries.Any((x) => x.BelongsToMe(b));
        }

        public bool IsModelRegistered(Type model)
        {
            return Registries.Any((x) => x.IsModelRegistered(model));
        }

        public DataRegistry<T> GetRegistryFor<T>() where T : IDataRegistryMember
        {
            foreach (var r in Registries)
            {
                if (r is DataRegistry<T> reg) return reg;
            }
            return null;
        }
    }
}
