using HarmonyLib;
using SRML.SR;

namespace SRML.Core.Patches
{
    [HarmonyPatch(typeof(GameContext), "Start")]
    internal static class GameContextAwakePatch
    {
        public static void Prefix()
        {
            SRCallbacks.OnLoad();
        }
    }
}
