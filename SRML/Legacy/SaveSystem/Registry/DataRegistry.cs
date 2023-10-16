using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem.Registry
{
    internal class DataRegistry<K> : DataRegistry where K : IDataRegistryMember
    {
        public delegate bool OwnershipCheckerDelegate<T>(T value);

        public Dictionary<int,Func<K>> actorDataIds = new Dictionary<int, Func<K>>();

        public Dictionary<Type, int> modelTypeToIds = new Dictionary<Type, int>();

        private OwnershipCheckerDelegate<K> ownershipChecker;

        public DataRegistry() { }

        public DataRegistry(OwnershipCheckerDelegate<K> checker) : this()
        {
            ownershipChecker = checker;
        }

        public void AddCustomData<T>(int id, Func<K> creator)
        {
            actorDataIds.Add(id,()=>creator());
            modelTypeToIds.Add(typeof(T),id);
        }

        public void AddCustomData<T>(int id, Type t)
        {
            AddCustomData<T>(id, ()=>(K) Activator.CreateInstance(t));
        }

        public override bool BelongsToMe(object b)
        {
            return b is K k&&(ownershipChecker?.Invoke(k) ?? true);
        }

        public K GetDataForID(int id)
        {
            return actorDataIds[id]();
        }

        public override int GetIDForModel(Type model)
        {
            return modelTypeToIds[model];
        }

        public override bool IsModelRegistered(Type model)
        {
            return modelTypeToIds.ContainsKey(model);
        }
    }

    internal abstract class DataRegistry
    {
        public abstract bool BelongsToMe(object b);
        public abstract int GetIDForModel(Type model);
        public abstract bool IsModelRegistered(Type model);
    }

    internal interface IDataRegistryMember
    {
        Type GetModelType();
    }
}