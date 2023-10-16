using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(GameModel), "InstantiateGadget", new[] { typeof(GameObject), typeof(GadgetSiteModel), typeof(bool) })]
    internal static class PlaceGadgetPatch
    {
        public static void Postfix(GadgetSiteModel site, GameObject __result)
        {
            if (ExtendedData.gadgetsInSave.ContainsKey(__result))
                return;
            ExtendedData.OnRegisterGadget(site.id, __result);
        }
    }
}
