using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PediaDirector))]
    [HarmonyPatch("IsUnlocked")]
    internal static class PediaDirectorIsUnlockedPatch
    {
        public static bool Prefix(PediaDirector __instance, ref bool __result)
        {
            if (__instance.pediaModel == null)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PediaDirector))]
    [HarmonyPatch("GetUnlockedCount")]
    internal static class PediaDirectorGetUnlockedCountPatch
    {
        public static bool Prefix(PediaDirector __instance, ref int __result)
        {
            if (__instance.pediaModel == null)
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}
