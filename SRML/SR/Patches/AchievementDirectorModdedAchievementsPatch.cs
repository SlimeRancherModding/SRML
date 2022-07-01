using HarmonyLib;
using System.Collections.Generic;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(AchievementsDirector), "Awake")]
    internal static class AchievementDirectorModdedAchievementsPatch
    {
        public static void Postfix(AchievementsDirector __instance)
        {
            foreach (KeyValuePair<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, AchievementRegistry.Tier)> ach in AchievementRegistry.moddedAchievements)
            {
                switch (ach.Value.Item2)
                {
                    case AchievementRegistry.Tier.TIER1:
                        __instance.TIER_1.Add(ach.Key);
                        break;
                    case AchievementRegistry.Tier.TIER2:
                        __instance.TIER_2.Add(ach.Key);
                        break;
                    case AchievementRegistry.Tier.TIER3:
                        __instance.TIER_3.Add(ach.Key);
                        break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(AchievementsDirector), "InitForLevel")]
    internal static class AchievementsDirectorInitTrackersPatch
    {
        public static void Postfix(AchievementsDirector __instance)
        {
            foreach (KeyValuePair<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, AchievementRegistry.Tier)> ach in AchievementRegistry.moddedAchievements)
            {
                AchievementsDirector.Tracker tracker = ach.Value.Item1;
                tracker.dir = __instance;
                __instance.trackers[ach.Key] = tracker;
            }
        }
    }
}
