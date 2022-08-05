using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR
{
    public static class AchievementRegistry
    {
        internal static Dictionary<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, Tier)> moddedAchievements = new Dictionary<AchievementsDirector.Achievement, (AchievementsDirector.Tracker, Tier)>();
        internal static Dictionary<Tier, Sprite> moddedTiers = new Dictionary<Tier, Sprite>();
        internal static Dictionary<AchievementsDirector.Achievement, Tier> achievementCustomTiers = new Dictionary<AchievementsDirector.Achievement, Tier>();

        /// <summary>
        /// Register a modded achievement into the registry.
        /// </summary>
        /// <param name="ach">The id of the achievement to register.</param>
        /// <param name="track">The tracker to track achievement progress.</param>
        /// <param name="tier">The tier the achievement goes into.</param>
        public static void RegisterModdedAchievement(AchievementsDirector.Achievement ach, AchievementsDirector.Tracker track, Tier tier) => 
            moddedAchievements.Add(ach, (track, tier));

        /// <summary>
        /// Register an achievement tier into the registry.
        /// </summary>
        /// <param name="tier">The id of the tier.</param>
        /// <param name="icon">The icon to be displayed in the achievement UI.</param>
        public static void RegisterAchievmentTier(Tier tier, Sprite icon) => moddedTiers.Add(tier, icon);

        /// <summary>
        /// Check if an achievement is modded.
        /// </summary>
        /// <param name="ach">The id of the achievement to check.</param>
        /// <returns>True if achievement id belongs to a modded achievement, otherwise false.</returns>
        public static bool IsModdedAchievement(AchievementsDirector.Achievement ach) => moddedAchievements.ContainsKey(ach);

        internal static bool ModdedAchievementPatch(AchievementsDirector.Achievement achievement) => !IsModdedAchievement(achievement);

        /// <summary>
        /// Tiers for an achievement to be put into.
        /// </summary>
        public enum Tier
        {
            TIER1,
            TIER2,
            TIER3
        }
    }
}
