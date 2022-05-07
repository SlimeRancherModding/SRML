using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(CorporatePartnerUI))]
    [HarmonyPatch("Awake")]
    internal static class CorporatePartnerAwakePatch
    {
        public static void Prefix(CorporatePartnerUI __instance)
        {
            List<CorporatePartnerUI.RankEntry> list = __instance.ranks.ToList();
            foreach (CorporatePartnerUI.RankEntry rankEntry in _7ZeeRegistry.moddedRankEntries)
            {
                if (!list.Contains(rankEntry))
                    list.Add(rankEntry);
            }
            __instance.ranks = list.ToArray();
        }
    }
}
