using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(EconomyDirector))]
    [HarmonyPatch("InitModel")]
    internal static class EconomyPatch
    {
        public static void Prefix(EconomyDirector __instance)
        {
            __instance.baseValueMap = __instance.baseValueMap.AddRangeToArray(PlortRegistry.valueMapsToPatch.ToArray());
        }
    }
}
