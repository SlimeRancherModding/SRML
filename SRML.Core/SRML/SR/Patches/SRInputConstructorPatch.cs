using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRML.SR.Patches
{
    //[HarmonyPatch(typeof(SRInput), MethodType.Constructor)]
    internal static class SRInputConstructorPatch
    {
        public static void Postfix(SRInput __instance)
        {
            //foreach (var v in BindingRegistry.precreators) v(__instance.actions);
        }
    }
}
