using HarmonyLib;
using System.Linq;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(PlayerState))]
    [HarmonyPatch("Reset")]
    internal static class PlayerStateResetPatch
    {
        public static void Postfix(PlayerState __instance)
        {
            foreach (var v in PlayerAmmoRegistry.Instance.Registered.Where((x) => x.Item1 != PlayerState.AmmoMode.DEFAULT))
                __instance.ammoDict[v.Item1].potentialAmmo.Add(v.Item2);
        }
    }
}
