using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(IdHandler))]
    [HarmonyPatch("Awake")]
    internal static class IdHandlerAwakePatch
    {
        public static void Prefix(IdHandler __instance)
        {
            Debug.Log(__instance);
            if (NeedsReplacing(__instance.id))
            {
                __instance.id = ModdedStringRegistry.ParseModColonAlias(__instance.IdPrefix(),__instance.id);
            }
        }

        static bool NeedsReplacing(string id) => id.Contains(":");
    }
}
