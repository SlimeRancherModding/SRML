using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.Options;

namespace SRML.API.Player.Patches
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
