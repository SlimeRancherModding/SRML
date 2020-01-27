using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    internal static class ExchangeOfferRegistry
    {
        internal static Dictionary<string, SRMod> customRanchers = new Dictionary<string, SRMod>();
        internal static Dictionary<string, SRMod> customOfferIDs = new Dictionary<string, SRMod>();

        public static bool IsCustom(string id)
        {
            return customRanchers.ContainsKey(id) || customOfferIDs.ContainsKey(id);
        }

        internal static bool IsCustom(ExchangeOfferV04 offer)
        {
            return IsCustom(offer.offerId) || IsCustom(offer.rancherId);
        }

        internal static SRMod GetModForData(ExchangeOfferV04 offer)
        {
            return customRanchers.Get(offer.rancherId) ?? customOfferIDs.Get(offer.offerId);
        }

        internal static SRMod GetModForID(string id) => customRanchers.Get(id) ?? customOfferIDs.Get(id);
    }
}
