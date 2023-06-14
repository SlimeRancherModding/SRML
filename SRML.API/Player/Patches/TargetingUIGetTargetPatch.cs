using HarmonyLib;
using System;
using UnityEngine;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(TargetingUI), "GetIdentifiableTarget")]
    internal static class TargetingUIGetTargetPatch
    {
        public static bool Prefix(TargetingUI __instance, GameObject gameObject, ref bool __result)
        {
            global::Identifiable.Id id = global::Identifiable.GetId(gameObject);
            if (id == global::Identifiable.Id.NONE)
            {
                __result = false;
                return false;
            }

            foreach (var kvp in TargetingRegistry.Instance.Registered)
            {
                if (kvp.Item1.Invoke(id))
                {
                    __result = true;

                    string name = kvp.Item2?.Invoke(id);
                    string desc = kvp.Item3?.Invoke(id);
                    __instance.nameText.text = name ?? global::Identifiable.GetName(id);
                    __instance.infoText.text = desc ?? __instance.GetIdentifiableInfoText(id);

                    return false;
                }
            }

            // backwards compat
#pragma warning disable CS0612 // Type or member is obsolete
            foreach (var kvp in SR.TargetingRegistry.customTargetingInfo)
            {
                if (kvp.Item1.Invoke(id))
                {
                    __result = true;

                    string name = kvp.Item2.Item1?.Invoke(id, __instance.uiBundle, __instance.pediaBundle);
                    string desc = kvp.Item2.Item2?.Invoke(id, __instance.uiBundle, __instance.pediaBundle);
                    __instance.nameText.text = name ?? global::Identifiable.GetName(id);
                    __instance.infoText.text = desc ?? __instance.GetIdentifiableInfoText(id);

                    return false;
                }
            }
#pragma warning restore CS0612 // Type or member is obsolete
            return true;
        }
    }
}
