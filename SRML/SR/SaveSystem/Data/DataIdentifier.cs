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

        public static DataIdentifier GetActorIdentifier(long actorId)
        {
            return new DataIdentifier() { Type = IdentifierType.ACTOR , longID=actorId};
        }
        public static void Write(BinaryWriter writer,DataIdentifier id)
        {
            writer.Write((int) id.Type);
            writer.Write(id.longID);
            writer.Write(id.stringID ?? "");
        }

        public static DataIdentifier Read(BinaryReader reader)
        {
            var type = (IdentifierType)reader.ReadInt32();
            var longID = reader.ReadInt64();
            var stringID = reader.ReadString();
            if (stringID == "") stringID = null;
            return new DataIdentifier
            {
                Type = type,
                longID = longID,    
                stringID = stringID
            };
        }

        public override bool Equals(object obj)
        {
            return obj is DataIdentifier identifier &&
                   Type == identifier.Type &&
                   longID == identifier.longID &&
                   (stringID ?? "") == (identifier.stringID ?? "");
        }

        public override int GetHashCode()
        {
            var hashCode = -877822480;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + longID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(stringID??"");
            return hashCode;
        }

        public static bool operator ==(DataIdentifier me, DataIdentifier other) => me.Equals(other);
        public static bool operator !=(DataIdentifier me, DataIdentifier other) => !me.Equals(other); 
    }
    public enum IdentifierType
    {
        NONE,
        ACTOR,
        GADGET,
        LANDPLOT
    }
}
