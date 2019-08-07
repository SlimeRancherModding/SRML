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
        internal static HashSet<LookupDirector.VacEntry> vacEntriesToPatch = new HashSet<LookupDirector.VacEntry>();
        internal static HashSet<LookupDirector.GadgetEntry> gadgetEntriesToPatch = new HashSet<LookupDirector.GadgetEntry>();

        internal static HashSet<LookupDirector.UpgradeEntry> upgradeEntriesToPatch =
            new HashSet<LookupDirector.UpgradeEntry>();

        internal static HashSet<GameObject> landPlotsToPatch = new HashSet<GameObject>();

        internal static HashSet<GameObject> resourceSpawnersToPatch = new HashSet<GameObject>();

        internal static HashSet<GameObject> gordosToPatch = new HashSet<GameObject>();

        internal static HashSet<LookupDirector.Liquid> liquidsToPatch = new HashSet<LookupDirector.Liquid>();

        internal static HashSet<LookupDirector.ToyEntry> toysToPatch = new HashSet<LookupDirector.ToyEntry>();

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

        public static void RegisterIdentifiablePrefab(Identifiable b)
        {   
            RegisterIdentifiablePrefab(b.gameObject);
        }

        public static void RegisterVacEntry(LookupDirector.VacEntry entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    vacEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.vacEntries.AddAndRemoveWhere(entry,(x,y)=>x.id==y.id);
                    GameContext.Instance.LookupDirector.vacEntryDict[entry.id] = entry;
                    break;
            }
        }

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

        public static void RegisterGadget(LookupDirector.GadgetEntry entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    gadgetEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.gadgetEntries.AddAndRemoveWhere(entry,(x,y)=>x.id==y.id);
                    GameContext.Instance.LookupDirector.gadgetEntryDict[entry.id] = entry;
                    break;
            }
        }

        public static void RegisterVacEntry(Identifiable.Id id, Color color, Sprite icon)
        {
            RegisterVacEntry(new LookupDirector.VacEntry(){id=id,color=color,icon=icon});
        }

        public static void RegisterUpgradeEntry(LookupDirector.UpgradeEntry entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    upgradeEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.upgradeEntries.AddAndRemoveWhere(entry,(x,y)=>x.upgrade==y.upgrade);
                    GameContext.Instance.LookupDirector.upgradeEntryDict[entry.upgrade] = entry;
                    break;
            }
        }

        public static void RegisterUpgradeEntry(PlayerState.Upgrade upgrade, Sprite icon, int cost)
        {
            RegisterUpgradeEntry(new LookupDirector.UpgradeEntry()
            {
                cost = cost,
                icon = icon,
                upgrade = upgrade
            });
        }

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

        public static void RegisterLiquid(LookupDirector.Liquid liquid)
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    liquidsToPatch.Add(liquid);
                    break;
                default:
                    GameContext.Instance.LookupDirector.liquidEntries.AddAndRemoveWhere(liquid,(x,y)=>x.id==y.id);
                    GameContext.Instance.LookupDirector.liquidDict[liquid.id] = liquid;
                    break;
            }
        }

        public static void RegisterToy(LookupDirector.ToyEntry entry)
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    toysToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.toyEntries.AddAndRemoveWhere(entry,(x,y)=>x.toyId==y.toyId);
                    GameContext.Instance.LookupDirector.toyDict[entry.toyId] = entry;
                    break;
            }
        }

        internal static void AddAndRemoveWhere<T>(this List<T> list,T value,Func<T,T,bool> cond)
        {
            var v = list.Where(x => cond(value, x)).ToList();
            foreach(var a in v)
            {
                list.Remove(a);
            }
            list.Add(value);
        }

        public static void RegisterToy(Identifiable.Id id, Sprite icon, int cost,string nameKey)
        {
            RegisterToy(new LookupDirector.ToyEntry() { toyId = id, icon = icon, cost = cost, nameKey = nameKey });
        }
    }
}
