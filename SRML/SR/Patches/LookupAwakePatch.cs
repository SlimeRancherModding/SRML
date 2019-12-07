using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(LookupDirector))]
    [HarmonyPatch("Awake")]
    internal static class LookupAwakePatch
    {
        public static void Prefix(LookupDirector __instance)
        {
            __instance.identifiablePrefabs.AddAndRemoveRangeWhere(LookupRegistry.objectsToPatch,(x,y)=>Identifiable.GetId(x)==Identifiable.GetId(y));
            __instance.vacItemDefinitions.AddAndRemoveRangeWhere(LookupRegistry.vacEntriesToPatch,(x,y)=>x.id==y.id);
            __instance.gadgetDefinitions.AddAndRemoveRangeWhere(LookupRegistry.gadgetEntriesToPatch, (x, y) =>x.id == y.id);
            __instance.upgradeDefinitions.AddAndRemoveRangeWhere(LookupRegistry.upgradeEntriesToPatch, (x, y) =>x.upgrade==y.upgrade);
            __instance.plotPrefabs.AddAndRemoveRangeWhere(LookupRegistry.landPlotsToPatch, (x, y) =>x.GetComponentInChildren<LandPlot>().typeId==y.GetComponentInChildren<LandPlot>().typeId);
            __instance.resourceSpawnerPrefabs.AddAndRemoveRangeWhere(LookupRegistry.resourceSpawnersToPatch, (x, y) =>x.GetComponent<SpawnResource>().id==y.GetComponent<SpawnResource>().id);
            __instance.liquidDefinitions.AddAndRemoveRangeWhere(LookupRegistry.liquidsToPatch, (x, y) =>x.id==y.id);
            __instance.gordoEntries.AddAndRemoveRangeWhere(LookupRegistry.gordosToPatch, (x, y) =>y.GetComponent<GordoIdentifiable>().id==x.GetComponent<GordoIdentifiable>().id);
            __instance.toyDefinitions.AddAndRemoveRangeWhere(LookupRegistry.toysToPatch, (x, y) =>x.toyId==y.toyId);
        }

        internal static void AddAndRemoveRangeWhere<T>(this ListAsset<T> list, IEnumerable<T> range, Func<T, T, bool> cond)
        {
            var listToAdd = range.ToList();

            var v = list.Where(x => listToAdd.Any(y=>cond(x,y))).ToList();
            foreach (var a in v)
            {
                list.items.Remove(a);
            }
            list.items.AddRange(listToAdd);
        }
    }
}
