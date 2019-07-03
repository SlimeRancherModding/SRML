using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PlayerState))]
    [HarmonyPatch("GetPotentialAmmo")]
    internal static class PlayerStateGetPotentialAmmoPatch
    {
        public static void Postfix(HashSet<Identifiable.Id> __result)
        {
            if (!AmmoRegistry.inventoryPrefabsToPatch.ContainsKey(PlayerState.AmmoMode.DEFAULT)) return;
            foreach (var v in AmmoRegistry.inventoryPrefabsToPatch[PlayerState.AmmoMode.DEFAULT]) __result.Add(v);
        }
    }
}
