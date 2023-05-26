using HarmonyLib;
using SRML.SR;
using System.Linq;

namespace SRML.Core.Patches
{
    [HarmonyPatch(typeof(MainMenuUI), "Start")]
    internal static class MainMenuPatch
    {
        public static void Prefix(MainMenuUI __instance)
        {
            if (ErrorGUI.errors.Count() > 0)
                ErrorGUI.TryCreateExtendedError(__instance);
        }
    }
}
