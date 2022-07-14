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

        public static void AddToRank(int level, RewardEntry entry)
        {
            if (!rewardsForLevel.ContainsKey(level)) rewardsForLevel[level] = new List<RewardEntry>();
            rewardsForLevel[level].Add(entry);
        }

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
