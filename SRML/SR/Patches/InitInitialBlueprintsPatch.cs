using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GadgetsModel))]
    [HarmonyPatch("InitInitialBlueprints")]
    internal static class InitInitialBlueprintsPatch
    {
        public static void Postfix(GadgetsModel __instance)
        {
            foreach (var v in GadgetRegistry.defaultAvailBlueprints.Union(GadgetRegistry.defaultBlueprints)) __instance.availBlueprints.Add(v);
            foreach (var v in GadgetRegistry.defaultBlueprints) __instance.blueprints.Add(v);
        }
    }
}
