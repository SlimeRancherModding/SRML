using HarmonyLib;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(AchievementsDirector), "Awake")]
    internal static class AchievementRegisterVanillaTierPatch
    {
        public static void Prefix(AchievementsDirector __instance)
        {
            AchievementTierRegistry reg = AchievementTierRegistry.Instance;

            __instance.TIER_1.UnionWith(reg.achivementsForTier[AchievementTierRegistry.Tier.TIER_1]);
            __instance.TIER_2.UnionWith(reg.achivementsForTier[AchievementTierRegistry.Tier.TIER_2]);
            __instance.TIER_3.UnionWith(reg.achivementsForTier[AchievementTierRegistry.Tier.TIER_3]);
        }
    }
}
