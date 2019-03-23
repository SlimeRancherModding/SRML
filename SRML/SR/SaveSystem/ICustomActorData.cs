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
    public interface ICustomActorData<T> where T:ActorModel
    {
        void PullCustomModel(T model);
        void PushCustomModel(T model);

        void WriteCustomData(BinaryWriter writer);
        void LoadCustomData(BinaryReader reader);

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
    }

    public class VanillaActorData : ActorDataV07 { } // this is so we can easily replace which actordata version we're extending from

    public abstract class CustomActorData<T> : VanillaActorData, ICustomActorData<T> where T : ActorModel
    {
        public VanillaActorData GetVanillaDataPortion()
        {
            return this;
        }

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public abstract void WriteCustomData(BinaryWriter writer);

        public override void Load(Stream stream, bool skipPayloadEnd)
        {
            base.Load(stream,false);
            var reader = new BinaryReader(stream);
            LoadCustomData(reader);
            ReadDataPayloadEnd(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            WriteDataPayloadEnd(writer);
            WriteCustomData(writer);
        }

    }
}
