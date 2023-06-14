using HarmonyLib;
using System;
using UnityEngine;

namespace SRML.API.Gadget.Patches
{
    [HarmonyPatch(typeof(GordoSnare), "AttachBait")]
    internal static class GordoSnarePatch
    {
        public static void Postfix(GordoSnare __instance)
        {
            foreach (Type t in SnareRegistry.Instance.RemoveOnSnare)
            {
                foreach (Component c in __instance.GetComponentsInChildren(t))
                    UnityEngine.Object.Destroy(c);
            }
        }
    }
}
