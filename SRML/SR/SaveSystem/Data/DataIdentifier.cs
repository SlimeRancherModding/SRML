using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;

namespace SRML.SR.SaveSystem.Data
{
    internal struct DataIdentifier
    {
        public IdentifierType Type;
        public long longID;
        public string stringID;

        public static readonly Dictionary<IdentifierType, Type> IdentifierTypeToData =
            new Dictionary<IdentifierType, Type>()
            {
                {IdentifierType.ACTOR,typeof(ActorDataV09) },
                {IdentifierType.GADGET,typeof(PlacedGadgetV08) },
                {IdentifierType.LANDPLOT,typeof(LandPlotV08) }
            };

        public static void Write(BinaryWriter writer,DataIdentifier id)
        {
            writer.Write((int) id.Type);
            writer.Write(id.longID);
            writer.Write(id.stringID ?? "");
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
