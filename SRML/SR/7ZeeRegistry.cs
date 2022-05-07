using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.SR
{
    public static class _7ZeeRegistry
    {
        internal static List<CorporatePartnerRankEntry> moddedRankEntries = new List<CorporatePartnerRankEntry>();

        public static void RegisterRankEntry(CorporatePartnerRankEntry rankEntry)
        {
            moddedRankEntries.Add(rankEntry);
        }

        public static void RegisterRankEntry(int level, CorporatePartnerUI.RankEntry rankEntry) => RegisterRankEntry(CorporatePartnerRankEntry.FromSR(level, rankEntry));
    }

    public class CorporatePartnerRankEntry
    {
        public int level;

        public int cost;

        public Sprite[] rewardIcons;

        public Sprite rewardBanner;

        public CorporatePartnerRankEntry(int level, int cost, Sprite rewardBanner, params Sprite[] rewardIcons)
        {
            this.level = level;
            this.cost = cost;
            this.rewardBanner = rewardBanner;
            this.rewardIcons = rewardIcons;
        }

        public CorporatePartnerUI.RankEntry ToSR() => new CorporatePartnerUI.RankEntry
        {
            cost = this.cost,
            rewardBanner = this.rewardBanner,
            rewardIcons = this.rewardIcons
        };

        public static CorporatePartnerRankEntry FromSR(int level, CorporatePartnerUI.RankEntry rankEntry) => new CorporatePartnerRankEntry(level, rankEntry.cost, rankEntry.rewardBanner, rankEntry.rewardIcons);
    }
}
