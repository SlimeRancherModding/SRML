using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using SRML.SR.SaveSystem.Data.Ammo;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("DecrementSelectedAmmo")]
    internal static class DecrementSelectedAmmoPatch
    {
        public static void Postfix(Ammo __instance, int amount)
        {
            if (AmmoIdentifier.TryGetIdentifier(__instance, out var identifier))
            {
                if (PersistentAmmoManager.HasPersistentAmmo(identifier))
                {
                    PersistentAmmoManager.OnAmmoDecrement(identifier,__instance.selectedAmmoIdx,amount);
                }
            }
        }
    }
}
