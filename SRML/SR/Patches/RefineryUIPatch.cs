using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(RefineryUI))]
    [HarmonyPatch("Awake")]
    internal static class RefineryUIPatch
    {
        public static void Prefix(RefineryUI __instance)
        {
            __instance.listedItems = __instance.listedItems.AddRangeToArray(AmmoRegistry.customRefineryResources.ToArray());
        }
    }
}
