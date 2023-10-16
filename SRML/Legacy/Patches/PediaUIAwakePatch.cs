using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PediaUI))]
    [HarmonyPatch("Awake")]
    internal static class PediaUIAwakePatch
    {
        public static void Postfix(PediaUI __instance)
        {
            PediaRegistry.activeRenderer = null; 
            foreach(var v in PediaRegistry.customTabs)
            {
                if(v.Key.IsVisible?.Invoke()??true) v.Key.InitForPediaAwake(__instance);
            }
        }
    }
}
