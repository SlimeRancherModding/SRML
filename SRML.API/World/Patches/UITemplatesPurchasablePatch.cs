using HarmonyLib;
using System;
using System.Diagnostics;
using System.Linq;

namespace SRML.API.World.Patches
{
    [HarmonyPatch(typeof(UITemplates))]
    [HarmonyPatch("CreatePurchaseUI")]
    internal static class UITemplatesPurchasablePatch
    {
        public static void Prefix(ref PurchaseUI.Purchasable[] purchasables, PurchaseUI.OnClose onClose)
        {
            StackTrace trace = new StackTrace(1);
            Type type = null;
            foreach(var v in trace.GetFrames())
            {
                var candidateType = v.GetMethod().DeclaringType;
                if (typeof(BaseUI).IsAssignableFrom(candidateType))
                {
                    type = candidateType;
                    break;
                }
            }
            if (type == null) 
                return;

            BaseUI ui = (BaseUI)onClose.Target;
            PurchasableRegistry registry = PurchasableRegistry.Instance;

            purchasables = purchasables.Union(registry.Registered.Where(x => x.Item1 == type).Select(x => x.Item2.Invoke(ui))).ToArray();

            Console.Console.Instance.Log(type);
            Console.Console.Instance.Log(registry.Registered.Any(x => x.Item1 == type));
            if (registry.manipulators.Keys.TryGetValue(x => x == type || type.IsSubclassOf(x) || x.IsAssignableFrom(type), out Type found))
                foreach (var v in registry.manipulators[found]) 
                    v(ui, ref purchasables);
        }
    }
}
