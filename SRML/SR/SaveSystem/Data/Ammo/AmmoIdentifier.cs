using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Ammo
{

    internal struct AmmoIdentifier
    {

        static readonly Dictionary<AmmoIdentifier, AmmoModel> _cache = new Dictionary<AmmoIdentifier, AmmoModel>();

        public AmmoType AmmoType;
        public ulong longIdentifier;
        public string stringIdentifier;
        public string custommodid;

        public AmmoIdentifier(AmmoType type, ulong id)
        {
            this.AmmoType = type;
            this.longIdentifier = id;
            this.stringIdentifier = "";
            this.custommodid = "";
        }

        public AmmoIdentifier(AmmoType type, string id)
        {
            this.AmmoType = type;
            this.longIdentifier = ulong.MaxValue;
            this.stringIdentifier = id;
            this.custommodid = "";
        }

        public AmmoIdentifier(AmmoType type, ulong longId, string stringId)
        {
            this.AmmoType = type;
            this.longIdentifier = longId;
            this.stringIdentifier = stringId;
            this.custommodid = "";
        }

        public AmmoIdentifier(AmmoType type, ulong longId, string stringId,string modid)
        {
            this.AmmoType = type;
            this.longIdentifier = longId;
            this.stringIdentifier = stringId;
            this.custommodid = modid;
        }
        public static void Write(AmmoIdentifier identifier, BinaryWriter writer)
        {
            writer.Write((int)identifier.AmmoType);
            writer.Write(identifier.longIdentifier);
            writer.Write(identifier.stringIdentifier);
            if (identifier.AmmoType.IsCustom()) writer.Write(identifier.custommodid); 
        }

        public static AmmoIdentifier Read(BinaryReader reader)
        {
            var ammotype = (Ammo.AmmoType)reader.ReadInt32();
            return new AmmoIdentifier(ammotype, reader.ReadUInt64(), reader.ReadString(), ammotype.IsCustom()?reader.ReadString():"");
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
            if (newIdentifier.AmmoType != AmmoType.NONE) _cache[newIdentifier] = model;
            return newIdentifier;
        }

        static AmmoIdentifier GetIdentifierInternal(AmmoModel ammoModel) // warning, this is a reference based check
        {
            foreach (var candidate in SceneContext.Instance.GameModel.player.ammoDict)
            {
                if (candidate.Value == ammoModel) return new AmmoIdentifier(Ammo.AmmoType.PLAYER, (ulong)candidate.Key);
            }

            foreach (var candidate in SceneContext.Instance.GameModel.gadgetSites.Where((x) => x.Value.HasAttached()))
            {
                var potentialKey = new AmmoIdentifier(AmmoType.GADGET, candidate.Key);
                if (candidate.Value.attached is DroneModel drone)
                {
                    if (drone.ammo == ammoModel)
                        return potentialKey;

                }

                if (candidate.Value.attached is WarpDepotModel depot)
                {
                    if (depot.ammo == ammoModel) return potentialKey;

                }

            }

            foreach (var candidate in SceneContext.Instance.GameModel.landPlots)
            {
                foreach (var ammo in candidate.Value.siloAmmo) {
                    if (ammo.Value == ammoModel)
                        return new AmmoIdentifier(AmmoType.LANDPLOT, (ulong)ammo.Key, candidate.Key);
                }
            }

            return new AmmoIdentifier(AmmoType.NONE, 0);
        }

        public AmmoModel ResolveModel()
        {
            return ResolveModel(this);
        }

        public static AmmoModel ResolveModel(AmmoIdentifier identifier)
        {
            if (_cache.TryGetValue(identifier, out var model)) return model;
            var newModel = ResolveModelInternal(identifier);
            if (newModel != null) _cache.Add(identifier, newModel);
            return newModel;
        }

        static AmmoModel ResolveModelInternal(AmmoIdentifier identifier)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.PLAYER:
                    return SceneContext.Instance.GameModel.player.ammoDict
                        [(PlayerState.AmmoMode)identifier.longIdentifier];

                case AmmoType.GADGET:
                    var gadget = SceneContext.Instance.GameModel.gadgetSites[identifier.stringIdentifier].attached;
                    if (gadget is DroneModel drone) return drone.ammo;
                    if (gadget is WarpDepotModel depot) return depot.ammo;
                    return null;
                case AmmoType.LANDPLOT:
                    var plot = SceneContext.Instance.GameModel.landPlots
                        [identifier.stringIdentifier];

                    var type = (SiloStorage.StorageType)(uint)identifier.longIdentifier;
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
                    return SceneContext.Instance.PlayerState.ammoDict[(PlayerState.AmmoMode)identifier.longIdentifier];
                default:
                    throw new NotImplementedException();
            }
        }

        public static List<AmmoDataV02> ResolveToData(AmmoIdentifier identifier, GameV11 game)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.PLAYER:
                    return game.player.ammo[(PlayerState.AmmoMode)identifier.longIdentifier];
                case AmmoType.GADGET:
                    return game.world.placedGadgets[identifier.stringIdentifier].ammo;
                case AmmoType.LANDPLOT:
                    return game.ranch.plots.First((x) => x.id == identifier.stringIdentifier)
                        .siloAmmo[(SiloStorage.StorageType)identifier.longIdentifier];
            }

            return null;
        }

        public static AmmoIdentifier GetIdentifier(List<AmmoDataV02> ammo, GameV11 game)
        {
            foreach (var v in game.player.ammo)
            {
                if (ammo == v.Value) return new AmmoIdentifier(AmmoType.PLAYER, (ulong)v.Key);
            }

            foreach (var v in game.ranch.plots)
            {
                foreach (var ammoData in v.siloAmmo)
                {
                    if (ammoData.Value == ammo) return new AmmoIdentifier(AmmoType.LANDPLOT, (ulong)ammoData.Key, v.id);

                }
            }

            foreach (var v in game.world.placedGadgets)
            {
                if (v.Value.ammo == ammo) return new AmmoIdentifier(AmmoType.GADGET, v.Key);
            }
            return new AmmoIdentifier(AmmoType.NONE, 0);
        }

        public static bool TryGetIdentifier(List<AmmoDataV02> ammo, GameV11 game, out AmmoIdentifier identifier)
        {
            identifier = GetIdentifier(ammo, game);
            return identifier.AmmoType != AmmoType.NONE;
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
                   longIdentifier == identifier.longIdentifier;
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
            hashCode = hashCode * -1521134295 + longIdentifier.GetHashCode();
            return hashCode;
        }
    }

    internal enum AmmoType
    {
        NONE,
        PLAYER,
        LANDPLOT,
        GADGET,
        CUSTOM = -2147483648

    }

    internal static class AmmoTypeExtensions
    {

        const int flag = -2147483648;
        public static bool IsCustom(this AmmoType type)
        {
            return ((int)type & flag) != 0;

        }

        public static uint GetNonCustomPortion(this AmmoType type)
        {
            return (uint)((int)type ^ flag);
        }

        public static AmmoType Customify(this AmmoType original)
        {
            return (AmmoType)((int)original & flag);
        }
    }
}
