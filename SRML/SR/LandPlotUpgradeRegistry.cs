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
        internal static IDRegistry<LandPlot.Upgrade> moddedUpgrades = new IDRegistry<LandPlot.Upgrade>();
        

        static LandPlotUpgradeRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedUpgrades);
            EnumPatcher.RegisterAlternate(typeof(LandPlot.Upgrade), (obj, name) => CreateLandPlotUpgrade(obj, name));
        }

        public static LandPlot.Upgrade CreateLandPlotUpgrade(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register landplot upgrades outside of the PreLoad step");
            return moddedUpgrades.RegisterValueWithEnum((LandPlot.Upgrade)value, name);
        }

        public static void RegisterPurchasableUpgrade<T>(UpgradeShopEntry entry) where T : LandPlotUI
        {
            PurchasableUIRegistry.RegisterPurchasable<T>((x) => new PurchaseUI.Purchasable(entry.NameKey,entry.icon,entry.mainImg,entry.DescKey,entry.cost,entry.landplotPediaId,()=> {
                x.Upgrade(entry.upgrade, entry.cost);
            },entry.isUnlocked ?? (()=>true),()=>!x.activator.HasUpgrade(entry.upgrade)));
        }

        public static void RegisterPlotUpgrader<T>(LandPlot.Id plot) where T : PlotUpgrader
        {
            switch (SRModLoader.CurrentLoadingStep)
            {
                case SRModLoader.LoadingStep.PRELOAD:
                    SRCallbacks.OnGameContextReady += () =>
                    {
                        GameContext.Instance.LookupDirector.GetPlotPrefab(plot).AddComponent<T>();
                    };
                    break;
                default:
                    GameContext.Instance.LookupDirector.GetPlotPrefab(plot).AddComponent<T>();
                    break;
            }
        }

        public struct UpgradeShopEntry
        {
            public LandPlot.Upgrade upgrade;
            public Sprite icon;
            public Sprite mainImg;
            public int cost;
            public PediaDirector.Id landplotPediaId;
            public Func<bool> isUnlocked;

            string landplotName;

            public string LandPlotName
            {
                get
                {
                    if (landplotName != null) return landplotName;
                    return landplotPediaId.ToString().ToLower();
                }
                set
                {
                    landplotName = value.ToLower();
                }
            }

            public string DescKey => $"m.upgrade.desc.{landplotName}.{upgrade.ToString().ToLower()}";
            public string NameKey => $"m.upgrade.name.{landplotName}.{upgrade.ToString().ToLower()}";
        }
    }
}
