using System;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(AttachFashions), "GetParentForSlot")]
    internal static class AttachFashionsGetParentBonePatch
    {
        public static bool Prefix(AttachFashions __instance, Fashion.Slot slot, ref Transform __result)
        {
            if (Identifiable.IsSlime(Identifiable.GetId(__instance.gameObject))) return true;
            if (FashionRegistry.slotAttachPoints.ContainsKey(slot))
            {
                __result = __instance.transform.Find(FashionRegistry.slotAttachPoints[slot].Item1);
                return false;
            }
            return true;
        }
    }
}
