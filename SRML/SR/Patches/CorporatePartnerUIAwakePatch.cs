using HarmonyLib;
using SRML.SR.UI;
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
                    rewardBanner = entry.Value.rewardBanner ?? genericEntry.rewardBanner
                });
            }

            EnhancedCorporatePartnerHandler.rewardEntry = PrefabUtils.CopyPrefab(__instance.rewardObjects[0]);
            GameObject newUI = GameObject.Instantiate(Main.uiBundle.LoadAsset<GameObject>("RewardsPanel"), __instance.rewardObjects[0].transform.parent.parent, false);
            EnhancedCorporatePartnerHandler.content = newUI.transform.GetChild(0).GetChild(0);
            Transform text = __instance.rewardObjects[0].transform.parent.GetChild(0);
            text.SetParent(newUI.transform.parent);
            text.SetAsFirstSibling();
            __instance.rewardObjects[0].transform.parent.gameObject.DestroyImmediate();
            newUI.transform.SetSiblingIndex(1);
            newUI.name = "RewardsPanel";
        }
    }
}
