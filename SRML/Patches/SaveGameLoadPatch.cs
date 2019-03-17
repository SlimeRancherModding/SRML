using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
namespace SRML.Patches
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(AutoSaveDirector))]
    internal static class SaveGameLoadPatch
    {
        static MethodBase TargetMethod()
        {
            return typeof(AutoSaveDirector).GetMethod("PushSavedGame", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        static void PostFix(AutoSaveDirector __instance)
        {
            // do something on save game loads
        }
    }
}
