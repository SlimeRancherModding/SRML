using HarmonyLib;
using MonomiPark.SlimeRancher;
using SRML.SR.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SavedProfile))]
    [HarmonyPatch("Pull")]
    internal static class SavedProfilePullPatch
    {
        public static void Prefix(SavedProfile __instance)
        {
            OptionsHandler.Pull(__instance);
        }
    }
}
