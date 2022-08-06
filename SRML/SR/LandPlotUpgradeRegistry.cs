using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class LandPlotUpgradeRegistry
    {
        internal static readonly IDRegistry<LandPlot.Upgrade> moddedUpgrades = new IDRegistry<LandPlot.Upgrade>();
        internal static readonly List<(Type, LandPlot.Id)> moddedUpgraders = new List<(Type, LandPlot.Id)>();
        public static readonly string DemolishKey = MessageUtil.Qualify("ui", "l.demolish_plot");
        public static readonly string ClearCropKey = MessageUtil.Qualify("ui", "b.clear_crop");

        static LandPlotUpgradeRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedUpgrades);
            PurchasableUIRegistry.RegisterManipulator((LandPlotUI ui, ref PurchaseUI.Purchasable[] purchasables) =>
            {
                List<PurchaseUI.Purchasable> list = purchasables.ToList();
                PurchaseUI.Purchasable purchasable1 = list.FirstOrDefault(match => match.nameKey == DemolishKey);
                if (purchasable1 == null)
                    return;
                list.Remove(purchasable1);
                PurchaseUI.Purchasable purchasable2 = list.FirstOrDefault(match => match.nameKey == ClearCropKey);
                if (purchasable2 != null)
                {
                    list.Remove(purchasable2);
                    list.Add(purchasable2);
                }
                list.Add(purchasable1);
                purchasables = list.ToArray();
            });
        }

        /// <summary>
        /// Creates a <see cref="LandPlot.Upgrade"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="LandPlot.Upgrade"/>.</param>
        /// <param name="name">The name of the <see cref="LandPlot.Upgrade"/>.</param>
        /// <returns>The created <see cref="LandPlot.Upgrade"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static LandPlot.Upgrade CreateLandPlotUpgrade(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register landplot upgrades outside of the PreLoad step");
            return moddedUpgrades.RegisterValueWithEnum((LandPlot.Upgrade)value, name);
        }

        /// <summary>
        /// Registers an land plot upgrade.
        /// </summary>
        /// <typeparam name="T">The type of land plot to register it to.</typeparam>
        /// <param name="entry">The upgrade to register.</param>
        public static void RegisterPurchasableUpgrade<T>(UpgradeShopEntry entry) where T : LandPlotUI => 
            PurchasableUIRegistry.RegisterPurchasable((PurchasableUIRegistry.PurchasableCreatorDelegateGeneric<T>)(x => 
            new PurchaseUI.Purchasable(entry.NameKey, entry.icon, entry.mainImg, entry.DescKey, entry.cost, entry.landplotPediaId, 
                () => x.Upgrade(entry.upgrade, entry.cost), entry.isUnlocked != null ? () => entry.isUnlocked(x.activator) : (Func<bool>)(() => true), 
                entry.isAvailable != null ? () => entry.isAvailable(x.activator) : (System.Func<bool>)(() => !x.activator.HasUpgrade(entry.upgrade)), 
                warning: (entry.warning ?? null), requireHoldToPurchase: entry.holdtopurchase)));

        /// <summary>
        /// Registers a <see cref="PlotUpgrader"/> to a land plot.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlotUpgrader"/> to register.</typeparam>
        /// <param name="plot">The <see cref="LandPlot.Id"/> to register the <see cref="PlotUpgrader"/> to</param>
        /// <exception cref="ArgumentException">Throws if the <see cref="PlotUpgrader"/> is already registered.</exception>
        public static void RegisterPlotUpgrader<T>(LandPlot.Id plot) where T : PlotUpgrader
        {
            if (moddedUpgraders.Exists((x) => x.Item1 == typeof(T) && x.Item2 == plot))
                throw new ArgumentException($"Type \'{typeof(T).FullName}\' is already registered as a plot upgrader for {plot}");
            moddedUpgraders.Add((typeof(T), plot));
            if (SRModLoader.CurrentLoadingStep == SRModLoader.LoadingStep.PRELOAD)
                SRCallbacks.OnGameContextReady += () => AddUpgraderComponents<T>(plot);
            else
                AddUpgraderComponents<T>(plot);
        }

        internal static void AddUpgraderComponents<T>(LandPlot.Id plot) where T : PlotUpgrader
        {
            foreach (var land in Resources.FindObjectsOfTypeAll<LandPlot>())
                if (land.typeId == plot)
                    land.gameObject.AddComponent<T>();
        }

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
