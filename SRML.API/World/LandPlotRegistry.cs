using HarmonyLib;
using SRML.Core.API.BuiltIn;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SRML.API.World
{
    public class LandPlotRegistry : EnumRegistry<LandPlotRegistry, LandPlot.Id>
    {
        internal Dictionary<string, List<LandPlot>> moddedLandplots = new Dictionary<string, List<LandPlot>>();

        public delegate void LandPlotRegisterEvent(LandPlot plot);
        public readonly LandPlotRegisterEvent OnRegisterLandPlot;

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterPrefabs(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var landplot in moddedLandplots.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.plotPrefabDict[landplot.typeId] = landplot.gameObject;
                lookupDirector.plotPrefabs.items.Add(landplot.gameObject);
            }
        }

        public virtual void RegisterPrefab(LandPlot toy)
        {
            if (toy.typeId == LandPlot.Id.NONE)
                throw new ArgumentException("Attempting to register a landplot with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedLandplots.ContainsKey(executingId))
                moddedLandplots[executingId] = new List<LandPlot>();

            moddedLandplots[executingId].Add(toy);
            OnRegisterLandPlot?.Invoke(toy);
        }

        public void RegisterAsPurchasable(LandPlotUI.PlotPurchaseItem purchaseItem, string titleKey, string descKey,
            Func<bool> unlockCondition, Func<bool> availCondition, PediaDirector.Id? pediaId,
            string btnOverride = null, string warning = null, Func<int> currCount = null, GadgetDefinition.CraftCost[] craftCosts = null,
            bool requireHoldToPurchase = false) =>
            PurchasableRegistry.Instance.Register<EmptyPlotUI>(x => new PurchaseUI.Purchasable(titleKey, purchaseItem.icon, purchaseItem.img, descKey,
                purchaseItem.cost, pediaId, new UnityAction(() => ((LandPlotUI)x).BuyPlot(purchaseItem)), unlockCondition, availCondition, btnOverride, warning,
                currCount, craftCosts, requireHoldToPurchase));

        public override void Initialize()
        {
        }
    }
}
