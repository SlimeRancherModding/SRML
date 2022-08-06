using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(AttachFashions), "GetParentOffset")]
    internal static class AttachFashionsGetOffsetPatch
    {
        public static void Postfix(AttachFashions __instance, Identifiable.Id id, ref Vector3 __result)
        {
            if (FashionRegistry.offsetsForFashions.TryGetValue(x => x.Item1 == id, out var customOffset))
                __result = customOffset.Item2.Item1.Invoke(__instance) ? customOffset.Item2.Item2 : __result;
        }
    }
}
