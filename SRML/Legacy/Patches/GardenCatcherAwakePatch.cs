using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GardenCatcher))]
    [HarmonyPatch("Awake")]
    internal static class GardenCatcherAwakePatch
    {
        public static void Prefix(GardenCatcher __instance)
        {
            PlantSlotRegistry.Patch(__instance);
        }
    }
}
