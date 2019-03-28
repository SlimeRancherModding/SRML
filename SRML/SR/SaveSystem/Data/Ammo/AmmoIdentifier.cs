using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.SaveSystem.Data.Ammo
{

    internal struct AmmoIdentifier
    {

        static readonly Dictionary<AmmoIdentifier,AmmoModel> _cache = new Dictionary<AmmoIdentifier, AmmoModel>();

        public AmmoType AmmoType;
        public ulong Identifier;

        public AmmoIdentifier(AmmoType type, ulong id)
        {
            this.AmmoType = type;
            this.Identifier = id;
        }

        public static void Write(AmmoIdentifier identifier, BinaryWriter writer)
        {
            writer.Write((int)identifier.AmmoType);
            writer.Write(identifier.Identifier);
        }

        public static AmmoIdentifier Read(BinaryReader reader)
        {
            return new AmmoIdentifier((Ammo.AmmoType) reader.ReadInt32(), reader.ReadUInt64());
        }

        public static AmmoIdentifier GetIdentifier(global::Ammo ammo)
        {
            return GetIdentifier(ammo.ammoModel);
        }

        public static AmmoIdentifier GetIdentifier(AmmoModel model)
        {
            var foundKey = _cache.FirstOrDefault((x) => x.Value == model).Key;
            if (foundKey.AmmoType != AmmoType.NONE) return foundKey;

            var newIdentifier = GetIdentifierInternal(model);
            if(newIdentifier.AmmoType!=AmmoType.NONE) _cache[newIdentifier] = model;
            return newIdentifier;
        }

        static AmmoIdentifier GetIdentifierInternal(AmmoModel ammoModel) // warning, this is a reference based check
        {
            foreach (var candidate in SceneContext.Instance.GameModel.player.ammoDict)
            {
                if (candidate.Value == ammoModel) return new AmmoIdentifier(Ammo.AmmoType.PLAYER, (ulong) candidate.Key);
            }

            foreach (var candidate in SceneContext.Instance.GameModel.gadgetSites.Where((x)=>x.Value.HasAttached()))
            {
                var potentialKey = new AmmoIdentifier(AmmoType.GADGET, ((ulong)candidate.Key.GetHashCode())<<32);
                if (candidate.Value.attached is DroneModel drone)
                {
                    if (drone.ammo == ammoModel)
                        return potentialKey;

                }

                if (candidate.Value.attached is WarpDepotModel depot)
                {
                    if (depot.ammo==ammoModel) return potentialKey;
                    
                }

            }

            foreach (var candidate in SceneContext.Instance.GameModel.landPlots)
            {
                foreach (var ammo in candidate.Value.siloAmmo) { 
                    if (ammo.Value == ammoModel)
                        return new AmmoIdentifier(AmmoType.LANDPLOT, (((ulong) candidate.Key.GetHashCode()) << 32)|(uint)ammo.Key);
                }
            }

            return new AmmoIdentifier(AmmoType.NONE,0);
        }

        public AmmoModel ResolveModel()
        {
            return ResolveModel(this);
        }

        public static AmmoModel ResolveModel(AmmoIdentifier identifier)
        {
            if (_cache.TryGetValue(identifier, out var model)) return model;
            var newModel = ResolveModelInternal(identifier);
            if(newModel!=null) _cache.Add(identifier,newModel);
            return newModel;
        }

        static AmmoModel ResolveModelInternal(AmmoIdentifier identifier)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.PLAYER:
                    return SceneContext.Instance.GameModel.player.ammoDict
                        [(PlayerState.AmmoMode) identifier.Identifier];

                case AmmoType.GADGET:
                    var gadget =  SceneContext.Instance.GameModel.gadgetSites.First((x) =>
                        (ulong)x.Key.GetHashCode() == (identifier.Identifier >> 32)).Value.attached;
                    if (gadget is DroneModel drone) return drone.ammo;
                    if (gadget is WarpDepotModel depot) return depot.ammo;
                    return null;
                case AmmoType.LANDPLOT:
                    var plot = SceneContext.Instance.GameModel.landPlots
                        .First((x) => (ulong) x.Key.GetHashCode() == (identifier.Identifier >> 32)).Value;
                    var type = (SiloStorage.StorageType)(uint)identifier.Identifier;
                    return plot.siloAmmo[type];
                default:
                    throw new NotImplementedException();
            }
        }

        public static global::Ammo ResolveAmmo(AmmoIdentifier identifier)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.PLAYER:
                   return SceneContext.Instance.PlayerState.ammoDict[(PlayerState.AmmoMode)identifier.Identifier];
                default:
                    throw new NotImplementedException();
            }
        }

        internal static void ClearCache()
        {
            _cache.Clear();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AmmoIdentifier))
            {
                return false;
            }

            var identifier = (AmmoIdentifier)obj;
            return AmmoType == identifier.AmmoType &&
                   Identifier == identifier.Identifier;
        }

        public static bool TryGetIdentifier(AmmoModel model, out AmmoIdentifier identifier)
        {
            identifier = GetIdentifier(model);
            return identifier.AmmoType != AmmoType.NONE;
        }

        public static bool TryGetIdentifier(global::Ammo model, out AmmoIdentifier identifier)
        {
            return TryGetIdentifier(model.ammoModel, out identifier);
        }

        public override int GetHashCode()
        {
            var hashCode = 1655372237;
            hashCode = hashCode * -1521134295 + AmmoType.GetHashCode();
            hashCode = hashCode * -1521134295 + Identifier.GetHashCode();
            return hashCode;
        }
    }

    internal enum AmmoType
    {
        NONE,
        PLAYER,
        LANDPLOT,
        GADGET
    }
}
