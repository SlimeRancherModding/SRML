using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SiloCatcher))]
    [HarmonyPatch("OnTriggerEnter")]
    internal static class SiloCatcherTriggerEnterPatch
    {
        public static GameObject LastInserted;
        public static void Prefix(SiloCatcher __instance, Collider collider)
        {
            if (collider.isTrigger || !__instance.type.HasInput() || Identifiable.GetId(collider.gameObject) == Identifiable.Id.NONE || !collider.gameObject.GetComponent<Vacuumable>() || collider.gameObject.GetComponent<Vacuumable>().isCaptive() || __instance.collectedThisFrame.Contains(collider.gameObject)) return;
            LastInserted = collider.gameObject;
            Debug.Log($"was inserted {collider.gameObject}");
        }
    }
}
