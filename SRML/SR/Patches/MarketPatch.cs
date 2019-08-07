using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(MarketUI))]
    [HarmonyPatch("Start")]
    internal static class MarketPatch
    {
        public static void Prefix(MarketUI __instance)
        {
            __instance.plorts = __instance.plorts.Where(x=>!PlortRegistry.plortsToPatch.Any(y=>y.id==x.id)).ToArray().AddRangeToArray(PlortRegistry.plortsToPatch.ToArray());
        }
    }
}
