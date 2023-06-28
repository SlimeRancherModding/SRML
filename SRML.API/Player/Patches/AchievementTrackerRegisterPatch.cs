using HarmonyLib;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(AchievementsDirector), "InitForLevel")]
    internal static class AchievementTrackerRegisterPatch
    {
        public static void Postfix(AchievementsDirector __instance)
        {
            foreach (var achAndTracker in AchievementsRegistry.Instance.trackersForAchievements)
            {
                achAndTracker.Value.dir = __instance;
                __instance.trackers[achAndTracker.Key] = achAndTracker.Value;
            }
        }
    }
}
