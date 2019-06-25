using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GadgetDirector))]
    [HarmonyPatch("InitBlueprintLocks")]
    internal static class InitBlueprintLocksPatch
    {
        public static void Postfix(GadgetDirector __instance)
        {
            if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled)
            {
                foreach(var v in GadgetRegistry.customBlueprintLocks)
                {
                    __instance.blueprintLocks[v.Key] = v.Value(__instance);
                }
            }
        }
    }
}
