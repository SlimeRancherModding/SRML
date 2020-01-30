using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SceneContext))]
    [HarmonyPatch("Awake")]
    internal class SceneContextAwakePatch
    {
        static void Postfix(SceneContext __instance)
        {
            if (Levels.isMainMenu()) return;
            
        }
    }
}
