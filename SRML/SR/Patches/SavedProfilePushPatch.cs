using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SavedProfile))]
    [HarmonyPatch("PushOptions")]
    internal static class SavedProfilePushPatch
    {
        public static void Prefix(OptionsV11 options)
        {
            OptionsHandler.Push(options);
        }
    }
}
