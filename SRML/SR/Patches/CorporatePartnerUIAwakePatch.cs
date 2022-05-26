using HarmonyLib;
using SRML.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(CorporatePartnerUI), "Awake")]
    internal static class CorporatePartnerUIAwakePatch
    {
        private static CorporatePartnerUI.RankEntry genericEntry = new CorporatePartnerUI.RankEntry()
        {
            cost = 0,
            rewardIcons = new Sprite[0]
        };

        public static void Prefix(CorporatePartnerUI __instance)
        {
            if (genericEntry.rewardBanner == null) genericEntry.rewardBanner = __instance.ranks.Last().rewardBanner;
            foreach (KeyValuePair<int, CorporatePartnerUI.RankEntry> entry in CorporatePartnerRegistry.customEntries)
            {
                while (entry.Key - 1 > __instance.ranks.Length)
                    __instance.ranks = __instance.ranks.AddToArray(genericEntry);
                __instance.ranks = __instance.ranks.AddToArray(new CorporatePartnerUI.RankEntry()
                {
                    cost = entry.Value.cost,
                    rewardIcons = entry.Value.rewardIcons,
                    rewardBanner = genericEntry.rewardBanner
                });
            }

            foreach (KeyValuePair<Sprite, int> entry in CorporatePartnerRegistry.customRewards)
            {
                if (__instance.ranks.Length < entry.Value)
                {
                    Console.Console.Instance.LogWarning("Attempting to add reward to non-existent corporate level! Skipping ...");
                    continue;
                }
                else
                {
                    __instance.ranks[entry.Value - 1].rewardIcons.AddToArray(entry.Key);
                }
            }
        }
    }
}
