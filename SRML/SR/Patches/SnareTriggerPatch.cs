using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GordoSnare), "OnTriggerEnter")]
    internal static class SnareTriggerPatch
    {
        public static bool Prefix(GordoSnare __instance, Collider col)
        {
            if (col.isTrigger || __instance.bait != null || __instance.isSnared)
                return true;
            Identifiable component = col.GetComponent<Identifiable>();
            if (component != null && SnareRegistry.snareables.Contains(component.id))
            {
                if (__instance.baitAttachedFx != null)
                    SRBehaviour.SpawnAndPlayFX(__instance.baitAttachedFx, __instance.gameObject);
                Destroyer.DestroyActor(col.gameObject, "GordoSnare.OnTriggerEnter");
                __instance.AttachBait(component.id);
                return false;
            }
            return true;
        }
    }
}
