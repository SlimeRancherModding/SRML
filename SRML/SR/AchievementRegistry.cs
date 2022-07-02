using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR
{
    public static class AchievementRegistry
    {
        internal static Dictionary<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, Tier)> moddedAchievements = new Dictionary<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, Tier)>();
        internal static Dictionary<Tier, Sprite> moddedTiers = new Dictionary<Tier, Sprite>();
        internal static Dictionary<AchievementsDirector.Achievement, Tier> achievementCustomTiers = new Dictionary<AchievementsDirector.Achievement, Tier>();

        public static void RegisterModdedAchievement(AchievementsDirector.Achievement ach, AchievementsDirector.Tracker track, Tier tier) => moddedAchievements.Add(ach, (track, tier));

        public static void RegisterAchievmentTier(Tier tier, Sprite icon) => moddedTiers.Add(tier, icon);

        public static bool IsModdedAchievement(AchievementsDirector.Achievement ach) => moddedAchievements.ContainsKey(ach);

        internal static bool ModdedAchievementPatch(AchievementsDirector.Achievement achievement) => !IsModdedAchievement(achievement);

        public enum Tier
        {
            TIER1,
            TIER2,
            TIER3
        }
    }
}
