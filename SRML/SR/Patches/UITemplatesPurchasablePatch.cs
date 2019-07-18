using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(UITemplates))]
    [HarmonyPatch("CreatePurchaseUI")]
    internal static class UITemplatesPurchasablePatch
    {
        public static void Prefix(ref PurchaseUI.Purchasable[] purchasables, PurchaseUI.OnClose onClose)
        {
            StackTrace trace = new StackTrace(1);
            Type type=null;
            foreach(var v in trace.GetFrames())
            {
                var candidateType = v.GetMethod().DeclaringType;
                if (typeof(BaseUI).IsAssignableFrom(candidateType))
                {
                    type = candidateType;
                    break;
                }
            }
            if (type == null) return;
            BaseUI ui = onClose.Target as BaseUI;
            if (ui == null || type != ui.GetType()) throw new Exception();

            purchasables = PurchasableUIRegistry.customPurchasables.Where(x => x.Key(type, ui)).Select(x => x.Value(ui)).ToArray().AddRangeToArray(purchasables);
        }
    }
}
