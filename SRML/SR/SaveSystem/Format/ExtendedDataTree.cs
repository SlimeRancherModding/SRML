using SRML.SR.SaveSystem.Data;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Format
{
    internal class ExtendedDataTree
    {
        public IdentifierType idType;
        public long identifier;
        public CompoundDataPiece dataPiece;

        public void Read(BinaryReader reader)
        {
            idType = (IdentifierType) reader.ReadInt32();
            identifier = reader.ReadInt64();
            dataPiece = DataPiece.Deserialize(reader) as CompoundDataPiece;
            if (dataPiece == null) throw new Exception("Invalid top level data piece!");
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)idType);
            writer.Write(identifier);
            dataPiece.key = "root";
            DataPiece.Serialize(writer, dataPiece);
        }

        internal enum IdentifierType
        {
            ACTOR,
            GADGET
        }
    }

    

    
}
