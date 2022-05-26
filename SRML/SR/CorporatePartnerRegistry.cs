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
        internal static Dictionary<Sprite, int> customRewards = new Dictionary<Sprite, int>();

        public static void AddRank(int level, int cost, Sprite[] icons)
        {
            if (customEntries.ContainsKey(level))
            {
                customEntries[level].rewardIcons = icons.Concat(customEntries[level].rewardIcons).ToArray();
            }
            else
            {
                customEntries[level] = new CorporatePartnerUI.RankEntry()
                {
                    cost = cost,
                    rewardIcons = icons
                };
            }
        }

        public static void AddToRank(int level, Sprite icon)
        {
            if (customEntries.ContainsKey(level)) customEntries[level].rewardIcons.AddToArray(icon);
            else customRewards.Add(icon, level);
        }
    }
}
