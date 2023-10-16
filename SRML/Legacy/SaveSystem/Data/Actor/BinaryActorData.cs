using System.IO;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem.Data.Actor
{
    class BinaryActorData<T> : CustomActorData<T> where T : ActorModel, ISerializableModel
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

        public override void LoadCustomData(BinaryReader reader)
        {
            int byteLength = reader.ReadInt32();
            data = reader.ReadBytes(byteLength);
        }

        public override void WriteCustomData(BinaryWriter writer)
        {
            writer.Write(data.Length);
            writer.Write(data);
        }
    }
}