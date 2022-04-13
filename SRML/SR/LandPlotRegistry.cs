using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class LandPlotRegistry
    {
        internal static IDRegistry<LandPlot.Id> landplots = new IDRegistry<LandPlot.Id>();
        static LandPlotRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(landplots);
        }

        public static LandPlot.Id CreateLandPlotId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register landplots outside of the PreLoad step");
            return landplots.RegisterValueWithEnum((LandPlot.Id)value, name);
        }

        public static bool IsModdedLandPlot(LandPlot.Id id)
        {
            return landplots.ContainsKey(id);
        }

        public static void RegisterPurchasableLandPlot(LandPlotShopEntry entry)
        {
            PurchasableUIRegistry.RegisterPurchasable<EmptyPlotUI>(x => new PurchaseUI.Purchasable(entry.NameKey, entry.icon, entry.mainImg, entry.DescKey, entry.cost, entry.pediaId, () =>
            {
                EmptyPlotUI emptyPlotUi = x;
                emptyPlotUi.BuyPlot(new LandPlotUI.PlotPurchaseItem()
                {
                    icon = entry.icon,
                    img = entry.mainImg,
                    cost = entry.cost,
                    plotPrefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPlotPrefab(entry.plot)
                });
            }, entry.isUnlocked ?? (() => true), (() => true), null, null, null, null));
        }

        public struct LandPlotShopEntry
        {
            public LandPlot.Id plot;
            public Sprite icon;
            public Sprite mainImg;
            public int cost;
            public PediaDirector.Id? pediaId;
            public Func<bool> isUnlocked;

            public string NameKey => "t." + plot.ToString().ToLower();
            public string DescKey => "m.intro." + plot.ToString().ToLower();
        }
    }
}
