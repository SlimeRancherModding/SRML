using SRML.Core.API.BuiltIn;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SRML.API.World
{
    public class LandPlotRegistry : EnumRegistry<LandPlotRegistry, LandPlot.Id>
    {
        public override void Process(LandPlot.Id toProcess)
        {
        }

        public void RegisterAsPurchasable(LandPlotUI.PlotPurchaseItem purchaseItem, string titleKey, string descKey,
            Func<bool> unlockCondition, Func<bool> availCondition, PediaDirector.Id? pediaId,
            string btnOverride = null, string warning = null, Func<int> currCount = null, GadgetDefinition.CraftCost[] craftCosts = null,
            bool requireHoldToPurchase = false) =>
            PurchasableRegistry.Instance.Register<EmptyPlotUI>(x => new PurchaseUI.Purchasable(titleKey, purchaseItem.icon, purchaseItem.img, descKey,
                purchaseItem.cost, pediaId, new UnityAction(() => ((LandPlotUI)x).BuyPlot(purchaseItem)), unlockCondition, availCondition, btnOverride, warning,
                currCount, craftCosts, requireHoldToPurchase));
    }
}
