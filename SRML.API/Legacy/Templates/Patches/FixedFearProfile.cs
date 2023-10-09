using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Templates.Patches
{
    [HarmonyPatch(typeof(FearProfile))]
    [HarmonyPatch("OnEnable")]
    public static class FixedFearProfile
    {
        public static bool Prefix(FearProfile __instance)
        {
            if (__instance.threats == null)
                return false;

            return true;
        }
    }
}
