using HarmonyLib;
using SRML.API.Identifiable.Slime;
using System.Linq;
using UnityEngine;

namespace SRML.API.Identifiable.Patches
{
    [HarmonyPatch(typeof(SlimeAppearanceApplicator), "GetFashionParent")]
    internal static class FashionSlotParentPatch
    {
        public static bool Prefix(SlimeAppearanceApplicator __instance, Fashion.Slot fashionSlot, ref Transform __result)
        {
            var slot = FashionSlotRegistry.Instance.Registered.FirstOrDefault(x => x.Item1 == fashionSlot);
            if (slot != default)
            {
                __result = __instance._boneLookup[slot.Item2].transform;
                return false;
            }
            return true;
        }
    }
}
