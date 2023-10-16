using HarmonyLib;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(Destroyer), "DestroyGadget")]
    internal static class OnGadgetDestroyPatch
    {
        public static void Prefix(GameObject gadgetObj) => ExtendedData.gadgetsInSave.Remove(gadgetObj);
    }
}
