using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PediaUI))]
    [HarmonyPatch("SelectTabForId")]
    internal static class PediaUISelectTabPatch
    {
        public static void Prefix(PediaUI __instance, PediaDirector.Id id)
        {
            if (id == PediaDirector.Id.LOCKED) return;

            if (PediaRegistry.activeTabRenderer != null)
            {
                PediaRegistry.activeTabRenderer.OnTabDeselected(__instance.gameObject);
                PediaRegistry.activeTabRenderer = null;
            }

            foreach (var v in PediaRegistry.customTabs)
            {
                if (v.Key.Entries.Contains(id))
                {
                    v.Key.TabToggle.isOn = true;

                    __instance.BuildListing(v.Key.Entries.ToArray());
                    PediaRegistry.activeTabRenderer = v.Key.TabRenderer;
                    if (PediaRegistry.activeTabRenderer != null) PediaRegistry.activeTabRenderer.OnTabSelected(__instance.gameObject);
                }
            }
        }
    }
}
