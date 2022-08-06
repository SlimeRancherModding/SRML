using System;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SlimeAppearanceApplicator), "GetFashionParent")]
    internal static class SlimeAppearanceApplicatorBoneSlotPatch
    {
        public static bool Prefix(SlimeAppearanceApplicator __instance, Fashion.Slot fashionSlot, ref Transform __result)
        {
            if (FashionRegistry.slotAttachPoints.ContainsKey(fashionSlot))
            {
                __result = __instance._boneLookup[FashionRegistry.slotAttachPoints[fashionSlot].Item2].transform;
                return false;
            }
            return true;
        }
    }
}
