using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data
{
    internal struct DataIdentifier
    {
        public IdentifierType Type;
        public long longID;
        public string stringID;


        public static void Write(BinaryWriter writer,DataIdentifier id)
        {
            writer.Write((int) id.Type);
            writer.Write(id.longID);
            writer.Write(id.stringID);
        }

        public static DataIdentifier Read(BinaryReader reader)
        {
            return new DataIdentifier
            {
                Type = (IdentifierType) reader.ReadInt32(),
                longID = reader.ReadInt64(),
                stringID = reader.ReadString()
            };
        }
    }
    public enum IdentifierType
    {
        NONE,
        ACTOR,
        GADGET,
        LANDPLOT
    }
}
