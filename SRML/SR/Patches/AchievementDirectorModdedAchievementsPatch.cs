using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

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
                    default:
                        AchievementRegistry.achievementCustomTiers[ach.Key] = ach.Value.Item2;
                        break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(AchievementsDirector), "GetAchievementImage")]
    internal static class AchievementsDirectorGetSpritePatch
    {
        public static bool Prefix(AchievementsDirector.Achievement achieve, ref Sprite __result)
        {
            if (AchievementRegistry.achievementCustomTiers.ContainsKey(achieve))
            {
                __result = AchievementRegistry.moddedTiers[AchievementRegistry.achievementCustomTiers[achieve]];
                return false;
            }
            return true;
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
