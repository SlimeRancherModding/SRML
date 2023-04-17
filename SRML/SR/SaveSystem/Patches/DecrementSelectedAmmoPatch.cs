using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using SRML.SR.SaveSystem.Data.Ammo;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("DecrementSelectedAmmo")]
    internal static class DecrementSelectedAmmoPatch
    {
        public static void Prefix(Ammo __instance, ref Identifiable.Id __state)
        {
            __state = __instance.Slots[__instance.selectedAmmoIdx].id;
        }

        public static void Postfix(Ammo __instance, int amount, Identifiable.Id __state)
        {
            if (AmmoIdentifier.TryGetIdentifier(__instance, out var identifier))
            {
                if (PersistentAmmoManager.HasPersistentAmmo(identifier))
                    PersistentAmmoManager.OnAmmoDecrement(identifier, __instance.selectedAmmoIdx, amount, __state);
            }
        }
    }

    [HarmonyPatch(typeof(Ammo), "Decrement", new[] { typeof(int), typeof(int) })]
    internal static class DecrementAmmoPatch
    {
        public static void Prefix(Ammo __instance, int index, ref Identifiable.Id __state)
        {
            __state = __instance.Slots[index].id;
        }

        public static void Postfix(Ammo __instance, int index, int count, Identifiable.Id __state)
        {
            if (AmmoIdentifier.TryGetIdentifier(__instance, out var identifier))
            {
                if (PersistentAmmoManager.HasPersistentAmmo(identifier))
                    PersistentAmmoManager.OnAmmoDecrement(identifier, index, count, __state);
            }
        }
    }
}
