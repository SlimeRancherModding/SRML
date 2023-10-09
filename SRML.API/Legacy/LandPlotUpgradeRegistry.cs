using System;
using UnityEngine;

namespace SRML.SR
{
    [Obsolete]
    public static class LandPlotUpgradeRegistry
    {
        /// <summary>
        /// Creates a <see cref="LandPlot.Upgrade"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="LandPlot.Upgrade"/>.</param>
        /// <param name="name">The name of the <see cref="LandPlot.Upgrade"/>.</param>
        /// <returns>The created <see cref="LandPlot.Upgrade"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static LandPlot.Upgrade CreateLandPlotUpgrade(object value, string name) =>
            API.World.LandPlotUpgradeRegistry.Instance.RegisterAndParse(name, value);

        /// <summary>
        /// Registers an land plot upgrade.
        /// </summary>
        /// <typeparam name="T">The type of land plot to register it to.</typeparam>
        /// <param name="entry">The upgrade to register.</param>
        public static void RegisterPurchasableUpgrade<T>(UpgradeShopEntry entry) where T : LandPlotUI =>
            API.World.PurchasableRegistry.Instance.Register<T>(x => new PurchaseUI.Purchasable(entry.NameKey, entry.icon, entry.mainImg,
                entry.DescKey, entry.cost, entry.landplotPediaId, () => ((LandPlotUI)x).Upgrade(entry.upgrade, entry.cost), 
                () => entry.isUnlocked(((LandPlotUI)x).activator), () => entry.isAvailable(((LandPlotUI)x).activator), 
                null, entry.warning, null, null, entry.holdtopurchase));

        /// <summary>
        /// Registers a <see cref="PlotUpgrader"/> to a land plot.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlotUpgrader"/> to register.</typeparam>
        /// <param name="plot">The <see cref="LandPlot.Id"/> to register the <see cref="PlotUpgrader"/> to</param>
        /// <exception cref="ArgumentException">Throws if the <see cref="PlotUpgrader"/> is already registered.</exception>
        public static void RegisterPlotUpgrader<T>(LandPlot.Id plot) where T : PlotUpgrader =>
            API.World.LandPlotUpgradeRegistry.Instance.AddUpgraderToPlot<T>(plot);

        public struct UpgradeShopEntry
        {
            public LandPlot.Upgrade upgrade;
            public Sprite icon;
            public Sprite mainImg;
            public int cost;
            public PediaDirector.Id landplotPediaId;
            public System.Func<LandPlot, bool> isUnlocked;
            public System.Func<LandPlot, bool> isAvailable;
            public string warning;
            public bool holdtopurchase;
            private string landplotName;

            public string LandPlotName
            {
                get => landplotName == null ? landplotPediaId.ToString().ToLower() : landplotName;
                set => landplotName = value.ToLower();
            }

            public string DescKey => "m.upgrade.desc." + landplotName + "." + upgrade.ToString().ToLower();

            public string NameKey => "m.upgrade.name." + landplotName + "." + upgrade.ToString().ToLower();
        }
    }
}
