using SRML.API.Identifiable;
using SRML.API.Identifiable.Slime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SRML.SRModLoader;

namespace SRML.SR
{
    public static class LookupRegistry
    {
        internal static HashSet<GadgetDefinition> gadgetEntriesToPatch = new HashSet<GadgetDefinition>();

        internal static HashSet<UpgradeDefinition> upgradeEntriesToPatch =
            new HashSet<UpgradeDefinition>();

        internal static HashSet<GameObject> landPlotsToPatch = new HashSet<GameObject>();

        /// <summary>
        /// Register an Identifiable Prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="b">The prefab to register.</param>
        public static void RegisterIdentifiablePrefab(GameObject b) => RegisterIdentifiablePrefab(b.GetComponent<Identifiable>());

        /// <summary>
        /// Register an Identifiable Prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="b">The <see cref="Identifiable"/> belonging to the prefab to register.</param>
        public static void RegisterIdentifiablePrefab(Identifiable b) => 
            API.Identifiable.IdentifiablePrefabRegistry.Instance.Register(b);

        /// <summary>
        /// Register <paramref name="entry"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="entry"></param>
        public static void RegisterVacEntry(VacItemDefinition entry) => VacItemRegistry.Instance.Register(entry);
        /// <summary>
        /// Register a landplot prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="prefab">The prefab to register</param>
        public static void RegisterLandPlot(GameObject prefab)
        {
            if (prefab.GetComponentInChildren<LandPlot>(true).typeId == LandPlot.Id.NONE)
                throw new InvalidOperationException("Attempting to register a LandPlot with id NONE. This is not allowed.");

            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    landPlotsToPatch.Add(prefab);
                    break;
                default:
                    GameContext.Instance.LookupDirector.plotPrefabs.AddAndRemoveWhere(prefab,(x,y)=> x.GetComponentInChildren<LandPlot>(true).typeId == y.GetComponentInChildren<LandPlot>(true).typeId);
                    GameContext.Instance.LookupDirector.plotPrefabDict[prefab.GetComponentInChildren<LandPlot>(true).typeId] = prefab;
                    break;
            }
        }
        /// <summary>
        /// Register a gadget entry to the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="entry"></param>
        public static void RegisterGadget(GadgetDefinition entry)
        {
            if (entry.id == Gadget.Id.NONE)
                throw new InvalidOperationException("Attempting to register a GadgetDefinition with id NONE. This is not allowed.");

            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    gadgetEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.gadgetDefinitions.AddAndRemoveWhere(entry,(x,y)=>x.id==y.id);
                    GameContext.Instance.LookupDirector.gadgetDefinitionDict[entry.id] = entry;
                    break;
            }
        }
        /// <summary>
        /// Register a vacuumable item into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="id">Id of the vacuumable item</param>
        /// <param name="color">Color of the background of the inventory slot</param>
        /// <param name="icon">Icon that will be used for the item in inventory</param>
        public static void RegisterVacEntry(Identifiable.Id id, Color color, Sprite icon)
        {
            var v = ScriptableObject.CreateInstance<VacItemDefinition>();
            v.id = id;
            v.color = color;
            v.icon = icon;
            RegisterVacEntry(v);
        }

        /// <summary>
        /// Register <paramref name="entry"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="entry">Upgrade Entry to register</param>
        public static void RegisterUpgradeEntry(UpgradeDefinition entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    upgradeEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.upgradeDefinitions.AddAndRemoveWhere(entry,(x,y)=>x.upgrade==y.upgrade);
                    GameContext.Instance.LookupDirector.upgradeDefinitionDict[entry.upgrade] = entry;
                    break;
            }
        }

        /// <summary>
        /// Create and register an upgrade entry
        /// </summary>
        /// <param name="upgrade">Upgrade ID</param>
        /// <param name="icon">Icon that will show up in the upgrade shop</param>
        /// <param name="cost">The cost of the upgrade</param>
        public static void RegisterUpgradeEntry(PlayerState.Upgrade upgrade, Sprite icon, int cost)
        {
            var v = ScriptableObject.CreateInstance<UpgradeDefinition>();
            v.upgrade = upgrade;
            v.icon = icon;
            v.cost = cost;
            RegisterUpgradeEntry(v);
        }

        /// <summary>
        /// Register a <see cref="SpawnResource"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="b"></param>
        public static void RegisterSpawnResource(GameObject b) =>
            SpawnResourcePrefabRegistry.Instance.Register(b.GetComponent<SpawnResource>());

        /// <summary>
        /// Register a gordo prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="gordo"></param>
        public static void RegisterGordo(GameObject gordo) => GordoRegistry.Instance.Register(gordo.GetComponent<GordoIdentifiable>());

        /// <summary>
        /// Register <paramref name="liquid"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="liquid">Liquid to register</param>
        public static void RegisterLiquid(LiquidDefinition liquid) => LiquidRegistry.Instance.Register(liquid);

        /// <summary>
        /// Register <paramref name="entry"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="entry">Entry to register</param>
        public static void RegisterToy(ToyDefinition entry) => API.Identifiable.Slime.ToyRegistry.Instance.Register(entry);

        internal static void AddAndRemoveWhere<T>(this ListAsset<T> list,T value,Func<T,T,bool> cond)
        {
            var v = list.Where(x => cond(value, x)).ToList();
            foreach(var a in v)
            {
                list.items.Remove(a);
            }
            list.items.Add(value);
        }

        /// <summary>
        /// Register a toy (note: does not register the identifiable itself, only the toy, do that separately)
        /// </summary>
        /// <param name="id"><see cref="Identifiable.Id"/> of the toy</param>
        /// <param name="icon">Icon for the toy in the toy store</param>
        /// <param name="cost">How much the toy costs in the toy store</param>
        /// <param name="nameKey"></param>
        public static void RegisterToy(Identifiable.Id id, Sprite icon, int cost, string nameKey)
        {
            var v = ScriptableObject.CreateInstance<ToyDefinition>();
            v.toyId = id;
            v.icon = icon;
            v.cost = cost;
            v.nameKey = nameKey;
            RegisterToy(v);
        }
    }
}
