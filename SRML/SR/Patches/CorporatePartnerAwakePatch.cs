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
            int highestLevel = _7ZeeRegistry.moddedRankEntries.Max(rank => rank.level);
            if (highestLevel < 28)
                highestLevel = 28;

            Dictionary<int, CorporatePartnerUI.RankEntry> dictionary = new Dictionary<int, CorporatePartnerUI.RankEntry>();
            for (int i = 0; i < __instance.ranks.Length; i++)
                dictionary.Add(i, __instance.ranks[i]);

            foreach (CorporatePartnerRankEntry rankEntry in _7ZeeRegistry.moddedRankEntries)
            {
                if (!dictionary.ContainsKey(rankEntry.level))
                    dictionary.Add(rankEntry.level, rankEntry.ToSR());
                else
                {
                    CorporatePartnerUI.RankEntry oldRankEntry = dictionary[rankEntry.level];
                    oldRankEntry.cost = rankEntry.cost;
                    oldRankEntry.rewardBanner = rankEntry.rewardBanner;
                    List<Sprite> joinedIcons = oldRankEntry.rewardIcons.ToList();
                    joinedIcons.AddRange(rankEntry.rewardIcons);
                    oldRankEntry.rewardIcons = joinedIcons.ToArray();
                    dictionary[rankEntry.level] = oldRankEntry;
                }
            }

            for (int i = 0; i < highestLevel; i++)
            {
                if (!dictionary.ContainsKey(i))
                    dictionary.Add(i, new CorporatePartnerUI.RankEntry
                    {
                        cost = 0,
                        rewardBanner = null,
                        rewardIcons = Array.Empty<Sprite>(),
                    });
            }

            __instance.ranks = dictionary.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();
        }
    }
}
