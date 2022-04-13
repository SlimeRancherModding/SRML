using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(WeaponVacuum), "Expel", new System.Type[] { typeof(GameObject), typeof(bool) })]
    internal static class WeaponVacuumExpelEmotionsPatch
    {
        public static void Prefix(WeaponVacuum __instance, ref bool ignoreEmotions)
        {
            if (__instance.player.Ammo.GetSelectedEmotions() != null) return;
            ignoreEmotions = true;
        }
    }
}
