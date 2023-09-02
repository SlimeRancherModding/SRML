using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(PlayerModel), "ApplyUpgrade")]
    internal static class PlayerModelApplyUpgradePatch
    {
        public static bool Prefix(PlayerModel __instance, PlayerState.Upgrade upgrade, bool isFirstTime)
        {
            var upgradeApplierDict = PersonalUpgradeRegistry.Instance.upgradeApplicators;
            if (!upgradeApplierDict.ContainsKey(upgrade))
                return true;

            upgradeApplierDict[upgrade](__instance, isFirstTime);
            return false;
        }
    }
}
