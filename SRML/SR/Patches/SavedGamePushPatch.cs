using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    [HarmonyPatch("Push",new Type[] { typeof(GameModel) })]
    internal static class SavedGamePushPatch
    {
        public static void Prefix(SavedGame __instance)
        {
            if (__instance.gameState == null) return;

        }
    }
}
