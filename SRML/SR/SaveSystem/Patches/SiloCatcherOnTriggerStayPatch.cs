using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    
    [HarmonyPatch(typeof(SiloCatcher))]
    [HarmonyPatch("Remove")]
    internal static class SiloCatcherRemovePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
        {
            foreach (var v in codes)
            {
                if ((v.opcode == OpCodes.Callvirt && (v.operand as MethodInfo).Name == "DecrementSelectedAmmo"))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SiloCatcherRemovePatch), "PlaceHolder"));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SiloCatcherRemovePatch), "Decrement"));
                }
                else yield return v; 
            }
        }

        public static void Decrement(SiloCatcher catcher)
        {
            catcher.StartCoroutine(Coroutine(catcher.storageSilo.GetRelevantAmmo()));
        }

        static IEnumerator Coroutine(Ammo ammo)
        {
            yield return new WaitForEndOfFrame();
            ammo.DecrementSelectedAmmo();
        }

        public static void PlaceHolder(Ammo ammo,int amount) {  
        }
        
    }

    [HarmonyPatch(typeof(SiloCatcher))]
    [HarmonyPatch("OnTriggerStay")]
    internal static class SiloCatcherOnTriggerStayPatch
    {
        public static void Postfix(SiloCatcher __instance,Collider collider)
        {
            if (!__instance.type.HasOutput())
            {
                return;
            }
            if (Time.time < __instance.nextEject)
            {
                return;
            }
            SiloActivator componentInParent = collider.gameObject.GetComponentInParent<SiloActivator>();
            if (componentInParent == null || !componentInParent.enabled)
            {
                return;
            }
            Vector3 normalized = (collider.gameObject.transform.position - __instance.transform.position).normalized;
            if (Mathf.Abs(Vector3.Angle(__instance.transform.forward, normalized)) > 45f)
            {
                return;
            }
            switch (__instance.type)
            {
                case SiloCatcher.Type.SILO_DEFAULT:
                case SiloCatcher.Type.SILO_OUTPUT_ONLY:
                    var ammo = __instance.storageSilo.GetRelevantAmmo();
                    //if(ammo.GetSelectedId()!=Identifiable.Id.NONE) ammo.DecrementSelectedAmmo();
                    break;
            }
        }
    }
}
