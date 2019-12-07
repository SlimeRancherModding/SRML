using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;
using Game = MonomiPark.SlimeRancher.Persist.GameV12;
namespace SRML.SR.SaveSystem.Data.Ammo
{

    public struct AmmoIdentifier
    {

        static readonly Dictionary<AmmoIdentifier, AmmoModel> _cache = new Dictionary<AmmoIdentifier, AmmoModel>();

        public AmmoType AmmoType;
        public long longIdentifier;
        public string stringIdentifier;
        public string custommodid;

        public AmmoIdentifier(AmmoType type, long id)
        {
            this.AmmoType = type;
            this.longIdentifier = id;
            this.stringIdentifier = "";
            this.custommodid = "";
        }

        public AmmoIdentifier(AmmoType type, string id)
        {
            this.AmmoType = type;
            this.longIdentifier = long.MaxValue;
            this.stringIdentifier = id;
            this.custommodid = "";
        }

        public AmmoIdentifier(AmmoType type, long longId, string stringId)
        {
            this.AmmoType = type;
            this.longIdentifier = longId;
            this.stringIdentifier = stringId;
            this.custommodid = "";
        }

        public AmmoIdentifier(AmmoType type, long longId, string stringId,string modid)
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
            return new AmmoIdentifier(ammotype, reader.ReadInt64(), reader.ReadString(), ammotype.IsCustom()?reader.ReadString():"");
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
                if (candidate.Value == ammoModel) return new AmmoIdentifier(Ammo.AmmoType.PLAYER, (long)candidate.Key);
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
                        return new AmmoIdentifier(AmmoType.LANDPLOT, (long)ammo.Key, candidate.Key);
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
        public static bool IsValidIdentifier(AmmoIdentifier identifier)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.GADGET:
                    return ModdedStringRegistry.IsValidString(identifier.stringIdentifier);
                case AmmoType.LANDPLOT:
                    return ModdedStringRegistry.IsValidString(identifier.stringIdentifier)&&Enum.IsDefined(typeof(SiloStorage.StorageType), (SiloStorage.StorageType)(int)identifier.longIdentifier);
                case AmmoType.PLAYER:
                    return Enum.IsDefined(typeof(PlayerState.AmmoMode),(PlayerState.AmmoMode)(int)identifier.longIdentifier);
            }
            return true;
        }

        public bool IsValid()
        {
            return IsValidIdentifier(this);
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

        static readonly Dictionary<AmmoIdentifier, global::Ammo> _ammoCache = new Dictionary<AmmoIdentifier, global::Ammo>();
        
        public static global::Ammo ResolveAmmo(AmmoIdentifier identifier)
        {
            if (_ammoCache.TryGetValue(identifier, out var model)) return model;
            var newModel = ResolveAmmoInternal(identifier);
            if (newModel != null) _ammoCache.Add(identifier, newModel);
            return newModel;
        }

        static global::Ammo ResolveAmmoInternal(AmmoIdentifier identifier)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.PLAYER:
                    return SceneContext.Instance.PlayerState.ammoDict[(PlayerState.AmmoMode)identifier.longIdentifier];
                default:
                    foreach(var v in Resources.FindObjectsOfTypeAll<Drone>())
                    {
                        if (v.ammo!=null&&AmmoIdentifier.GetIdentifier(v.ammo).Equals(identifier)) return v.ammo;
                    }
                    foreach(var v in Resources.FindObjectsOfTypeAll<SiloStorage>())
                    {
                        if (v.ammo != null && AmmoIdentifier.GetIdentifier(v.ammo).Equals(identifier)) return v.ammo;
                    }
                    break;
            }
            return null;
        }

        public static List<AmmoDataV02> ResolveToData(AmmoIdentifier identifier, Game game)
        {
            switch (identifier.AmmoType)
            {
                case AmmoType.PLAYER:
                    return game.player.ammo[(PlayerState.AmmoMode)identifier.longIdentifier];
                case AmmoType.GADGET:
                    return ModdedStringRegistry.IsValidString(identifier.stringIdentifier) ? (game.world.placedGadgets[identifier.stringIdentifier].drone?.drone?.ammo is AmmoDataV02 amdat ? new AmmoDataV02[] { amdat }.ToList() : game.world.placedGadgets[identifier.stringIdentifier].ammo) :null;
                case AmmoType.LANDPLOT:
                    return game.ranch.plots.FirstOrDefault((x) => x.id == identifier.stringIdentifier)?
                        .siloAmmo[(SiloStorage.StorageType)identifier.longIdentifier];
            }

            return null;
        }

        public static AmmoIdentifier GetIdentifier(List<AmmoDataV02> ammo, Game game)
        {
            foreach (var v in game.player.ammo)
            {
                if (ammo == v.Value) return new AmmoIdentifier(AmmoType.PLAYER, (long)v.Key);
            }

            foreach (var v in game.ranch.plots)
            {
                foreach (var ammoData in v.siloAmmo)
                {
                    if (ammoData.Value == ammo) return new AmmoIdentifier(AmmoType.LANDPLOT, (long)ammoData.Key, v.id);

                }
            }

            foreach (var v in game.world.placedGadgets)
            {
                if (v.Value.ammo == ammo) return new AmmoIdentifier(AmmoType.GADGET, v.Key);
            }
            return new AmmoIdentifier(AmmoType.NONE, 0);
        }

        public static bool TryGetIdentifier(List<AmmoDataV02> ammo, Game game, out AmmoIdentifier identifier)
        {
            identifier = GetIdentifier(ammo, game);
            return identifier.AmmoType != AmmoType.NONE;
        }

        internal static void ClearCache()
        {
            _cache.Clear();
            _ammoCache.Clear();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AmmoIdentifier))
            {
                return false;
            }

            var identifier = (AmmoIdentifier)obj;
            return AmmoType == identifier.AmmoType &&
                   longIdentifier == identifier.longIdentifier &&
                   (stringIdentifier ?? "") == (identifier.stringIdentifier ?? "");
        }

        public static bool IsModdedIdentifier(AmmoIdentifier id)
        {
            switch (id.AmmoType)
            {
                case AmmoType.PLAYER:
                    return ModdedIDRegistry.IsModdedID((PlayerState.AmmoMode)(int)id.longIdentifier);
                case AmmoType.LANDPLOT:
                    return ModdedIDRegistry.IsModdedID((SiloStorage.StorageType)(int)id.longIdentifier);
            }
            return id.AmmoType.IsCustom();
        }

        internal static SRMod GetModForIdentifier(AmmoIdentifier id)
        {
            switch (id.AmmoType)
            {
                case AmmoType.PLAYER:
                    return ModdedIDRegistry.ModForID((PlayerState.AmmoMode)(int)id.longIdentifier);
                case AmmoType.LANDPLOT:
                    return ModdedIDRegistry.ModForID((SiloStorage.StorageType)(int)id.longIdentifier);
            }
            return id.AmmoType.IsCustom() ? SRModLoader.GetMod(id.custommodid) : null;
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

    public enum AmmoType
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
