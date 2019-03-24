using System;
using System.IO;
using MonomiPark.SlimeRancher.DataModel;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
namespace SRML.SR.SaveSystem
{
    internal abstract class CustomActorData<T> : VanillaActorData where T : ActorModel
    {
        public virtual VanillaActorData GetVanillaDataPortion()
        {
            return this;
        }

        public virtual Type GetModelType()
        {
            return typeof(T);
        }

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public abstract void WriteCustomData(BinaryWriter writer);

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
}