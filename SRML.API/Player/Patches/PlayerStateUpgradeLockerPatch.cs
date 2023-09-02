using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(PlayerState), "InitUpgradeLocks")]
    internal static class PlayerStateUpgradeLockerPatch
    {
        public static void Postfix(PlayerState __instance, PlayerModel model)
        {
            if (Levels.isSpecial())
                return;

            foreach (PlayerState.Upgrade upgrade in PersonalUpgradeRegistry.Instance.defaultAvails)
                model.availUpgrades.Add(upgrade);
            foreach (var locker in PersonalUpgradeRegistry.Instance.basicUpgradeLockers)
                model.upgradeLocks[locker.Key] = __instance.CreateBasicLock(locker.Value.Item1, locker.Value.Item2, locker.Value.Item3);
            foreach (var locker in PersonalUpgradeRegistry.Instance.upgradeLockers)
                model.upgradeLocks[locker.Key] = locker.Value;
        }
    }
}
