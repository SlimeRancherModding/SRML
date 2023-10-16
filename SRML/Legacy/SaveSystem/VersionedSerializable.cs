using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    public abstract class VersionedSerializable : IVersionedSerializable
    {
        public abstract int LatestVersion { get; }

        public int Version { get; protected set; }

        public virtual void Read(BinaryReader reader)
        {
            Version = reader.ReadInt32();
            ReadData(reader);
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(LatestVersion);
            WriteData(writer);
        }

        public abstract void ReadData(BinaryReader reader);
        public abstract void WriteData(BinaryWriter writer);
    }
}
