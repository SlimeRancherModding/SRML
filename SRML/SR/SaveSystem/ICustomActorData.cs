using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;

namespace SRML.SR.SaveSystem
{
    public interface ICustomActorData<T> : Persistable where T:ActorModel
    {
        void PullCustomModel(T model);
        void PushCustomModel(T model);

        void WriteCustomData(BinaryWriter writer);
        void LoadCustomData(BinaryReader reader);

        Type GetModelType();

        VanillaActorData GetVanillaDataPortion();
    }

    internal class ActorDataWrapper<T> : ICustomActorData<ActorModel> where T : ActorModel
    {
        public ActorDataWrapper(ICustomActorData<T> wrapped)
        {
            wrappedObject = wrapped;
        }
        public ICustomActorData<T> wrappedObject;

        public void PullCustomModel(ActorModel model)
        {
            wrappedObject.PushCustomModel((T)model);
        }

        public void PushCustomModel(ActorModel model)
        {
            wrappedObject.PushCustomModel((T)model);
        }

        public void WriteCustomData(BinaryWriter writer)
        {
            wrappedObject.WriteCustomData(writer);
        }

        public void LoadCustomData(BinaryReader reader)
        {
            wrappedObject.LoadCustomData(reader);
        }

        public VanillaActorData GetVanillaDataPortion()
        {
            return wrappedObject.GetVanillaDataPortion();
        }

        public void Load(Stream stream)
        {
            wrappedObject.Load(stream);
        }

        public long Write(Stream stream)
        {
            return wrappedObject.Write(stream);
        }

        public Type GetModelType()
        {
            return wrappedObject.GetModelType();
        }
    }

    public class VanillaActorData : ActorDataV07 { } // this is so we can easily replace which actordata version we're extending from
}
