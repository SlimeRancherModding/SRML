using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRML.SR
{
    public static class CorporatePartnerRegistry
    {
        internal static Dictionary<int, CorporatePartnerUI.RankEntry> customEntries = new Dictionary<int, CorporatePartnerUI.RankEntry>();
        internal static Dictionary<int, List<RewardEntry>> rewardsForLevel = new Dictionary<int, List<RewardEntry>>();

        /// <summary>
        /// Adds a rank into the 7Zee Rewards catalogue. If the rank already exists, then combine with the pre-existing one.
        /// </summary>
        /// <param name="level">The level of the rank.</param>
        /// <param name="cost">How much it costs to purchase the rank.</param>
        /// <param name="rewards">The rewards for the rank.</param>
        /// <param name="banner">The banner to show behind the rank name. If null, then it will set to the highest rank's banner.</param>
        public static void AddRank(int level, int cost, RewardEntry[] rewards, Sprite banner = null)
        {
            if (!customEntries.ContainsKey(level))
            {
                customEntries[level] = new CorporatePartnerUI.RankEntry()
                {
                    cost = cost,
                    rewardIcons = new Sprite[0],
                    rewardBanner = banner
                };
            }
            foreach (RewardEntry entry in rewards) AddToRank(level, entry);
        }

        /// <summary>
        /// Adds a reward to a rank.
        /// </summary>
        /// <param name="level">The rank to add a reward to.</param>
        /// <param name="entry">The <see cref="RewardEntry"/> to add.</param>
        public static void AddToRank(int level, RewardEntry entry)
        {
            if (!rewardsForLevel.ContainsKey(level)) rewardsForLevel[level] = new List<RewardEntry>();
            rewardsForLevel[level].Add(entry);
        }

        /// <summary>
        /// A special way to register rewards with a unique translation key.
        /// </summary>
        public struct RewardEntry
        {
            public RewardEntry(Sprite icon, string nameKey)
            {
                this.icon = icon;
                this.nameKey = nameKey;
            }

            public Sprite icon;
            public string nameKey;
        }
    }
}
