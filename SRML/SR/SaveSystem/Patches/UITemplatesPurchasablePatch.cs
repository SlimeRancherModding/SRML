using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(UITemplates))]
    [HarmonyPatch("CreatePurchaseUI")]
    internal static class UITemplatesPurchasablePatch
    {
        public static void Prefix(ref PurchaseUI.Purchasable[] purchasables)
        {
            StackTrace trace = new StackTrace(1);
            foreach(var v in trace.GetFrames())
            {
                var type = v.GetMethod().DeclaringType;
                if (typeof(BaseUI).IsAssignableFrom(type))
                {
                    UnityEngine.Debug.Log(type);
                    break;
                }
            }
            purchasables = purchasables.AddToArray(new PurchaseUI.Purchasable("test", null, null, "test", 100, null, null, () => true, () => true));
        }
    }
}
