using System;
using System.IO;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem
{
    public abstract class CustomActorData<T> : VanillaActorData, ICustomActorData<T> where T : ActorModel
    {
        public VanillaActorData GetVanillaDataPortion()
        {
            return this;
        }

        public Type GetModelType()
        {
            return typeof(T);
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