using System.Collections.Generic;

namespace SRML.SR
{
    public static class AchievementRegistry
    {
        internal static Dictionary<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, Tier)> moddedAchievements = new Dictionary<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, Tier)>();

        public static void RegisterModdedAchievement(AchievementsDirector.Achievement ach, AchievementsDirector.Tracker track, Tier tier) => moddedAchievements.Add(ach, (track, tier));
        
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
