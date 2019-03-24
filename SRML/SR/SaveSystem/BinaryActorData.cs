using System.IO;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem
{
    public class BinaryActorData<T> : ICustomActorData<T> where T : ActorModel, ISerializableModel
    {
        private byte[] data;

        public void PullCustomModel(T model)
        {
            using (var stream = new MemoryStream())
            {
                model.WriteData(new BinaryWriter(stream));
                data = stream.GetBuffer();
            }
        }

        public void PushCustomModel(T model)
        {
            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                model.LoadData(reader);
            }
        }

        public void LoadCustomData(BinaryReader reader)
        {
            int byteLength = reader.ReadInt32();
            data = reader.ReadBytes(byteLength);
        }

        public void WriteCustomData(BinaryWriter writer)
        {
            writer.Write(data.Length);
            writer.Write(data);
        }
    }
}