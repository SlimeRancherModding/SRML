using HarmonyLib;
using MonomiPark.SlimeRancher;
using SRML.SR.Options;

namespace SRML.API.Player.Patches
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
