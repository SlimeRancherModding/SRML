using HarmonyLib;
using System;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(TargetingUI), "GetIdentifiableTarget")]
    internal static class TargetingUIGetTargetPatch
    {
        public static bool Prefix(TargetingUI __instance, GameObject gameObject, ref bool __result)
        {
            Identifiable.Id id = Identifiable.GetId(gameObject);
            if (id == Identifiable.Id.NONE)
            {
                __result = false;
                return false;
            }

            foreach ((Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, MessageBundle, string>)) kvp in TargetingRegistry.customTargetingInfo)
            {
                if (kvp.Item1.Invoke(id))
                {
                    __result = true;
                    __instance.nameText.text = kvp.Item2.Item1 == null ? Identifiable.GetName(id) : kvp.Item2.Item1.Invoke(id, __instance.uiBundle, __instance.pediaBundle);
                    __instance.infoText.text = kvp.Item2.Item2 == null ? __instance.GetIdentifiableInfoText(id) : kvp.Item2.Item2.Invoke(id, __instance.uiBundle, __instance.pediaBundle);
                    return false;
                }
            }
            return true;
        }
    }
}
