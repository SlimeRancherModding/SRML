using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
    public static class _7ZeeRegistry
    {
        internal static List<CorporatePartnerUI.RankEntry> moddedRankEntries = new List<CorporatePartnerUI.RankEntry>();

        public static void RegisterRankEntry(CorporatePartnerUI.RankEntry rankEntry)
        {
            moddedRankEntries.Add(rankEntry);
        }
    }
}
