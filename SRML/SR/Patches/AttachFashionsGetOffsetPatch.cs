using System;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(AttachFashions), "GetParentOffset")]
    internal static class AttachFashionsGetOffsetPatch
    {
        public static void Postfix(AttachFashions __instance, Identifiable.Id id, ref Vector3 __result)
        {
            if (FashionRegistry.offsetsForFashions.ContainsKey(id))
                __result = FashionRegistry.offsetsForFashions[id].Item1.Invoke(__instance) ? FashionRegistry.offsetsForFashions[id].Item2 : __result;
        }
    }
}
