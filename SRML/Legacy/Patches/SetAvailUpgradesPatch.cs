using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PlayerModel))]
    [HarmonyPatch("SetAvailUpgrades")]
    internal static class SetAvailUpgradesPatch
    {
        public static void Postfix(PlayerModel __instance)
        {
            foreach (var v in PersonalUpgradeRegistry.moddedLockers)
            {
                if (v.Value == null && !__instance.availUpgrades.Contains(v.Key)) __instance.availUpgrades.Add(v.Key);
            }
        }
    }
}
