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
    internal class ExtendedDataTree : VersionedSerializable
    {
        public IdentifierType idType;
        public long longIdentifier;
        public string stringIdentifier="";
        public CompoundDataPiece dataPiece;

        public override int LatestVersion => 0;

        public override void Read(BinaryReader reader)
        {
            bool isOld = false;
            int number =  reader.ReadInt32();
            if (number < 3)
            {
                isOld = true; number += 3;
            }
            idType = (IdentifierType)number;
            if(!isOld) base.Read(reader);
            longIdentifier = reader.ReadInt64();
            stringIdentifier = isOld ? "" : reader.ReadString();
            dataPiece = DataPiece.Deserialize(reader) as CompoundDataPiece;
            if (dataPiece == null) throw new Exception("Invalid top level data piece!");
        }

        public override void Write(BinaryWriter writer)
        {
            
            writer.Write((int)idType);
            base.Write(writer);
            writer.Write(longIdentifier);
            writer.Write(stringIdentifier);
            DataPiece.Serialize(writer, dataPiece);
        }

        static ExtendedDataTree()
        {
            EnumTranslator.RegisterEnumFixer<ExtendedDataTree>((translator, mode, piece) => 
            {
                if(piece.dataPiece!=null) translator.FixEnumValues(mode, piece.dataPiece);
            });
        }

        internal enum IdentifierType
        {
            ACTOR=3,
            GADGET=4,
            LANDPLOT=5
        }
    }

    

    
}
