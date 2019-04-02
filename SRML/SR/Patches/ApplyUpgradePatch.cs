using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PlayerState))]
    [HarmonyPatch("ApplyUpgrade")]
    internal static class ApplyUpgradePatch
    {
        public static bool Prefix(PlayerState __instance, PlayerState.Upgrade upgrade, bool isFirstTime)
        {
            if (PersonalUpgradeRegistry.upgradeCallbacks.TryGetValue(upgrade, out var callback))
            {
                callback(__instance, isFirstTime);
                return false;
            }

            return true;
        }
    }
}
