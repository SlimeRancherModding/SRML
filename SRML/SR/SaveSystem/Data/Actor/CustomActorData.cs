using System;
using System.IO;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Registry;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
namespace SRML.SR.SaveSystem.Data.Actor
{
    internal abstract class CustomActorData<T> : CustomActorData where T : ActorModel
    {
        public virtual VanillaActorData GetVanillaDataPortion()
        {
            return this;
        }

        public override Type GetModelType()
        {
            return typeof(T);
        }


        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public sealed override void PushCustomModel(ActorModel model)
        {
            this.PushCustomModel((T) model);
        }

        public sealed override void PullCustomModel(ActorModel model)
        {
            this.PullCustomModel((T) model);
        }

        public sealed override void Load(Stream stream, bool skipPayloadEnd)
        {
            base.Load(stream,false);
            var reader = new BinaryReader(stream);
            LoadCustomData(reader);
            if(!skipPayloadEnd) ReadDataPayloadEnd(reader);
        }

        public sealed override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            WriteDataPayloadEnd(writer);
            WriteCustomData(writer);
        }

    }
    public abstract class CustomActorData : VanillaActorData, IDataRegistryMember
    {
        public abstract void PullCustomModel(ActorModel model);

        public abstract void PushCustomModel(ActorModel model);

        public abstract void WriteCustomData(BinaryWriter writer);

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract Type GetModelType();
    }
}