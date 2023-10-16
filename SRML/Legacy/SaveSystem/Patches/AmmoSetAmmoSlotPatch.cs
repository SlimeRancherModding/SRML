using HarmonyLib;
using SRML.SR.SaveSystem.Data.Ammo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("SetAmmoSlot")]
    internal static class AmmoSetAmmoSlotPatch
    {
        public static void Postfix(Ammo __instance, int idx,bool __result)
        {
            if (idx < __instance.ammoModel.usableSlots)
            {
                if (AmmoIdentifier.TryGetIdentifier(__instance, out var identifier))
                {
                    if (PersistentAmmoManager.HasPersistentAmmo(identifier))
                    {
                        PersistentAmmoManager.PersistentAmmoData[identifier].OnSelected(__instance.GetSelectedId(), __instance.selectedAmmoIdx);
                    }
                }
            }
        }
    }
}
