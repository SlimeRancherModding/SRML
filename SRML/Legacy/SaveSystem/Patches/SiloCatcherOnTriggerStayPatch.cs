using HarmonyLib;
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
                if (v.opcode == OpCodes.Callvirt && (v.operand as MethodInfo).Name == "DecrementSelectedAmmo")
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
            yield return null;
            ammo.DecrementSelectedAmmo();
        }

        public static void PlaceHolder(Ammo ammo,int amount) {  
        }
        
    }
}
