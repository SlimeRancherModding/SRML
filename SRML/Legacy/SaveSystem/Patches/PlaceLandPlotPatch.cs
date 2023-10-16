using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(LandPlotModel), "InstantiatePlot")]
    internal static class PlaceLandPlotPatch
    {
        public static void Postfix(LandPlotModel __instance)
        {
            GameObject plotObject = __instance.gameObj.GetComponentInChildren<LandPlot>().gameObject;
            if (ExtendedData.gadgetsInSave.ContainsKey(plotObject))
                return;
            ExtendedData.OnRegisterLandPlot(__instance.gameObj.GetComponent<LandPlotLocation>().id, plotObject);
        }
    }
}
