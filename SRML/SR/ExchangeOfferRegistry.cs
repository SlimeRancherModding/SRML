using HarmonyLib;
using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class ExchangeOfferRegistry
    {
        internal static Dictionary<ExchangeDirector.Rancher, SRMod> customRanchers = new Dictionary<ExchangeDirector.Rancher, SRMod>();
        internal static Dictionary<(ExchangeDirector.Category, Identifiable.Id[]), SRMod> customCategories = new Dictionary<(ExchangeDirector.Category, Identifiable.Id[]), SRMod>();
        internal static Dictionary<ProgressDirector.ProgressType, ExchangeDirector.UnlockList> customUnlocks = new Dictionary<ProgressDirector.ProgressType, ExchangeDirector.UnlockList>();
        internal static Dictionary<Identifiable.Id, float> customUnlockValues = new Dictionary<Identifiable.Id, float>();
        internal static Dictionary<string, SRMod> customRancherIDs = new Dictionary<string, SRMod>();
        internal static Dictionary<string, SRMod> customOfferIDs = new Dictionary<string, SRMod>();

        public static void RegisterRancher(ExchangeDirector.Rancher rancher) => customRanchers.Add(rancher, SRMod.GetCurrentMod());

        public static void RegisterRancherID(string id) => customRancherIDs.Add(id, SRMod.GetCurrentMod());

        public static void RegisterOfferID(string id) => customOfferIDs.Add(id, SRMod.GetCurrentMod());

        public static void RegisterCategory(ExchangeDirector.Category category, Identifiable.Id[] ids) => customCategories.Add((category, ids), SRMod.GetCurrentMod());

        public static void RegisterUnlockableItem(Identifiable.Id item, ProgressDirector.ProgressType type, int countForValue)
        {
            if (!customUnlocks.ContainsKey(type)) customUnlocks[type] = new ExchangeDirector.UnlockList() { unlock = type, ids = new Identifiable.Id[0] };
            customUnlocks[type].ids = customUnlocks[type].ids.AddToArray(item);
            customUnlockValues.Add(item, countForValue);
        }

        public static void RegisterInitialItem(Identifiable.Id item, int countForValue) => RegisterUnlockableItem(item, ProgressDirector.ProgressType.NONE, countForValue);

        public static bool IsCustom(string id) => customRancherIDs.ContainsKey(id) || customOfferIDs.ContainsKey(id);

        public static bool IsCustom(ExchangeDirector.Rancher entry) => customRanchers.ContainsKey(entry);

        public static bool IsCustom(ExchangeDirector.Category cat) => customCategories.Any(x => x.Key.Item1 == cat);

        internal static bool IsCustom(ExchangeOfferV04 offer) => IsCustom(offer.offerId) || IsCustom(offer.rancherId);

        internal static SRMod GetModForData(ExchangeOfferV04 offer) => customRancherIDs.Get(offer.rancherId) ?? customOfferIDs.Get(offer.offerId);

        internal static SRMod GetModForID(string id) => customRancherIDs.Get(id) ?? customOfferIDs.Get(id);
    }
}
