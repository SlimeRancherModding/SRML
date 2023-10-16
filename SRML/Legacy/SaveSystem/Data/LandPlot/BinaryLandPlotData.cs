using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem.Data.LandPlot
{
    internal class BinaryLandPlotData<T> : CustomLandPlotData<T> where T : LandPlotModel, ISerializableModel
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
