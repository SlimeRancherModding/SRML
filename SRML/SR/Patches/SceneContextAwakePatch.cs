using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SceneContext))]
    [HarmonyPatch("Awake")]
    [HarmonyPriority(Priority.High)]
    internal class SceneContextAwakePatch
    {
        static void Postfix(SceneContext __instance)
        {
            
        }
    }
}
