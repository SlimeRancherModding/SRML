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
            __instance.identifiablePrefabs.AddRange(LookupRegistry.objectsToPatch);
            __instance.vacEntries.AddRange(LookupRegistry.vacEntriesToPatch);
            __instance.gadgetEntries.AddRange(LookupRegistry.gadgetEntriesToPatch);
            __instance.upgradeEntries.AddRange(LookupRegistry.upgradeEntriesToPatch);
            __instance.plotPrefabs.AddRange(LookupRegistry.landPlotsToPatch);
            __instance.resourceSpawnerPrefabs.AddRange(LookupRegistry.resourceSpawnersToPatch);
            __instance.liquidEntries.AddRange(LookupRegistry.liquidsToPatch);
            __instance.gordoEntries.AddRange(LookupRegistry.gordosToPatch);
        }
    }
}
