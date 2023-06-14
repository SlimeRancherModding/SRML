using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(PlayerState))]
    [HarmonyPatch("GetPotentialAmmo")]
    internal static class PlayerStatePotentialAmmoPatch
    {
        public static void Postfix(HashSet<global::Identifiable.Id> __result)
        {
            foreach (var v in PlayerAmmoRegistry.Instance.Registered.Where((x) => x.Item1 == PlayerState.AmmoMode.DEFAULT))
                __result.Add(v.Item2);
        }
    }
}
