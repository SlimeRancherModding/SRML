using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using SRML.SR.UI;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(MainMenuUI))]
    [HarmonyPatch("Start")]
    internal static class MainMenuStartPatch
    {
        public static void Postfix(MainMenuUI __instance)
        {
            IEnumerable<SRMod> erroring = SRModLoader.Mods.Values.Where(x => x.ModInfo.EncounteredError && x.ModInfo.LoadState != SRModInfo.State.INITIALIZATION_ERROR);
            
            if (erroring.Count() > 0)
                ErrorGUI.TryCreateExtendedError(__instance, ErrorGUI.errorUI, erroring);
            else
                SRCallbacks.OnMainMenuLoad(__instance);
        }
    }
}
