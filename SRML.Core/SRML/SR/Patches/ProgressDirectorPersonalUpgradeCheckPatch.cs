using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ProgressDirector), "CheckProgressUpgrades")]
    internal static class ProgressDirectorPersonalUpgradeCheckPatch
    {
        static void Postfix() => SceneContext.Instance.PlayerState.CheckAllUpgradeLockers();
    }
}
