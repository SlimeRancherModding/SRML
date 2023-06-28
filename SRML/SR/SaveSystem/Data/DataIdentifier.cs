﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Format;

namespace SRML.SR.SaveSystem.Data
{
    internal struct DataIdentifier
    {
        public IdentifierType Type;
        public ExtendedDataTree.IdentifierType DataType;
        public long longID;
        public string stringID;

        public DataIdentifier TranslateWithEnum(EnumTranslator translator, EnumTranslator.TranslationMode mode)
        {
            if (Type != IdentifierType.EXCHANGEOFFER) return this;
            return new DataIdentifier() { Type = Type, longID =(int)translator.TranslateEnum(mode, (ExchangeDirector.OfferType)(int)longID), stringID = stringID };
        }

        public static readonly Dictionary<IdentifierType, Type> IdentifierTypeToData =
            new Dictionary<IdentifierType, Type>()
            {
                { IdentifierType.ACTOR, typeof(ActorDataV09) },
                { IdentifierType.GADGET, typeof(PlacedGadgetV08) },
                { IdentifierType.LANDPLOT, typeof(LandPlotV08) },
                { IdentifierType.GORDO, typeof(GordoV01) },
                { IdentifierType.TREASUREPOD, typeof(TreasurePodV01) },
                { IdentifierType.EXCHANGEOFFER, typeof(ExchangeOfferV04) }
            };

        public static DataIdentifier GetActorIdentifier(long actorId) => new DataIdentifier() 
        { 
            Type = IdentifierType.ACTOR,
            DataType = ExtendedDataTree.IdentifierType.ACTOR,
            longID = actorId
        };

        public static DataIdentifier GetGameDataIdentifier(string dataId) => new DataIdentifier()
        {
            Type = IdentifierType.GAMEDATA,
            DataType = ExtendedDataTree.IdentifierType.GAMEDATA,
            stringID = dataId
        };

        public static DataIdentifier GetGadgetIdentifier(string gadgetId) => new DataIdentifier()
        {
            Type = IdentifierType.GADGET,
            DataType = ExtendedDataTree.IdentifierType.GADGET,
            stringID = gadgetId
        };

        public static DataIdentifier GetLandPlotIdentifier(string landplotId) => new DataIdentifier()
        {
            Type = IdentifierType.LANDPLOT,
            DataType = ExtendedDataTree.IdentifierType.LANDPLOT,
            stringID = landplotId
        };

        public static string GetGadgetSaveId(GadgetSiteModel site) => $"{site.id}.{site.attached.ident}";

        public static string GetLandPlotSaveId(LandPlotModel plot) => $"{plot.gameObj.GetComponent<LandPlotLocation>().id}.{plot.typeId}";

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

        public static bool operator == (DataIdentifier me, DataIdentifier other) => me.Equals(other);
        public static bool operator != (DataIdentifier me, DataIdentifier other) => !me.Equals(other); 
    }
    public enum IdentifierType
    {
        NONE,
        ACTOR,
        GADGET,
        LANDPLOT,
        GORDO,
        TREASUREPOD,
        EXCHANGEOFFER,
        GAMEDATA,
    }
}
