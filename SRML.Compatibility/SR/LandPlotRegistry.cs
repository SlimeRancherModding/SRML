using System;
using UnityEngine;

namespace SRML.SR
{
    [Obsolete]
    public static class LandPlotRegistry
    {
        /// <summary>
        /// Creates a <see cref="LandPlot.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="LandPlot.Id"/>.</param>
        /// <param name="name">The name of the <see cref="LandPlot.Id"/>.</param>
        /// <returns>The created <see cref="LandPlot.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static LandPlot.Id CreateLandPlotId(object value, string name) => API.World.LandPlotRegistry.Instance.RegisterAndParse(name, value);

        /// <summary>
        /// Check if an <see cref="Identifiable.Id"/> was registered by a mod
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the <see cref="Identifiable.Id"/> was registered by a mod, otherwise false.</returns>
        public static bool IsModdedLandPlot(LandPlot.Id id) => API.World.LandPlotRegistry.Instance.IsRegistered(id);

        /// <summary>
        /// Registers a land plot.
        /// </summary>
        /// <param name="entry">The land plot to register.</param>
        public static void RegisterPurchasableLandPlot(LandPlotShopEntry entry) =>
            API.World.LandPlotRegistry.Instance.RegisterAsPurchasable(new LandPlotUI.PlotPurchaseItem()
            {
                cost = entry.cost,
                icon = entry.icon,
                img = entry.mainImg,
                plotPrefab = GameContext.Instance.LookupDirector.GetPlotPrefab(entry.plot)
            }, entry.NameKey, entry.DescKey, entry.isUnlocked, () => true, entry.pediaId);

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
