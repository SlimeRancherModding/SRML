using HarmonyLib;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(LandPlot), "OnDestroy")]
    internal static class OnLandPlotDestroyPatch
    {
        public static void Prefix(LandPlot __instance) => ExtendedData.landplotsInSave.Remove(__instance.gameObject);
    }
}
