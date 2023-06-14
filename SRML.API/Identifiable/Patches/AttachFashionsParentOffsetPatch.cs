using HarmonyLib;
using SRML.API.Identifiable.Slime;
using UnityEngine;

namespace SRML.API.Identifiable.Patches
{
    [HarmonyPatch(typeof(AttachFashions), "GetParentOffset")]
    internal static class AttachFashionsParentOffsetPatch
    {
        public static void Postfix(AttachFashions __instance, global::Identifiable.Id id, ref Vector3 __result)
        {
            if (FashionOffsetRegistry.Instance.Registered.TryGetValue(x => x.Item1 == id, out var customOffset))
                __result = customOffset.Item3.Invoke(__instance) ? customOffset.Item2 : __result;
        }
    }
}
