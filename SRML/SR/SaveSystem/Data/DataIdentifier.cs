using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data
{
    public struct DataIdentifier
    {
        public IdentifierType Type;
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
                {IdentifierType.ACTOR,typeof(ActorDataV09) },
                {IdentifierType.GADGET,typeof(PlacedGadgetV08) },
                {IdentifierType.LANDPLOT,typeof(LandPlotV08) },
                {IdentifierType.GORDO,typeof(GordoV01) },
                {IdentifierType.TREASUREPOD,typeof(TreasurePodV01) },
                {IdentifierType.EXCHANGEOFFER, typeof(ExchangeOfferV04) },
                {IdentifierType.RANCH, typeof(RanchV07) },
                {IdentifierType.WORLD, typeof(WorldV22) },
                {IdentifierType.PLAYER, typeof(PlayerV14) },
                {IdentifierType.APPEARANCES, typeof(AppearancesV01) },
                {IdentifierType.PEDIA, typeof(PediaV03) }
            };



        public static readonly Dictionary<Type,IdentifierType> PersistedDataSetsToIdentifiers = new Dictionary<IdentifierType, Type>()
        {
            {IdentifierType.ACTOR,typeof(VersionedPersistedDataSet<ActorDataV08>) },
                {IdentifierType.GADGET,typeof(VersionedPersistedDataSet<PlacedGadgetV07>) },
                {IdentifierType.LANDPLOT,typeof(VersionedPersistedDataSet<LandPlotV07>) },
                {IdentifierType.GORDO,typeof(GordoV01) },
                {IdentifierType.TREASUREPOD,typeof(TreasurePodV01) },
                {IdentifierType.EXCHANGEOFFER, typeof(VersionedPersistedDataSet<ExchangeOfferV03>) },
                {IdentifierType.RANCH, typeof(VersionedPersistedDataSet<RanchV06>) },
                {IdentifierType.WORLD, typeof(VersionedPersistedDataSet<WorldV21>) },
                {IdentifierType.PLAYER,typeof(VersionedPersistedDataSet<PlayerV13>) },
                {IdentifierType.APPEARANCES,typeof(AppearancesV01) },
                {IdentifierType.PEDIA,typeof(VersionedPersistedDataSet<PediaV02>) }
        }.Invert();


        public static KeyValuePair<DataIdentifier, object> Convert<T>(KeyValuePair<string, T> obj) => new KeyValuePair<DataIdentifier, object>(new DataIdentifier() { Type = GetIdentifierType(obj.Value.GetType()), stringID = obj.Key }, obj.Value);
        public static IdentifierType GetIdentifierType(Type type) => PersistedDataSetsToIdentifiers.FirstOrDefault(x => x.Key.IsAssignableFrom(type)).Value;

        public static DataIdentifier GetIdentifier(GameV12 game, object obj)
        {
            DataIdentifier CreateIdentifier(string stringId, long longId) => new DataIdentifier() { Type = GetIdentifierType(obj.GetType()), longID = longId, stringID = stringId };
            DataIdentifier CreateIdentifierS(string stringId) => CreateIdentifier(stringId, 0);
            DataIdentifier CreateIdentifierL(long longId) => CreateIdentifier(null, longId);
            switch (obj)
            {
                case ActorDataV09 actor:
                    return CreateIdentifierL(actor.actorId);
                case PlacedGadgetV08 gadget:
                    return CreateIdentifierS(game.world.placedGadgets.GetKeyOfValue(gadget));
                case GordoV01 gordo:
                    return CreateIdentifierS(game.world.gordos.GetKeyOfValue(gordo));
                case TreasurePodV01 pod:
                    return CreateIdentifierS(game.world.treasurePods.GetKeyOfValue(pod));
                case ExchangeOfferV04 offer:
                    return CreateIdentifierL((long)game.world.offers.GetKeyOfValue(offer));
                case LandPlotV08 plot:
                    return CreateIdentifierS(plot.id);
                case WorldV22 world:
                    return CreateIdentifierL(0);
                case RanchV07 ranch:
                    return CreateIdentifierL(0);
                case PlayerV14 player:
                    return CreateIdentifierL(0);
                case AppearancesV01 _:
                    return CreateIdentifierL(0);
                case PediaV03 _:
                    return CreateIdentifierL(0);

            }
            throw new NotImplementedException();
        }

        public static GameObject ResolveIdentifierToGameObject(GameModel model, DataIdentifier identifier)
        {
            switch (identifier.Type)
            {
                case IdentifierType.ACTOR:
                    return model.actors.Get(identifier.longID).transform?.gameObject;
                case IdentifierType.GADGET:
                    return model.gadgetSites.Get(identifier.stringID).transform?.gameObject;
                case IdentifierType.LANDPLOT:
                    return model.landPlots.Get(identifier.stringID).gameObj;
                case IdentifierType.GORDO:
                    return model.gordos.Get(identifier.stringID).gameObj;
                case IdentifierType.TREASUREPOD:
                    return model.pods.Get(identifier.stringID).gameObj;
               
                case IdentifierType.PLAYER:

                    return SceneContext.Instance.Player;

            }
            return null;
        }

        public static object ResolveIdentifier(GameV12 game, DataIdentifier identifier)
        {
            switch (identifier.Type)
            {
                case IdentifierType.ACTOR:
                    return game.actors.FirstOrDefault(x => x.actorId == identifier.longID);

                case IdentifierType.GADGET:
                    return game.world.placedGadgets.Get(identifier.stringID);
                case IdentifierType.LANDPLOT:
                    return game.ranch.plots.FirstOrDefault(x => identifier.stringID == x.id);
                case IdentifierType.GORDO:
                    return game.world.gordos.Get(identifier.stringID);
                case IdentifierType.TREASUREPOD:
                    return game.world.treasurePods.Get(identifier.stringID);
                case IdentifierType.EXCHANGEOFFER:
                    return game.world.offers.Get((ExchangeDirector.OfferType)identifier.Type);
                case IdentifierType.WORLD:
                    return game.world;
                case IdentifierType.RANCH:
                    return game.ranch;
                case IdentifierType.PLAYER:
                    return game.player;
                case IdentifierType.APPEARANCES:
                    return game.appearances;
                case IdentifierType.PEDIA:
                    return game.pedia;
            }
            throw new NotImplementedException();
        }

        public static bool TryResolveIdentifier(GameV12 game, DataIdentifier identifier, out object obj)
        {
            return (obj = ResolveIdentifier(game, identifier)) != null;
        }

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

        public override string ToString()
        {
            return $"(Type: {Type}, LongID: {longID} StringID: {stringID}";
        }

        public static bool operator ==(DataIdentifier me, DataIdentifier other) => me.Equals(other);
        public static bool operator !=(DataIdentifier me, DataIdentifier other) => !me.Equals(other);

    }
    public enum IdentifierType { 

        NONE,
        ACTOR,
        GADGET,
        LANDPLOT,
        GORDO,
        TREASUREPOD,
        EXCHANGEOFFER,
        WORLD,
        RANCH,
        PLAYER,
        APPEARANCES,
        PEDIA
    }
}
