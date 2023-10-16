using SRML.API.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR
{
    [Obsolete]
    public static class AchievementRegistry
    {
        private static Dictionary<Tier, AchievementTierRegistry.Tier> convertTiers = new Dictionary<Tier, AchievementTierRegistry.Tier>()
        {
            { Tier.TIER1, AchievementTierRegistry.Tier.TIER_1 },
            { Tier.TIER2, AchievementTierRegistry.Tier.TIER_2 },
            { Tier.TIER3, AchievementTierRegistry.Tier.TIER_3 }
        };

        /// <summary>
        /// Register a modded achievement into the registry.
        /// </summary>
        /// <param name="ach">The id of the achievement to register.</param>
        /// <param name="track">The tracker to track achievement progress.</param>
        /// <param name="tier">The tier the achievement goes into.</param>
        public static void RegisterModdedAchievement(AchievementsDirector.Achievement ach, AchievementsDirector.Tracker track, Tier tier)
        {
            AchievementsRegistry.Instance.RegisterTracker(ach, track);
            AchievementTierRegistry.Instance.RegisterIntoTier(ach, convertTiers[tier]);
        }

        /// <summary>
        /// Register an achievement tier into the registry.
        /// </summary>
        /// <param name="tier">The id of the tier.</param>
        /// <param name="icon">The icon to be displayed in the achievement UI.</param>
        public static void RegisterAchievementTier(Tier tier, Sprite icon)
        {
            if (!convertTiers.ContainsKey(tier))
                convertTiers[tier] = AchievementTierRegistry.Instance.Register(Enum.GetName(typeof(Tier), tier));

            AchievementTierRegistry.Instance.AddIcon(convertTiers[tier], icon);
        }

        /// <summary>
        /// Check if an achievement is modded.
        /// </summary>
        /// <param name="ach">The id of the achievement to check.</param>
        /// <returns>True if achievement id belongs to a modded achievement, otherwise false.</returns>
        public static bool IsModdedAchievement(AchievementsDirector.Achievement ach) =>
            AchievementsRegistry.Instance.IsRegistered(ach);

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
