using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ExchangeDirector), "Awake")]
    internal static class ExchangeDirectorAwakePatch
    {
        public static void Prefix(ExchangeDirector __instance)
        {
            foreach (ExchangeDirector.Rancher rancher in ExchangeOfferRegistry.customRanchers.Keys)
            {
                ExchangeDirector.Rancher modified = rancher;
                modified.chatBackground = modified.chatBackground ?? __instance.ranchers[0].chatBackground;
                __instance.ranchers = __instance.ranchers.AddToArray(modified);
            }
            foreach ((ExchangeDirector.Category, Identifiable.Id[]) cat in ExchangeOfferRegistry.customCategories.Keys)
                __instance.catDict.Add(cat.Item1, cat.Item2);
            foreach (ExchangeDirector.UnlockList list in ExchangeOfferRegistry.customUnlocks.Values)
            {
                if (list.unlock == ProgressDirector.ProgressType.NONE) __instance.initUnlocked = __instance.initUnlocked.AddRangeToArray(list.ids);
                else __instance.unlockLists = __instance.unlockLists.AddToArray(list);
            }
            foreach (KeyValuePair<Identifiable.Id, float> d in ExchangeOfferRegistry.customUnlockValues)
                __instance.valueDict[d.Key] = d.Value;
        }
    }
    [HarmonyPatch(typeof(ExchangeDirector), "Start")]
    internal static class ExchangeDirectorStartPatch
    {
        public static void Prefix(ExchangeDirector __instance)
        {
            if (!__instance.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.GENERAL)) return;
            if (__instance.GetRancher(__instance.worldModel.currOffers[ExchangeDirector.OfferType.GENERAL].rancherId) == null)
                __instance.ClearOffer(ExchangeDirector.OfferType.GENERAL);
        }
    }

    [HarmonyPatch(typeof(ProgressDirector), "GetRancherProgressType")]
    internal static class RancherProgressPatch
    {
        public static bool Prefix(ProgressDirector __instance, ref ProgressDirector.ProgressType __result, string rancherName)
        {
            return !ExchangeOfferRegistry.customRancherProgress.TryGetValue(rancherName, out __result);
        }
    }
}
