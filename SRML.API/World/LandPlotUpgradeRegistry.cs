using SRML.Console;
using SRML.Core.API.BuiltIn;
using SRML.SR;
using System;
using System.Linq;
using UnityEngine.Events;

namespace SRML.API.World
{
    public class LandPlotUpgradeRegistry : EnumRegistry<LandPlotUpgradeRegistry, LandPlot.Upgrade>
    {
        public const string DemolishKey = "%ui:l.demolish_plot";
        public const string ClearCrop = "%ui:b.clear_crop";

        public LandPlotUI.PlotPurchaseItem Demolish { get; private set; }

        public override void Initialize()
        {
            PurchasableRegistry.Instance.RegisterManipulator<LandPlotUI>((BaseUI x, ref PurchaseUI.Purchasable[] y) =>
            {
                PurchaseUI.Purchasable demolish = y.FirstOrDefault(z => z.nameKey == DemolishKey);
                PurchaseUI.Purchasable crop = y.FirstOrDefault(z => z.nameKey == ClearCrop);

                if (crop != null)
                    y = y.Where(z => z != crop).Append(crop).ToArray();
                if (demolish != null)
                    y = y.Where(z => z != demolish).Append(demolish).ToArray();
            });

            SRCallbacks.OnGameContextReady += () => Demolish =
                GameContext.Instance.LookupDirector.GetPlotPrefab(LandPlot.Id.POND).GetComponent<LandPlot>().GetComponentInChildren<UIActivator>().
                uiPrefab.GetComponent<PondUI>().demolish;
        }

        public override void Process(LandPlot.Upgrade toProcess)
        {
        }

        public void RegisterAsPurchasable<T>(LandPlotUI.UpgradePurchaseItem purchaseItem, string titleKey, string descKey,
            Func<bool> unlockCondition, Func<bool> availCondition, PediaDirector.Id? pediaId,
            string btnOverride = null, string warning = null, Func<int> currCount = null, GadgetDefinition.CraftCost[] craftCosts = null,
            bool requireHoldToPurchase = false) where T : LandPlotUI =>
            PurchasableRegistry.Instance.Register<T>(x => new PurchaseUI.Purchasable(titleKey, purchaseItem.icon, purchaseItem.img, descKey,
                purchaseItem.cost, pediaId, new UnityAction(() => ((LandPlotUI)x).Upgrade(purchaseItem.upgrade, purchaseItem.cost)), 
                unlockCondition, () => availCondition.Invoke() && !((LandPlotUI)x).activator.HasUpgrade(purchaseItem.upgrade), btnOverride, warning, currCount, 
                craftCosts, requireHoldToPurchase));

        public void AddUpgraderToPlot<T>(LandPlot.Id id) where T : PlotUpgrader => AddUpgraderToPlot(id, typeof(T));

        public void AddUpgraderToPlot(LandPlot.Id id, Type t)
        {
            if (!t.IsSubclassOf(typeof(PlotUpgrader)))
                throw new ArgumentException("Can't add non-PlotUpgrader type to LandPlot");

            GameContext.Instance.LookupDirector.GetPlotPrefab(id).AddComponent(t);
        }
    }
}
