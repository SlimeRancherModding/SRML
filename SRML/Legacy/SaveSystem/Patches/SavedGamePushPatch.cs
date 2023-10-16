using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    [HarmonyPatch("Push",new Type[]{typeof(GameModel)})]
    internal static class SavedGamePushPatch
    {
        public static void Postfix(GameModel gameModel)
        {
            //ExtendedData.CullIfNotValid(gameModel);
            PersistentAmmoManager.SyncAll();
        }
    }
}
