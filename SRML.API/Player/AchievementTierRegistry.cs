using SRML.Core.API.BuiltIn;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.API.Player
{
    public class AchievementTierRegistry : EnumRegistry<AchievementTierRegistry, AchievementTierRegistry.Tier>
    {
        internal Dictionary<Tier, HashSet<AchievementsDirector.Achievement>> achivementsForTier = 
            new Dictionary<Tier, HashSet<AchievementsDirector.Achievement>>();
        internal Dictionary<AchievementsDirector.Achievement, Tier> tiersForAchievement =
            new Dictionary<AchievementsDirector.Achievement, Tier>();
        internal Dictionary<Tier, Sprite> spritesForTiers = new Dictionary<Tier, Sprite>();

        public override void Initialize()
        {
            base.Initialize();
            achivementsForTier[Tier.TIER_1] = new HashSet<AchievementsDirector.Achievement>();
            achivementsForTier[Tier.TIER_2] = new HashSet<AchievementsDirector.Achievement>();
            achivementsForTier[Tier.TIER_3] = new HashSet<AchievementsDirector.Achievement>();
        }

        public void AddIcon(Tier tier, Sprite icon) => spritesForTiers[tier] = icon;

        public void RegisterIntoTier(AchievementsDirector.Achievement achievement, Tier tier)
        {
            achivementsForTier[tier].Add(achievement);
            tiersForAchievement[achievement] = tier;
        }

        public AchievementsDirector.Achievement[] GetModdedAchievements(Tier tier) => achivementsForTier[tier].ToArray();

        public bool IsModdedTier(Tier tier) => tier > Tier.TIER_3;

        public override void Process(Tier toProcess) =>
            achivementsForTier[toProcess] = new HashSet<AchievementsDirector.Achievement>();

        public enum Tier
        {
            TIER_1,
            TIER_2,
            TIER_3
        }
    }
}
