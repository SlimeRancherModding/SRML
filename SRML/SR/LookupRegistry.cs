using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SRML.SRModLoader;

namespace SRML.SR
{
    public static class LookupRegistry
    {
        internal static HashSet<GameObject> objectsToPatch = new HashSet<GameObject>();
        internal static HashSet<VacItemDefinition> vacEntriesToPatch = new HashSet<VacItemDefinition>();
        internal static HashSet<GadgetDefinition> gadgetEntriesToPatch = new HashSet<GadgetDefinition>();

        internal static HashSet<UpgradeDefinition> upgradeEntriesToPatch =
            new HashSet<UpgradeDefinition>();

        internal static HashSet<GameObject> landPlotsToPatch = new HashSet<GameObject>();

        internal static HashSet<GameObject> resourceSpawnersToPatch = new HashSet<GameObject>();

        internal static HashSet<GameObject> gordosToPatch = new HashSet<GameObject>();

        internal static HashSet<LiquidDefinition> liquidsToPatch = new HashSet<LiquidDefinition>();

        internal static HashSet<ToyDefinition> toysToPatch = new HashSet<ToyDefinition>();

        /// <summary>
        /// Register an Identifiable Prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="b">The prefab to register.</param>
        public static void RegisterIdentifiablePrefab(GameObject b)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    objectsToPatch.Add(b);
                    break;
                default:
                    GameContext.Instance.LookupDirector.identifiablePrefabs.AddAndRemoveWhere(b,(x,y)=> Identifiable.GetId(x)== Identifiable.GetId(y));
                    GameContext.Instance.LookupDirector.identifiablePrefabDict[Identifiable.GetId(b)] = b;
                    break;
            }
        }

        /// <summary>
        /// Register an Identifiable Prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="b">The <see cref="Identifiable"/> belonging to the prefab to register.</param>
        public static void RegisterIdentifiablePrefab(Identifiable b)
        {   
            RegisterIdentifiablePrefab(b.gameObject);
        }

        /// <summary>
        /// Register <paramref name="entry"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="entry"></param>
        public static void RegisterVacEntry(VacItemDefinition entry)
        {
            
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    vacEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.vacItemDefinitions.AddAndRemoveWhere(entry, (x,y) => x.id == y.id);
                    GameContext.Instance.LookupDirector.vacItemDict[entry.id] = entry;
                    break;
            }
        }
        /// <summary>
        /// Register a landplot prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="prefab">The prefab to register</param>
        public static void RegisterLandPlot(GameObject prefab)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    landPlotsToPatch.Add(prefab);
                    break;
                default:
                    GameContext.Instance.LookupDirector.plotPrefabs.AddAndRemoveWhere(prefab,(x,y)=> x.GetComponentInChildren<LandPlot>(true).typeId== y.GetComponentInChildren<LandPlot>(true).typeId);
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
        public static void RegisterSpawnResource(GameObject b)
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    resourceSpawnersToPatch.Add(b);
                    break;
                default:
                    GameContext.Instance.LookupDirector.resourceSpawnerPrefabs.AddAndRemoveWhere(b,(x,y)=>x.GetComponent<SpawnResource>().id==y.GetComponent<SpawnResource>().id);
                    GameContext.Instance.LookupDirector.resourcePrefabDict[b.GetComponent<SpawnResource>().id] = b;
                    break;
            }
        }

        /// <summary>
        /// Register a gordo prefab into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="gordo"></param>
        public static void RegisterGordo(GameObject gordo)
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    gordosToPatch.Add(gordo);
                    break;
                default:
                    GameContext.Instance.LookupDirector.gordoEntries.AddAndRemoveWhere(gordo,(x,y)=>x.GetComponent<GordoIdentifiable>().id==y.GetComponent<GordoIdentifiable>().id);
                    GameContext.Instance.LookupDirector.gordoDict[gordo.GetComponent<GordoIdentifiable>().id] = gordo;
                    break;
            }
        }

        /// <summary>
        /// Register <paramref name="liquid"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="liquid">Liquid to register</param>
        public static void RegisterLiquid(LiquidDefinition liquid)
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    liquidsToPatch.Add(liquid);
                    break;
                default:
                    GameContext.Instance.LookupDirector.liquidDefinitions.AddAndRemoveWhere(liquid,(x,y)=>x.id==y.id);
                    GameContext.Instance.LookupDirector.liquidDict[liquid.id] = liquid;
                    break;
            }
        }

        /// <summary>
        /// Register <paramref name="entry"/> into the <see cref="LookupDirector"/>
        /// </summary>
        /// <param name="entry">Entry to register</param>
        public static void RegisterToy(ToyDefinition entry)
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    toysToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.toyDefinitions.AddAndRemoveWhere(entry,(x,y)=>x.toyId==y.toyId);
                    GameContext.Instance.LookupDirector.toyDict[entry.toyId] = entry;
                    break;
            }
        }

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
