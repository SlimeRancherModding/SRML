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
    public interface ICustomActorData<T>  where T:ActorModel
    {
        void PullCustomModel(T model);
        void PushCustomModel(T model);

        void WriteData(BinaryWriter writer);
        void LoadData(BinaryReader reader);
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

        public void WriteData(BinaryWriter writer)
        {
            wrappedObject.WriteData(writer);
        }

        public void LoadData(BinaryReader reader)
        {
            wrappedObject.LoadData(reader);
        }
    }

    public abstract class CustomActorData<T> : ActorDataV07, ICustomActorData<T> where T : ActorModel
    {
        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

    }

    public class BinaryActorData<T> : CustomActorData<T> where T : ActorModel, ISerializableModel
    {
        private byte[] data;

        public override void PullCustomModel(T model)
        {
            using (var stream = new MemoryStream())
            {
                model.WriteData(new BinaryWriter(stream));
                data = stream.GetBuffer();
            }
        }

        public override void PushCustomModel(T model)
        {
            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                model.LoadData(reader);
            }
        }

        public override void LoadData(BinaryReader reader)
        {
            base.LoadData(reader);
            int byteLength = reader.ReadInt32();
            data = reader.ReadBytes(byteLength);
        }

        public override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            writer.Write(data.Length);
            writer.Write(data);
        }
    }

}
