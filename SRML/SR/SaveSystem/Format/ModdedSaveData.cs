using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.Utils;
namespace SRML.SR.SaveSystem.Format
{
    class ModdedSaveData
    {
        public int version;
        public long dataSegmentStartOffset;
        public Dictionary<string, long> modToOffset = new Dictionary<string, long>();
        public List<ModDataSegment> segments = new List<ModDataSegment>();
        public void ReadHeader(BinaryReader reader)
        {
            version = reader.ReadInt32();
            dataSegmentStartOffset = reader.ReadInt64();
            modToOffset.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                modToOffset.Add(reader.ReadString(),reader.ReadInt64());
            }
            
        }

        public void WriteHeader(BinaryWriter writer)
        {
            long start = writer.BaseStream.Position;
            writer.Write(version);
            writer.Write(dataSegmentStartOffset);

            writer.Write(modToOffset.Count);
            foreach (var v in modToOffset)
            {
                writer.Write(v.Key);
                writer.Write(v.Value);
            }

            long current = writer.BaseStream.Position;
            writer.BaseStream.Seek(start + sizeof(Int32), SeekOrigin.Begin);
            writer.Write(current-start);
            writer.BaseStream.Seek(current,SeekOrigin.Begin);
        }

    }
}
