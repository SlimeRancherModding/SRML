using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(IdHandler), MethodType.Constructor)]
    internal class OnIdHandlerGetPatch
    {
        public static void Postfix(IdHandler __instance)
        {
            ExtendedData.handlersInSave.Add(__instance, string.Empty);
        }
    }
}
